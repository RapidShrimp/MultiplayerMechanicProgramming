using System;
using Unity.Netcode;
using UnityEngine;

public class StarCollector : PuzzleModule
{

    protected StarPlayer player;

    NetworkVariable<int> Stars = new NetworkVariable<int>(
        value: 0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner);

    NetworkVariable<int> RequiredStars = new NetworkVariable<int>(
    value: 0,
    NetworkVariableReadPermission.Everyone,
    NetworkVariableWritePermission.Owner);


    private void OnEnable()
    {
        player = GetComponentInChildren<StarPlayer>();
        Stars.OnValueChanged += OnStarCollected;
        RequiredStars.OnValueChanged += OnRequiredStarsChanged;
        
    }

    private void OnDisable()
    {
        Stars.OnValueChanged -= OnStarCollected;
        RequiredStars.OnValueChanged -= OnRequiredStarsChanged;
    }

    private void OnRequiredStarsChanged(int previousValue, int newValue)
    {
    }

    private void OnStarCollected(int previousValue, int newValue)
    {
    }

    public override void StartPuzzleModule()
    {
        base.StartPuzzleModule();


    }
    public override void OnMoveInput(Vector2 Direction, bool Performed)
    {
        if (!isActiveAndEnabled) { return; }
        player.Handle_PlayerMove(Direction, Performed);
    }


}
