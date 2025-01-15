using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class DialConfiguration : Configuration, IInteractable
{
    [SerializeField] NetworkVariable<int> DialPosition = new NetworkVariable<int>(
        value:0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner);
    
    NetworkVariable<int> CorrectPosition = new NetworkVariable<int>(
        value: 0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner);

    [SerializeField] GameObject DialMesh;

    public override void OnNetworkSpawn()
    {
        DialPosition.OnValueChanged += OnDialTurned;
        CorrectPosition.OnValueChanged += OnDialTurned;
    }
    public override void OnNetworkDespawn()
    {
        DialPosition.OnValueChanged -= OnDialTurned;
        CorrectPosition.OnValueChanged -= OnDialTurned;
    }
    override public void StartModule()
    {
        if (IsOwner)
        {
            ChangeDialPosition_Rpc(Random.Range(0, 4));
        }
    }


    [Rpc(SendTo.Owner)]
    private void SetCorrectDialPosition_Rpc(int NewPosition)
    {
        Mathf.Clamp(NewPosition, 0, 3);
        CorrectPosition.Value = NewPosition;
    }

    [Rpc(SendTo.Owner)]
    private void ChangeDialPosition_Rpc(int NewPosition)
    {
        DialPosition.Value = NewPosition;
        if (DialPosition.Value == CorrectPosition.Value) 
        {
            IsCompleted.Value = true;
            return;
        }
        IsCompleted.Value = false;
    }

    private void OnDialTurned(int OldValue, int NewValue)
    {
        if(!IsOwner) { return;}
        DialMesh.transform.rotation = Quaternion.Euler(0, 0, 90 * NewValue);
    }

    public bool OnClick()
    {
        int DialIncrement;
        if (IsOwner)
        {
            DialIncrement = DialPosition.Value == 3 ? 0 : DialPosition.Value+1;
            ChangeDialPosition_Rpc(DialIncrement);
        }
        else
        {
            do {DialIncrement = Random.Range(0, 4);} 
            while (DialIncrement != DialPosition.Value);
            ChangeDialPosition_Rpc(DialIncrement);
        }
        return true;
    }

    public void OnDrag()
    {
        //Do Nothing :)
    }
}
