using System;
using UnityEngine;

public interface PuzzleInterface
{

    public event Action<int> OnPuzzleComplete;
    public event Action OnPuzzleError;
    public event Action<int> OnPuzzleFail;

    public void StartPuzzleModule();
    public void CompleteModule();
    public void FailModule();
    public void ErrorPuzzle();
    public void DeactivatePuzzleModule();
}
