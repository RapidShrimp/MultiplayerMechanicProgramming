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


    GameObject StarsContainer = null;
    private void Awake()
    {
        player = GetComponentInChildren<StarPlayer>();
        player.OnStarCollected += OnStarCollected;
    }

    private void OnDisable()
    {
    }

    private void OnStarCollected()
    {
        if (!IsOwner) { return; }
        Stars.Value--;
        Debug.Log($"Stars {Stars.Value}");
        if (Stars.Value == 0) { CompleteModule(); }
    }

    public override void StartPuzzleModule()
    {
        base.StartPuzzleModule();
        StarsContainer = transform.GetChild(1).gameObject;
        if (IsOwner)
        {
            int StarsToFind = 0;
            for (int i = 0; i < StarsContainer.transform.childCount; i++)
            {
                bool isActive = UnityEngine.Random.Range(0, 2) == 1;
                SetChildState_Rpc(i, isActive);
                if( isActive ) {StarsToFind++;}
                
            }
            Stars.Value = StarsToFind;
            Debug.Log($"Stars {Stars.Value}");
        }
    }

    [Rpc(SendTo.Everyone)]
    protected void SetChildState_Rpc(int Index, bool Active)
    {
        StarsContainer.transform.GetChild(Index).gameObject.SetActive(Active);
    }

    public override void OnMoveInput(Vector2 Direction, bool Performed)
    {
        if (!isActiveAndEnabled) { return; }
        player.Handle_PlayerMove(Direction, Performed);
    }


}
