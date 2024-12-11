using System;
using UnityEngine;

public class PuzzleModule : MonoBehaviour, PuzzleInterface
{

    public event Action<float> OnPuzzleComplete;
    public event Action OnPuzzleError;
    public event Action<float> OnPuzzleFail;

    protected int Attempts = 1;
    protected int AttemptCount = 0;

    protected float AwardTime = 5.0f;
    protected float FailTime = 3.0f;

    public void StartPuzzleModule()
    {
        throw new NotImplementedException();
    }
    public void DeactivatePuzzleModule()
    {
        throw new NotImplementedException();
    }

    public void CompleteModule()
    {
        throw new NotImplementedException();
    }

    public void FailModule()
    {
        throw new NotImplementedException();
    }

}
