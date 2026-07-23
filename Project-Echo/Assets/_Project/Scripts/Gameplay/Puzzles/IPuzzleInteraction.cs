using System;

namespace Echo.Gameplay.Puzzles
{
    public interface IPuzzleInteraction
    {
        string PuzzleId { get; }
        bool IsSolved { get; }

        event Action<string, bool> OnPuzzleStateChanged;

        void Interact(int playerIndex, string inputCode);
        void ResetPuzzle();
    }
}
