# Architectural Specification: Networking Architecture

* **Status**: APPROVED
* **Date**: 2026-07-09
* **Engine Focus**: Unity 6 LTS

---

## 1. Design Intent & Requirements Traceability

The Networking Architecture governs save game synchronization, asynchronous cooperative play, and data transfer. It is built to enforce child safety and offline-first stability:

* **Strict Child Safety & Privacy (Vision §4 & §5 & GDD §1.2 & §3.5)**: Head-to-head multiplayer, live lobbies, and live text/voice chat are **strictly banned** for the Core age band (6–9). Social interaction is async-only (gifting cosmetics, leaving preset barks, viewing ghost structures like the Community Dock).
* **Parental Gates (Vision §5 & §10 & GDD §9.2)**: Adding friends via code linkages, registering cloud profiles, or processing subscription syncs must be gated behind a parental PIN validation prompt.
* **Offline-First Networking (Vision §5 & GDD §1.2)**: Gameplay must never block on network responses. The network module runs completely in the background, caching failed requests in a local persistent queue and retrying when a connection is established.

---

## 2. API Schema Specification (Async REST Contracts)

QuestBit interacts with our backend using a secure, first-party REST API over HTTPS.

### 2.1 Sync Save Game State (`POST /v1/save/sync`)
Sends local save file data and retrieves remote cloud data.
* *Request Body*:
  ```json
  {
    "clientTimestamp": 1783599000,
    "saveVersion": 4,
    "payloadEncrypted": "a8f93...[AES-256 encrypted payload]"
  }
  ```
* *Response Body (Success)*:
  ```json
  {
    "status": "SYNC_SUCCESS",
    "serverTimestamp": 1783599002,
    "actionRequired": "NONE"
  }
  ```
* *Response Body (Conflict - Server has newer progress)*:
  ```json
  {
    "status": "CONFLICT",
    "serverTimestamp": 1783599002,
    "actionRequired": "RESOLVE_MERGE",
    "remotePayloadEncrypted": "c7b82...[Server save data]"
  }
  ```

### 2.2 Send Gift Package (`POST /v1/gift/send`)
Enables async gifting (GDD §6.2) to linked friend accounts.
* *Request Body*:
  ```json
  {
    "senderAnonymousId": "anon_8f3d2a",
    "recipientFriendCode": "QBIT-482-901",
    "giftItemId": "charm_ferro_knot",
    "presetMessageKey": "dialogue_gift_message_cheer" 
  }
  ```

---

## 3. Networking Manager & Sync Queue (C#)

The Network Manager queues requests locally and resolves them using an exponential back-off retry schema.

### 3.1 Interface Contracts

```csharp
using Cysharp.Threading.Tasks;

namespace QuestBit.Systems.Networking
{
    public struct NetworkResponse
    {
        public bool Success;
        public long HttpCode;
        public string Body;
        public string ErrorMessage;
    }

    public interface INetworkClient
    {
        UniTask<NetworkResponse> PostJsonAsync(string url, string jsonPayload);
        UniTask<NetworkResponse> GetAsync(string url);
    }

    public interface INetworkingManager
    {
        bool IsConnected { get; }
        void QueueRequest(string apiEndpoint, string jsonPayload);
        UniTask ProcessQueueAsync();
    }
}
```

### 3.2 Network Sync Queue Controller

```csharp
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace QuestBit.Systems.Networking
{
    public class NetworkSyncQueue : MonoBehaviour, INetworkingManager
    {
        [SerializeField] private string _serverBaseUrl = "https://api.questbit.com";
        private readonly List<QueuedRequest> _queue = new List<QueuedRequest>(8);
        private string _queueCachePath = null!;
        private INetworkClient _client = null!;
        
        private bool _isProcessing;
        public bool IsConnected { get; private set; }

        public void Initialize(INetworkClient client, string persistentDataPath)
        {
            _client = client;
            _queueCachePath = Path.Combine(persistentDataPath, "network_queue.json");
            LoadQueueFromDisk();
        }

        public void QueueRequest(string apiEndpoint, string jsonPayload)
        {
            var request = new QueuedRequest
            {
                Endpoint = apiEndpoint,
                Payload = jsonPayload,
                RetryCount = 0,
                NextRetryTime = 0
            };

            lock (_queue)
            {
                _queue.Add(request);
                SaveQueueToDisk();
            }

            if (IsConnected && !_isProcessing)
            {
                ProcessQueueAsync().Forget();
            }
        }

        public async UniTask ProcessQueueAsync()
        {
            if (_isProcessing || _queue.Count == 0) return;
            _isProcessing = true;

            while (_queue.Count > 0)
            {
                // Verify connection before starting loop
                if (!EvaluateConnection())
                {
                    _isProcessing = false;
                    return;
                }

                QueuedRequest request;
                lock (_queue)
                {
                    request = _queue[0];
                }

                long currentTime = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                if (request.NextRetryTime > currentTime)
                {
                    // Delay processing if backing off
                    await UniTask.Delay(1000);
                    continue;
                }

                string targetUrl = $"{_serverBaseUrl}{request.Endpoint}";
                var response = await _client.PostJsonAsync(targetUrl, request.Payload);

                if (response.Success)
                {
                    lock (_queue)
                    {
                        _queue.RemoveAt(0);
                        SaveQueueToDisk();
                    }
                }
                else
                {
                    // Handle failure: apply exponential back-off
                    request.RetryCount++;
                    
                    // Delay equation: 5s, 25s, 125s, capped at 300 seconds
                    int delaySeconds = (int)Mathf.Min(Mathf.Pow(5, request.RetryCount), 300);
                    request.NextRetryTime = currentTime + delaySeconds;
                    
                    Debug.LogWarning($"[Network] Request to {request.Endpoint} failed (HTTP {response.HttpCode}). Retrying in {delaySeconds}s. (Attempt {request.RetryCount})");

                    lock (_queue)
                    {
                        _queue[0] = request; // Update retry parameters
                        SaveQueueToDisk();
                    }
                    
                    // Stop queue execution to prevent network spamming
                    break; 
                }
            }

            _isProcessing = false;
        }

        private bool EvaluateConnection()
        {
            IsConnected = Application.internetReachability != NetworkReachability.NotReachable;
            return IsConnected;
        }

        private void SaveQueueToDisk()
        {
            try
            {
                string json = JsonUtility.ToJson(new QueueWrapper { Requests = _queue });
                File.WriteAllText(_queueCachePath, json);
            }
            catch (IOException ex)
            {
                Debug.LogError($"[Network] Failed to save queue to disk: {ex.Message}");
            }
        }

        private void LoadQueueFromDisk()
        {
            if (!File.Exists(_queueCachePath)) return;
            try
            {
                string json = File.ReadAllText(_queueCachePath);
                var wrapper = JsonUtility.FromJson<QueueWrapper>(json);
                if (wrapper.Requests != null)
                {
                    _queue.Clear();
                    _queue.AddRange(wrapper.Requests);
                }
            }
            catch (IOException)
            {
                _queue.Clear();
            }
        }
    }

    [System.Serializable]
    public struct QueuedRequest
    {
        public string Endpoint;
        public string Payload;
        public int RetryCount;
        public long NextRetryTime;
    }

    [System.Serializable]
    public struct QueueWrapper
    {
        public List<QueuedRequest> Requests;
    }
}
```

---

## 4. Child-Safety Guardrails

To conform to our safety principles, the following controls are configured at compile-time:

1. **No Real-Time Lobbies**: The client build contains **zero networking code** for WebSockets, UDP sockets, or WebRTC lobby setups.
2. **Anonymous Identification**: Friends list linking uses a **non-PII Friend Code** (e.g. `QBIT-482-901`) randomly issued by the server. No email searches, phone synchronization, or social network links are allowed.
3. **Parental Verification Gate**: Before the network class attempts to sync a cloud backup or add a friend code, it dispatches an event requesting UI validation. The parent must solve a simple, narrated math equation or enter their passcode before the task executes.

---

## 5. Failure Modes & Edge Cases

### 1. WebGL Cross-Origin Resource Sharing (CORS) Blocks
* **Symptom**: WebGL build fails to communicate with the GCP endpoint, throwing `CORS Access-Control-Allow-Origin` errors in the browser console.
* **Mitigation**: The GCP cloud endpoint API must explicitly configure CORS headers to accept HTTP requests from our authorized school WebGL hosting domains (e.g. `play.questbit.com` or Google Classroom frames).

### 2. Server Down/Timeout
* **Symptom**: The GCP server undergoes a cold start or database maintenance, resulting in 503 errors.
* **Mitigation**: The `NetworkSyncQueue` exponential back-off handles this automatically. The queue stops sending requests and retries at escalating intervals (up to a 5-minute cap), preventing server overloading during recovery.

---

## 6. Verification & Automated QA

1. **Safety Scan (Build Check)**:
   Automated build validation checks that no compiled assembly imports namespaces from Unity Multiplayer (`UnityEngine.Networking` or similar legacy systems), ensuring no accidental lobby configurations exist in the production client.

2. **Sync Queue Resilience Test**:
   Disable `Application.internetReachability` via unit-test simulation. Enqueue **3 save sync requests**. Confirm that the requests are correctly written to the `network_queue.json` file cache. Re-enable network simulation and verify that the sync requests are successfully posted to the API.
