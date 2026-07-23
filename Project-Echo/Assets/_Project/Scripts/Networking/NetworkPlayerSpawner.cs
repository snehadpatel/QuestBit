using System.Collections.Generic;
using Echo.Gameplay.Player;
using UnityEngine;

namespace Echo.Networking
{
    public class NetworkPlayerSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject _playerPrefab;
        [SerializeField] private Transform[] _spawnPoints;
        
        private readonly Dictionary<int, GameObject> _spawnedPlayers = new Dictionary<int, GameObject>();

        public void SpawnPlayer(int playerIndex)
        {
            if (_playerPrefab == null)
            {
                Debug.LogError("[NetworkPlayerSpawner] Player prefab is unassigned!");
                return;
            }

            Transform spawnPoint = (_spawnPoints != null && _spawnPoints.Length > playerIndex)
                ? _spawnPoints[playerIndex]
                : transform;

            GameObject playerInstance = Instantiate(_playerPrefab, spawnPoint.position, spawnPoint.rotation);
            playerInstance.name = $"Player_{playerIndex + 1}";
            _spawnedPlayers[playerIndex] = playerInstance;

            Debug.Log($"[NetworkPlayerSpawner] Spawned Player {playerIndex + 1} at {spawnPoint.position}");
        }

        public void DespawnPlayer(int playerIndex)
        {
            if (_spawnedPlayers.TryGetValue(playerIndex, out GameObject playerInstance))
            {
                Destroy(playerInstance);
                _spawnedPlayers.Remove(playerIndex);
                Debug.Log($"[NetworkPlayerSpawner] Despawned Player {playerIndex + 1}");
            }
        }
    }
}
