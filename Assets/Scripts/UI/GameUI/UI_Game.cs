using System;
using System.Collections;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class UI_Game : UI_RenderTarget
{

    public event Action<int,bool> OnButtonPressRecieved; //Int is the Index of the Button, bool is if the button was performed / canceled
    public event Action<Vector2, bool> OnJoystickMovement;
    public event Action<int> OnScoreUpdated;

    [SerializeField] TextMeshProUGUI DisplayText;
    protected UI_Score ScoreCounter;
    protected PuzzleModule[] Puzzles;
    protected PuzzleModule CurrentPuzzle;
    protected UI_Background Background;
    protected SFX_Item Audios;
    public UI_PlayerIdentifier PlayerIdentifier;
    
    protected bool ConfigurationSet = false;
    [Rpc(SendTo.Everyone)]
    public void SetConfigurationCompletion_Rpc( bool IsSet) 
    {
        Debug.Log("Configuration Set");
        ConfigurationSet = IsSet; }
    
    public Camera GetUICamera()
    {
        return cam; 
    }

    private void Awake()
    {
        cam = GetComponent<Canvas>().worldCamera;
        ScoreCounter = GetComponentInChildren<UI_Score>();
        Puzzles = GetComponentsInChildren<PuzzleModule>();
        Background = GetComponentInChildren<UI_Background>();
        Audios = GetComponentInChildren<SFX_Item>();
        PlayerIdentifier = GetComponentInChildren<UI_PlayerIdentifier>();
        cam.enabled = false;
        ToggleActiveRender(false);
    }

    public override void ToggleActiveRender(bool Active)
    {
        base.ToggleActiveRender(Active);
        if (CurrentPuzzle != null)
        {
            CurrentPuzzle.GetComponent<PuzzleModule>().RequestUIChanges();
        }
        ForceNewRender();
    }

    public void ForceNewRender()
    {
        if(!cam.isActiveAndEnabled) { return; }
        cam.Render();
    }

    public override void OnNetworkSpawn()
    {
        ScoreCounter.ChangeScore_Rpc(0);
        foreach (PuzzleModule puzzle in Puzzles)
        {
            BindToPuzzle(puzzle);
            NetworkTransform[] NetMovement = puzzle.GetComponentsInChildren<NetworkTransform>();
            foreach(NetworkTransform netMovement in NetMovement) { netMovement.enabled = false; }
            puzzle.gameObject.SetActive(false);
        }
    }

    public void BindToPuzzle(PuzzleModule puzzle)
    {
        puzzle.OnPuzzleComplete += Handle_PuzzleComplete_Rpc;
        puzzle.OnPuzzleFail += Handle_PuzzleFail;
        puzzle.OnPuzzleError += Handle_PuzzleError_Rpc;
        puzzle.OnUIUpdated += ForceNewRender;
    }

    protected void SetActivePuzzle(PuzzleModule puzzle)
    {
        if(CurrentPuzzle != null)
        {
            SetPuzzleInactive(CurrentPuzzle);
        }

        CurrentPuzzle = puzzle;

        //Enable Network Transforms if there are any (Future proofing this project)
        NetworkTransform[] netMovements = CurrentPuzzle.GetComponentsInChildren<NetworkTransform>();
        foreach (NetworkTransform netMovement in netMovements) { netMovement.enabled = true; }
        CurrentPuzzle.gameObject.SetActive(true);
        CurrentPuzzle.StartPuzzleModule();

    }

    public void SetPuzzleInactive(PuzzleModule puzzle)
    {

        //Unity likes to cry if a network transform is enabled in an inactive object (Mega Sad)
        NetworkTransform[] netMovements = CurrentPuzzle.GetComponentsInChildren<NetworkTransform>();
        foreach (NetworkTransform netMovement in netMovements) { netMovement.enabled = false; }
        puzzle.gameObject.SetActive(false);
        puzzle.DeactivatePuzzleModule();

    }

    public void StartNewPuzzle()
    {
        if (!IsOwner) { return; }
        StartNewPuzzle_Rpc(UnityEngine.Random.Range(0, Puzzles.Length));
    }

    [Rpc(SendTo.Everyone)]
    public void StartNewPuzzle_Rpc(int IndexRevealed)
    {
        DisplayText.gameObject.SetActive(false);
        SetActivePuzzle(Puzzles[IndexRevealed]);
    }

    [Rpc(SendTo.Everyone)]
    private void Handle_PuzzleComplete_Rpc(int AwardedScore)
    {
        SetPuzzleInactive(CurrentPuzzle);
        ForceNewRender();
        Background.PuzzleComplete();
        Audios.PlaySFX(0, "Complete");
        OnScoreUpdated?.Invoke(AwardedScore);
        StartCoroutine(NextPuzzleDelay());
    }
    private void Handle_PuzzleFail(int PunishmentScore)
    {
        OnScoreUpdated?.Invoke(-PunishmentScore);
    }
    [Rpc(SendTo.Everyone)]
    private void Handle_PuzzleError_Rpc()
    {
        OnScoreUpdated?.Invoke(-5);
        Audios.PlaySFX(0, "Incorrect");
        Background.PuzzleErrored();
    }

    public void ButtonPressed(int ButtonIndex ,bool Performed) 
    { 
        OnButtonPressRecieved?.Invoke(ButtonIndex, Performed);
    }

    public void UpdateScore(int NewValue)
    {
        ScoreCounter.ChangeScore_Rpc(NewValue);
        ForceNewRender();
    }

    IEnumerator NextPuzzleDelay()
    {
        DisplayText.gameObject.SetActive(true);
        DisplayText.text = "Waiting for New Puzzle";
        ForceNewRender();
        yield return new WaitForSeconds(8);
        while (!ConfigurationSet)
        {
            DisplayText.text = "Must Complete all Configurations to recieve new puzzle";
            yield return new WaitForFixedUpdate();
        }
        DisplayText.gameObject.SetActive(false);
        StartNewPuzzle();
    }

    public void OnGameReady()
    {
        StartCoroutine(StartGameUI());
    }
    public void OnGameEnded(bool IsWinner, int WinnerIndex)
    {
        SetPuzzleInactive(CurrentPuzzle);
        CurrentPuzzle.GetComponent<PuzzleModule>().DeactivatePuzzleModule();
        
        StopAllCoroutines();
        DisplayText.gameObject.SetActive(true);
        if (IsWinner)
        {
            DisplayText.text = $"Congrats Player {WinnerIndex+1}!\nYOU WIN!";
            SFX_AudioManager.Singleton.SwapToMusic(Audios.FindAudioByName("Victory"),0.1f,0.5f);
        }
        else
        {
            DisplayText.text = $"Player {WinnerIndex+1} is the Winner";
            SFX_AudioManager.Singleton.SwapToMusic(Audios.FindAudioByName("GameOver"), 0.1f, 0.5f);

        }
    }

    public void MoveUI(Vector2 MoveDir, bool Performed)
    {
        if (!isActiveAndEnabled || CurrentPuzzle == null || !CurrentPuzzle.gameObject.activeInHierarchy) { return; }
        CurrentPuzzle.OnMoveInput(MoveDir, Performed);
    }

    IEnumerator StartGameUI()
    {
        SFX_AudioManager.Singleton.PlaySoundToPlayer(Audios.FindAudioByName("StartCountdown"));
        DisplayText.text = "Ready";
        yield return new WaitForSeconds(1);
        DisplayText.text = "Set";
        yield return new WaitForSeconds(1);
        DisplayText.text = "GO!";

    }
}
