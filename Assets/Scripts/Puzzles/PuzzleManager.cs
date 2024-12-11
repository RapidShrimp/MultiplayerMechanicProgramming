using System;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    [SerializeField] private PuzzleModule[] Puzzles;
    private PuzzleModule ActiveModule = null;
    
    private void Start()
    {
        InitPuzzleManager();
    }

    public void InitPuzzleManager()
    {
        foreach (PuzzleModule Module in Puzzles)
        {
            Module.OnPuzzleComplete += Handle_PuzzleComplete;
            Module.OnPuzzleFail += Handle_PuzzleFail;
            Module.OnPuzzleError += Handle_PuzzleError;
        }
    }

    private void Handle_PuzzleComplete(float AwardedTime)
    {
        Debug.Log("Completed Puzzle");

    }
    private void Handle_PuzzleFail(float PunishmentTime)
    {
        Debug.Log("Failed Puzzle");
    }
    private void Handle_PuzzleError()
    {
        Debug.Log("Errored Puzzle");
    }
    public void StartNewPuzzle()
    {
        if (ActiveModule != null) 
        {
            ActiveModule.DeactivatePuzzleModule();
        }
        
        ActiveModule = Puzzles[UnityEngine.Random.Range(0, Puzzles.Length)];
        if (ActiveModule == null) { Debug.LogError("Couldnt Find Puzzle"); }

        ActiveModule.StartPuzzleModule();
    }
}
