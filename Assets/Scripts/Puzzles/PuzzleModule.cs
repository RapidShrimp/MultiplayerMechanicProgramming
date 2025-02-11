using System;
using Unity.IO.LowLevel.Unsafe;
using Unity.Netcode;
using UnityEngine;

public class PuzzleModule : NetworkBehaviour, PuzzleInterface, IInteractable
{
    public event Action<int> OnPuzzleComplete;
    public event Action<int> OnPuzzleFail;
    public event Action OnPuzzleError;
    public event Action OnUIUpdated;

    protected SFX_Item PuzzleAudios;

    protected int Attempts = 1;
    protected int CurrentAttempt = 0;
    [SerializeField] protected int AwardScore = 100;

    private void OnEnable()
    {
        CurrentAttempt = 0;
        PuzzleAudios = GetComponent<SFX_Item>();
    }

    virtual public void RequestUIChanges() { /*Override*/ }


    public virtual void StartPuzzleModule()
    {
        CurrentAttempt = 0;
    }

    public virtual void DeactivatePuzzleModule()
    {
        //StopAllCoroutines();
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
        OnPuzzleFail?.Invoke(Mathf.FloorToInt(AwardScore / 2));
    }

    public virtual bool OnClick()
    {
        if (!IsOwner) { return false; }
        return false;

    }

    public virtual bool OnDrag(Vector3 WorldPos, bool IsHeld)
    {
        if (!IsOwner) { return false; };
        return false;
    }

    public virtual void OnMoveInput(Vector2 Direction, bool Performed)
    {
        //Override Here
    }

    protected virtual void UpdateUIRender()
    {
        OnUIUpdated?.Invoke();
    }

    public void OnHovver(bool Hovered)
    {
        throw new NotImplementedException();
    }
}
