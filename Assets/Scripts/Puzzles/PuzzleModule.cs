using System;
using Unity.Netcode;
using UnityEngine;

public class PuzzleModule : NetworkBehaviour, PuzzleInterface, IInteractable
{
    public event Action<int> OnPuzzleComplete;
    public event Action<int> OnPuzzleFail;
    public event Action OnPuzzleError;
    public event Action OnUIUpdated;


    [SerializeField] GameObject Puzzle_UI;
    protected int Attempts = 1;
    protected int CurrentAttempt = 0;
    [SerializeField] protected int AwardScore = 100;

    private void OnEnable()
    {
        CurrentAttempt = 0;
    }

    public virtual void StartPuzzleModule()
    {
        CurrentAttempt = 0;
    }

    public virtual void DeactivatePuzzleModule()
    {
        Puzzle_UI.SetActive(false);
    }


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
    }

    public virtual void ErrorPuzzle()
    {
        OnPuzzleError?.Invoke();
    }
    public virtual void CompleteModule()
    {
        OnPuzzleComplete?.Invoke(AwardScore);
    }

    public virtual void FailModule()
    {
        OnPuzzleFail?.Invoke(Mathf.FloorToInt(AwardScore/2));
    }

    public virtual bool OnClick()
    {
        if(!IsOwner) { return false; }
        return false;

    }

    public virtual bool OnDrag(Vector3 WorldPos)
    {
        if(!IsOwner) { return false; };
        return false;
    }

    protected void UpdateUIRender()
    {
        OnUIUpdated?.Invoke();
    }
}
