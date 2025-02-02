using System;
using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class UI_Game : UI_RenderTarget
{

    public event Action<int,bool> OnButtonPressRecieved; //Int is the Index of the Button, bool is if the button was performed / canceled

    protected UI_Score ScoreCounter;
    PuzzleModule[] Puzzles;
    GameObject CurrentPuzzle;
    public Camera GetUICamera()
    {
        return cam; 
    }
    private void Awake()
    {
        cam = GetComponent<Canvas>().worldCamera;
        ScoreCounter = GetComponentInChildren<UI_Score>();
        Puzzles = GetComponentsInChildren<PuzzleModule>();

        foreach (PuzzleModule puzzle in Puzzles) 
        { 
            puzzle.OnPuzzleComplete += Handle_PuzzleComplete;
            puzzle.OnPuzzleFail += Handle_PuzzleFail;
            puzzle.OnPuzzleError += Handle_PuzzleError;
            puzzle.OnUIUpdated += ForceNewRender;
        }

        cam.enabled = false;
        ToggleActiveRender(RenderAuto);
    }

    private void ForceNewRender()
    {
        cam.Render();
    }

    public override void OnNetworkSpawn()
    {
        ScoreCounter.ChangeScore_Rpc(0);
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
        CurrentPuzzle = Puzzles[IndexRevealed].gameObject;
        CurrentPuzzle.SetActive(true);
        Puzzles[IndexRevealed].StartPuzzleModule();
    }

    private void Handle_PuzzleComplete(int AwardedScore)
    {
        CurrentPuzzle.SetActive(false);
        ForceNewRender();
        StartCoroutine(NextPuzzleDelay());
        Debug.Log("Completed Puzzle");
    }
    private void Handle_PuzzleFail(int PunishmentScore)
    {
        Debug.Log("Failed Puzzle");
    }
    private void Handle_PuzzleError()
    {
        Debug.Log("Errored Puzzle");
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
        yield return new WaitForSeconds(8);
        StartNewPuzzle();
    }

}
