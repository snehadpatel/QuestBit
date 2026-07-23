using Echo.Core.DependencyInjection;
using Echo.Core.Input;
using UnityEngine;

namespace Echo.Gameplay.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float _walkSpeed = 4.5f;
        [SerializeField] private float _gravity = -9.81f;
        [SerializeField] private float _interactionDistance = 3.0f;

        [Header("Asymmetric Perspective")]
        [SerializeField] private AsymmetricCamera _playerCamera;

        private CharacterController _characterController;
        private Vector3 _velocity;
        private IInputService _inputService;

        [Inject]
        public void Construct(IInputService inputService)
        {
            _inputService = inputService;
        }

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
        }

        private void Start()
        {
            if (_inputService == null)
            {
                ServiceContainer.Global.InjectDependencies(this);
            }
        }

        private void Update()
        {
            HandleMovement();
            HandleInteraction();
        }

        private void HandleMovement()
        {
            Vector2 moveInput = _inputService?.MoveInput ?? Vector2.zero;
            Vector3 moveVector = transform.right * moveInput.x + transform.forward * moveInput.y;

            _characterController.Move(moveVector * _walkSpeed * Time.deltaTime);

            if (_characterController.isGrounded && _velocity.y < 0)
            {
                _velocity.y = -2f;
            }

            _velocity.y += _gravity * Time.deltaTime;
            _characterController.Move(_velocity * Time.deltaTime);
        }

        private void HandleInteraction()
        {
            if (_inputService != null && _inputService.InteractPressed)
            {
                if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, _interactionDistance))
                {
                    Debug.Log($"[PlayerController] Interacted with: {hit.collider.gameObject.name}");
                }
            }
        }
    }
}
