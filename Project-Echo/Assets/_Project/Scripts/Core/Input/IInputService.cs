using System;
using UnityEngine;

namespace Echo.Core.Input
{
    public interface IInputService
    {
        Vector2 MoveInput { get; }
        Vector2 LookInput { get; }
        bool InteractPressed { get; }
        bool PrimaryActionPressed { get; }
        bool SecondaryActionPressed { get; }
        bool PausePressed { get; }

        event Action<bool> OnSwitchScanTriggered;

        void SetInputEnabled(bool enabled);
    }
}
