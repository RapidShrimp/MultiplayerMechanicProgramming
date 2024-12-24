using System;
using UnityEngine;

public class ArcadeUnit : MonoBehaviour
{
    private PuzzleModule[] Puzzles;
    private PuzzleModule ActiveModule = null;
    private Configuration[] Configurations;


    private int Health;
    private int MaxHealth = 100;

    private int Time;
    private int Score;

    private void Awake()
    {
        Configurations = GetComponentsInChildren<Configuration>();
        foreach (Configuration config in Configurations) 
        {
            config.OnConfigurationUpdated += Handle_ConfigurationUpdated;
            config.OnConfigurationSabotaged += Handle_ConfigurationSabotaged;
        }

        
    }


    void StartGame()
    {
        Score = 0;
        Health = MaxHealth;
        Time = 120; // Change this later

    }
    #region Configurations
    private void Handle_ConfigurationUpdated(bool IsActive)
    {
        Debug.Log($"Configuration Updated: {IsActive}");
    }
    private void Handle_ConfigurationSabotaged()
    {
        Score += 25;
    }

    #endregion
    #region Puzzles
    public void BindToPuzzles()
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
        Score += 150;

    }
    private void Handle_PuzzleFail(float PunishmentTime)
    {
        Debug.Log("Failed Puzzle");
        Score -= 25;
    }
    private void Handle_PuzzleError()
    {
        Debug.Log("Errored Puzzle");
        Score -= 10;
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
    #endregion
}
