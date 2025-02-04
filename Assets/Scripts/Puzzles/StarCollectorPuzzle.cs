using System;
using Unity.Netcode;
using UnityEngine;

public class StarCollector : PuzzleModule
{

    StarPlayer player;

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

    

}
