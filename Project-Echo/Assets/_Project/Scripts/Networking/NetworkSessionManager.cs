using System;
using UnityEngine;

namespace Echo.Networking
{
    public class NetworkSessionManager : MonoBehaviour
    {
        public enum NetworkMode
        {
            SinglePlayer,
            Host,
            Client
        }

        [SerializeField] private NetworkMode _currentMode = NetworkMode.SinglePlayer;
        [SerializeField] private int _maxPlayers = 4;
        [SerializeField] private string _sessionName = "EchoRoom_01";

        public NetworkMode CurrentMode => _currentMode;
        public bool IsServer => _currentMode == NetworkMode.Host || _currentMode == NetworkMode.SinglePlayer;

        public event Action<string> OnSessionJoined;
        public event Action<string> OnSessionFailed;

        public void StartHost(string sessionName)
        {
            _sessionName = sessionName;
            _currentMode = NetworkMode.Host;
            Debug.Log($"[NetworkSession] Hosting session '{_sessionName}' (Authoritative Host Model)...");
            OnSessionJoined?.Invoke(_sessionName);
        }

        public void JoinClient(string sessionName)
        {
            _sessionName = sessionName;
            _currentMode = NetworkMode.Client;
            Debug.Log($"[NetworkSession] Connecting to host session '{_sessionName}'...");
            OnSessionJoined?.Invoke(_sessionName);
        }

        public void Disconnect()
        {
            Debug.Log($"[NetworkSession] Disconnected from session '{_sessionName}'.");
            _currentMode = NetworkMode.SinglePlayer;
        }
    }
}
