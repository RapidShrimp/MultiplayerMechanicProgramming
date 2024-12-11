using System;
using UnityEngine;

public interface PuzzleInterface
{
    public void StartPuzzleModule();
    public void CompleteModule();
    public void FailModule();
    public void DeactivatePuzzleModule();
}
