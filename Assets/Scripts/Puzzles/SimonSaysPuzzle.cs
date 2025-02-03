using System;
using Unity.Netcode;
using UnityEngine;


public class SimonSaysPuzzle : PuzzleModule
{

    int[] CorrectSequence;
    int[] Curr_Sequence;


    private void Awake()
    {
        GetComponentInParent<UI_Game>().OnButtonPressRecieved += OnButtonPressed;
    }


    public override void StartPuzzleModule()
    {
        base.StartPuzzleModule();
        if (!IsOwner) { return; }
        int[] Sequence = new int[5];
        for (int i = 0; i < Sequence.Length; i++)
        {
            Sequence[i] = UnityEngine.Random.Range(0, 5);
        }
        OnCorrectSequenceUpdated_Rpc(Sequence);
    }

    [Rpc(SendTo.Everyone)]
    public void OnCorrectSequenceUpdated_Rpc(int[] Array)
    {
        CorrectSequence = Array;
        Curr_Sequence = new int[5]; 
    }

    private void OnButtonPressed(int ButtonIndex, bool Performed)
    {
        if(!isActiveAndEnabled) { return; }
        if(Curr_Sequence.Length == 5 && CorrectSequence == Curr_Sequence) { CompleteModule(); }
    }

    public override void RequestUIChanges()
    {
        //Do UI Updates Here
    }


    
}