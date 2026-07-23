using System;
using UnityEngine;

namespace Echo.Core.Input
{
    public class InputService : MonoBehaviour, IInputService
    {
        public Vector2 MoveInput { get; private set; }
        public Vector2 LookInput { get; private set; }
        public bool InteractPressed { get; private set; }
        public bool PrimaryActionPressed { get; private set; }
        public bool SecondaryActionPressed { get; private set; }
        public bool PausePressed { get; private set; }

        public event Action<bool> OnSwitchScanTriggered;

        private bool _inputEnabled = true;

        private void Update()
        {
            if (!_inputEnabled)
            {
                MoveInput = Vector2.zero;
                LookInput = Vector2.zero;
                InteractPressed = false;
                PrimaryActionPressed = false;
                SecondaryActionPressed = false;
                PausePressed = false;
                return;
            }

            // Keyboard/Gamepad fallback mapping
            MoveInput = new Vector2(UnityEngine.Input.GetAxisRaw("Horizontal"), UnityEngine.Input.GetAxisRaw("Vertical")).normalized;
            LookInput = new Vector2(UnityEngine.Input.GetAxis("Mouse X"), UnityEngine.Input.GetAxis("Mouse Y"));
            
            InteractPressed = UnityEngine.Input.GetKeyDown(KeyCode.E) || UnityEngine.Input.GetButtonDown("Fire1");
            PrimaryActionPressed = UnityEngine.Input.GetMouseButtonDown(0);
            SecondaryActionPressed = UnityEngine.Input.GetMouseButtonDown(1);
            PausePressed = UnityEngine.Input.GetKeyDown(KeyCode.Escape);

            // Accessibility Single-Switch Trigger (Space / Enter / Gamepad South)
            if (UnityEngine.Input.GetKeyDown(KeyCode.Space) || UnityEngine.Input.GetKeyDown(KeyCode.Return))
            {
                OnSwitchScanTriggered?.Invoke(true);
            }
        }

        public void SetInputEnabled(bool enabled)
        {
            _inputEnabled = enabled;
        }
    }
}
