using System;
using UnityEngine;

namespace Echo.Gameplay.Puzzles
{
    public abstract class BasePuzzle : MonoBehaviour, IPuzzleInteraction
    {
        [SerializeField] private string _puzzleId;
        [SerializeField] protected string CorrectSolution;

        public string PuzzleId => _puzzleId;
        public bool IsSolved { get; protected set; }

        public event Action<string, bool> OnPuzzleStateChanged;

        public virtual void Interact(int playerIndex, string inputCode)
        {
            if (IsSolved) return;

            if (inputCode == CorrectSolution)
            {
                IsSolved = true;
                OnPuzzleStateChanged?.Invoke(_puzzleId, true);
                OnSolveSuccess();
            }
            else
            {
                OnSolveFailure(playerIndex);
            }
        }

        public virtual void ResetPuzzle()
        {
            IsSolved = false;
            OnPuzzleStateChanged?.Invoke(_puzzleId, false);
        }

        protected virtual void OnSolveSuccess()
        {
            Debug.Log($"[Puzzle] Puzzle '{_puzzleId}' SOLVED successfully!");
        }

        protected virtual void OnSolveFailure(int playerIndex)
        {
            Debug.LogWarning($"[Puzzle] Player {playerIndex} failed solution attempt for '{_puzzleId}'.");
        }
    }
}
