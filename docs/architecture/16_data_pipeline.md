# Architectural Specification: Data Pipeline & Telemetry

* **Status**: APPROVED
* **Date**: 2026-07-09
* **Engine Focus**: Unity 6 LTS

---

## 1. Design Intent & Requirements Traceability

The Data Pipeline captures, caches, and transmits educational and system gameplay logs. It directly implements our learning pedagogy and strict privacy policies:

* **COPPA & GDPR-K Compliance (Vision §5 & §9 & GDD §1.2 & §15.2)**: To ensure child data safety, QuestBit **excludes all third-party analytics and advertising SDKs** (e.g., Unity Analytics, Google Analytics, Firebase SDKs). This blocks advertising identifier (IDFA/GAID) leaks. All data is collected via a first-party HTTPS API under data minimization rules.
* **Productive Struggle Analytics (Vision §3 & §9 & GDD §2.3 & Ch. 10)**: The pipeline tracks "productive struggle"—recording the time elapsed, active retries, and eventual success milestones for fraction bridges or glyph paths—to feed the hidden **Mastery Engine** and populate parent-facing dashboards.
* **Parent Report Validation (Vision §10)**: Events provide clean, parsed educational signals (skills unlocked, conceptual mistakes logged as clues) rather than engagement metrics like raw tap counts.

---

## 2. Telemetry Schema Specification (JSON)

Every event is serialized into a standard, first-party schema. The payload isolates learning analytics from device metadata.

### 2.1 Base Event Schema

```json
{
  "eventId": "e93f8e5c-0294-4d89-ab83-c2837f8dbd84",
  "playerSessionId": "2a3c4d5e-6f7g-8h9i-0j1k-2l3m4n5o6p7q",
  "timestamp": 1783598000,
  "eventType": "EDUCATIONAL_PROGRESS",
  "payload": {
    "skillId": "math_fraction_halves",
    "oldStage": 2,
    "newStage": 3,
    "triggerSource": "quest_cove_halfway_dock_completed"
  }
}
```

### 2.2 Productive Struggle Event Payload

```json
{
  "eventId": "a1b2c3d4-e5f6-7a8b-9c0d-1e2f3a4b5c6d",
  "playerSessionId": "2a3c4d5e-6f7g-8h9i-0j1k-2l3m4n5o6p7q",
  "timestamp": 1783598250,
  "eventType": "PRODUCTIVE_STRUGGLE",
  "payload": {
    "skillId": "math_fraction_quarters",
    "durationSeconds": 142.5,
    "retryCount": 4,
    "cluesLogged": [
      "too_short_by_quarter",
      "overspan_by_half"
    ],
    "resolvedWithSuccess": true
  }
}
```

---

## 3. Data Pipeline C# Interfaces & Batch Manager

The Data Pipeline queues events locally and flushes them to our HTTPS endpoint in batches when internet connectivity is available.

### 3.1 Interface Contracts

```csharp
using Cysharp.Threading.Tasks;

namespace QuestBit.Systems.DataPipeline
{
    public enum EventCategory
    {
        EducationalProgress,
        ProductiveStruggle,
        SystemPerformance,  // Frame drops, WebGL OOM alerts
        ParentPortalInteraction
    }

    public interface IDataPipeline
    {
        /// <summary>
        /// Enqueues a telemetry event. The event is serialized and written to the local disk queue immediately.
        /// </summary>
        void LogEvent(EventCategory category, string eventName, object payload);

        /// <summary>
        /// Forces a flush of the local queue, sending batched events to the first-party server.
        /// </summary>
        UniTask FlushQueueAsync();

        void SetOnlineStatus(bool isOnline);
    }
}
```

### 3.2 Telemetry Queue Controller

```csharp
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Cysharp.Threading.Tasks;
using QuestBit.Systems.Networking;

namespace QuestBit.Systems.DataPipeline
{
    public class TelemetryQueueController : IDataPipeline
    {
        private readonly List<string> _memoryQueue = new List<string>(10);
        private readonly string _cacheFilePath;
        private readonly INetworkClient _networkClient;
        
        private bool _isOnline;
        private bool _isFlushing;

        private const int BATCH_SIZE_TRIGGER = 10;
        private const long MAX_CACHE_SIZE_BYTES = 10 * 1024 * 1024; // 10MB limit

        public TelemetryQueueController(INetworkClient networkClient, string persistentDataPath)
        {
            _networkClient = networkClient;
            _cacheFilePath = Path.Combine(persistentDataPath, "telemetry_cache.json");
        }

        public void LogEvent(EventCategory category, string eventName, object payload)
        {
            var telemetryEvent = new TelemetryEventWrapper
            {
                EventId = System.Guid.NewGuid().ToString(),
                Timestamp = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                Category = category.ToString(),
                EventName = eventName,
                PayloadJson = JsonUtility.ToJson(payload)
            };

            string serializedEvent = JsonUtility.ToJson(telemetryEvent);

            lock (_memoryQueue)
            {
                _memoryQueue.Add(serializedEvent);
                
                // Write immediately to the local backup file to prevent loss if app crashes
                AppendToLocalCache(serializedEvent);

                if (_memoryQueue.Count >= BATCH_SIZE_TRIGGER && _isOnline && !_isFlushing)
                {
                    FlushQueueAsync().Forget();
                }
            }
        }

        public async UniTask FlushQueueAsync()
        {
            if (_isFlushing || !_isOnline) return;

            _isFlushing = true;
            List<string> batchToSend;

            lock (_memoryQueue)
            {
                if (_memoryQueue.Count == 0)
                {
                    _isFlushing = false;
                    return;
                }
                batchToSend = new List<string>(_memoryQueue);
            }

            // Create JSON batch array payload
            string batchJson = "[" + string.Join(",", batchToSend) + "]";
            
            // Send batch to our first-party HTTPS server (GCP Cloud Run Endpoint)
            var response = await _networkClient.PostJsonAsync("https://telemetry.questbit.com/v1/ingest", batchJson);

            if (response.Success)
            {
                lock (_memoryQueue)
                {
                    // Remove sent events from the active memory queue
                    _memoryQueue.RemoveRange(0, batchToSend.Count);
                    ClearLocalCache(batchToSend.Count);
                }
            }

            _isFlushing = false;
        }

        public void SetOnlineStatus(bool isOnline)
        {
            _isOnline = isOnline;
            if (_isOnline)
            {
                FlushQueueAsync().Forget();
            }
        }

        private void AppendToLocalCache(string jsonLine)
        {
            try
            {
                var fileInfo = new FileInfo(_cacheFilePath);
                if (fileInfo.Exists && fileInfo.Length > MAX_CACHE_SIZE_BYTES)
                {
                    // Enforce Cache Limit: delete oldest entries (FIFO)
                    PruneLocalCache();
                }
                
                File.AppendAllText(_cacheFilePath, jsonLine + Environment.NewLine);
            }
            catch (IOException ex)
            {
                Debug.LogError($"[Telemetry] Failed to write local cache: {ex.Message}");
            }
        }

        private void PruneLocalCache()
        {
            // Read lines, remove the first 20% of entries, and rewrite file
            var lines = new List<string>(File.ReadAllLines(_cacheFilePath));
            if (lines.Count > 100)
            {
                int pruneCount = lines.Count / 5;
                lines.RemoveRange(0, pruneCount);
                File.WriteAllLines(_cacheFilePath, lines);
                Debug.LogWarning($"[Telemetry] Cache exceeded 10MB. Pruned oldest {pruneCount} lines.");
            }
        }

        private void ClearLocalCache(int count)
        {
            try
            {
                if (!File.Exists(_cacheFilePath)) return;
                
                var lines = new List<string>(File.ReadAllLines(_cacheFilePath));
                if (lines.Count >= count)
                {
                    lines.RemoveRange(0, count);
                    File.WriteAllLines(_cacheFilePath, lines);
                }
            }
            catch (IOException ex)
            {
                Debug.LogError($"[Telemetry] Failed to update local cache: {ex.Message}");
            }
        }
    }

    [System.Serializable]
    public struct TelemetryEventWrapper
    {
        public string EventId;
        public long Timestamp;
        public string Category;
        public string EventName;
        public string PayloadJson;
    }
}
```

---

## 4. COPPA and GDPR-K Data Minimization Rules

The pipeline enforces these rules to protect student privacy:

1. **No External Ad Trackers**: The application config contains **zero dependencies** on third-party SDK platforms (e.g. Firebase, Unity Gaming Services Analytics, GameAnalytics).
2. **IP Scrubbing**: The first-party ingestion server discards the client IP address in memory before logs are written to our database.
3. **Local-First Default**: If parents do not opt-in to cloud backups, telemetry data is cached locally for parent reports and is never transmitted to the internet.

---

## 5. Failure Modes & Edge Cases

### 1. Network Disconnect during Transmit
* **Symptom**: Telemetry data fails to transmit due to cellular connection drop.
* **Mitigation**: The `FlushQueueAsync` method catches timeouts. If transmission fails, the events remain in the local cache, and the pipeline schedules a retry using an **exponential back-off delay (10s, 30s, 60s, 300s)**.

### 2. Disk Storage Full (Chromebook)
* **Symptom**: App persistent storage has reached its limit, throwing disk write exceptions.
* **Mitigation**: If `AppendToLocalCache` throws an exception, the pipeline stops writing to disk and holds events in a small, fixed-size memory array (maximum 50 events). Oldest events are dropped if the memory array overflows, preserving core game functionality.

---

## 6. Verification & Automated Audits

1. **Third-Party SDK Auditing Test (CI/CD)**:
   A CI build check script scans all package files (`questbit-unity/Packages/manifest.json`) and compiled libraries inside `Assets/ThirdParty/`.
   * *Pass Criteria*: The build is rejected if references to known analytics or advertising SDKs (e.g., Firebase, Unity Ads, Adjust, AppsFlyer) are found.

2. **Batch Transmission Integration Test**:
   Log 15 events sequentially while online. Verify that the batch system bundles the first 10 events into a single payload, posts it to the mock server, and clears the matching lines from the local file cache.
