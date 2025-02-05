using System;
using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class UI_Game : UI_RenderTarget
{

    public event Action<int,bool> OnButtonPressRecieved; //Int is the Index of the Button, bool is if the button was performed / canceled
    public event Action<Vector2, bool> OnJoystickMovement;
    public event Action<int> OnScoreUpdated;

    [SerializeField] TextMeshProUGUI DisplayText;
    protected UI_Score ScoreCounter;
    protected PuzzleModule[] Puzzles;
    protected GameObject CurrentPuzzle;
    protected UI_Background Background;
    public UI_PlayerIdentifier PlayerIdentifier;
    

    public bool ConfigurationSet = false;
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
            puzzle.OnPuzzleComplete += Handle_PuzzleComplete_Rpc;
            puzzle.OnPuzzleFail += Handle_PuzzleFail;
            puzzle.OnPuzzleError += Handle_PuzzleError_Rpc;
            puzzle.OnUIUpdated += ForceNewRender;
            puzzle.gameObject.SetActive(false);
        }
    }

    public override void OnNetworkDespawn()
    {
    }


    public void StartNewPuzzle()
    {
        StartNewPuzzle_Rpc(UnityEngine.Random.Range(0, Puzzles.Length));
    }

    [Rpc(SendTo.Everyone)]
    public void StartNewPuzzle_Rpc(int IndexRevealed)
    {
        DisplayText.gameObject.SetActive(false);
        CurrentPuzzle = Puzzles[IndexRevealed].gameObject;
        CurrentPuzzle.SetActive(true);
        Puzzles[IndexRevealed].StartPuzzleModule();
    }

    [Rpc(SendTo.Everyone)]
    private void Handle_PuzzleComplete_Rpc(int AwardedScore)
    {
        CurrentPuzzle.SetActive(false);
        ForceNewRender();
        StartCoroutine(NextPuzzleDelay());
        Background.PuzzleComplete();
        OnScoreUpdated?.Invoke(AwardedScore);
    }
    private void Handle_PuzzleFail(int PunishmentScore)
    {
        OnScoreUpdated?.Invoke(-PunishmentScore);
    }
    [Rpc(SendTo.Everyone)]
    private void Handle_PuzzleError_Rpc()
    {
        OnScoreUpdated?.Invoke(-5);
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

    public void OnGameEnded(bool IsWinner, int WinnerIndex)
    {
        CurrentPuzzle.GetComponent<PuzzleModule>().DeactivatePuzzleModule();
        CurrentPuzzle.gameObject.SetActive(false);
        StopAllCoroutines();
        DisplayText.gameObject.SetActive(true);
        if (IsWinner)
        {
            DisplayText.text = $"Congrats Player {WinnerIndex+1}!\nYOU WIN!";
        }
        else
        {
            DisplayText.text = $"Player {WinnerIndex+1} is the Winner";
        }
    }
}
