using System;
using Echo.Core.StateMachine;
using UnityEngine;

namespace Echo.Gameplay.Creature
{
    public class CreatureAIController : MonoBehaviour
    {
        [Header("Creature Parameters")]
        [SerializeField] private CreatureAIState _currentState = CreatureAIState.Inactive;
        [SerializeField] private float _agitationLevel = 0.0f; // 0 to 100
        [SerializeField] private float _agitationThresholdTracking = 30.0f;
        [SerializeField] private float _agitationThresholdHunting = 75.0f;

        public CreatureAIState CurrentState => _currentState;
        public float AgitationLevel => _agitationLevel;

        public event Action<CreatureAIState> OnCreatureStateChanged;

        private void Update()
        {
            EvaluateStateTransitions();
        }

        public void AddNoiseOrPanic(float amount)
        {
            _agitationLevel = Mathf.Clamp(_agitationLevel + amount, 0f, 100f);
            Debug.Log($"[CreatureAI] Noise/Panic recorded (+{amount}). Current Agitation: {_agitationLevel}");
        }

        public void ReduceAgitation(float amount)
        {
            _agitationLevel = Mathf.Clamp(_agitationLevel - amount, 0f, 100f);
        }

        private void EvaluateStateTransitions()
        {
            CreatureAIState nextState = _currentState;

            switch (_currentState)
            {
                case CreatureAIState.Inactive:
                    if (_agitationLevel > 10.0f) nextState = CreatureAIState.Probing;
                    break;
                case CreatureAIState.Probing:
                    if (_agitationLevel >= _agitationThresholdTracking) nextState = CreatureAIState.Tracking;
                    else if (_agitationLevel <= 5.0f) nextState = CreatureAIState.Inactive;
                    break;
                case CreatureAIState.Tracking:
                    if (_agitationLevel >= _agitationThresholdHunting) nextState = CreatureAIState.Hunting;
                    else if (_agitationLevel < 20.0f) nextState = CreatureAIState.Probing;
                    break;
                case CreatureAIState.Hunting:
                    if (_agitationLevel < 40.0f) nextState = CreatureAIState.Retreating;
                    break;
                case CreatureAIState.Retreating:
                    nextState = CreatureAIState.Stalled;
                    break;
                case CreatureAIState.Stalled:
                    if (_agitationLevel < 10.0f) nextState = CreatureAIState.Inactive;
                    break;
            }

            if (nextState != _currentState)
            {
                _currentState = nextState;
                OnCreatureStateChanged?.Invoke(_currentState);
                Debug.Log($"[CreatureAI] State Transitioned -> {_currentState}");
            }
        }
    }
}
