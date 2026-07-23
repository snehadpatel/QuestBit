using UnityEngine;

namespace Echo.Gameplay.Player
{
    public class AsymmetricCamera : MonoBehaviour
    {
        [Header("Per-Player Reality Layers")]
        [SerializeField] private Camera _targetCamera;
        [SerializeField] private LayerMask _playerSpecificCullingMask;
        
        [Header("Distortion / Fog Effects")]
        [SerializeField] private bool _enablePerspectiveDistortion = false;
        [SerializeField] private float _fogDensityOverride = 0.02f;

        private void Awake()
        {
            if (_targetCamera == null)
            {
                _targetCamera = GetComponent<Camera>();
            }
        }

        public void ApplyPlayerPerspective(int playerIndex, LayerMask customCullingMask)
        {
            if (_targetCamera != null)
            {
                _targetCamera.cullingMask = customCullingMask;
                UnityEngine.RenderSettings.fogDensity = _fogDensityOverride;
                Debug.Log($"[AsymmetricCamera] Applied reality filter for Player {playerIndex}. Culling Mask: {customCullingMask.value}");
            }
        }
    }
}
