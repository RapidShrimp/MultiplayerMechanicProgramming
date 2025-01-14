using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class ArcadeUnit : NetworkBehaviour
{
    private PuzzleModule[] Puzzles;
    private PuzzleModule ActiveModule = null;
    [SerializeField] private Configuration[] Configurations;

    

    private int Health;
    private int MaxHealth = 100;

    private int Time;
    private int Score;


    public override void OnNetworkSpawn()
    {
        Configurations = GetComponentsInChildren<Configuration>();
        foreach (Configuration config in Configurations) 
        {
            config.OnConfigurationUpdated += Handle_ConfigurationUpdated;
            config.OnConfigurationSabotaged += Handle_ConfigurationSabotaged;
        }
    }

    public void StartGame(SO_GameSettings Settings)
    {
        Score = 0;
        MaxHealth = 100;// Settings.DefaultHealth;
        Health = MaxHealth;
        Time = 120; // Settings.GameTime; // Change this later
        if(!IsOwner) {return;}
        foreach (Configuration config in Configurations)
        {
            //config.StartModule();
        }
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
