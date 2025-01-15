using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class SliderConfiguration : Configuration, IInteractable
{
    [SerializeField] NetworkVariable<int> DesiredXPos= new NetworkVariable<int>(
        value:0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner);
    
    [SerializeField] NetworkVariable<int> SliderXPos = new NetworkVariable<int>(
     value: 0,
     NetworkVariableReadPermission.Everyone,
     NetworkVariableWritePermission.Owner);

    int SliderMax = 10;
    [SerializeField] GameObject SliderMesh;
    [SerializeField] GameObject SliderDesired;

    public override void OnNetworkSpawn()
    {
        DesiredXPos.OnValueChanged += SetCorrectSliderPos_Rpc;
    }
    public override void OnNetworkDespawn()
    {
        DesiredXPos.OnValueChanged -= SetCorrectSliderPos_Rpc;
    }

    override public void StartModule()
    {
        if (IsOwner)
        {
            DesiredXPos.Value = Random.Range(0, SliderMax + 1);
        }
    }


    [Rpc(SendTo.Owner)]
    private void SetCorrectSliderPos_Rpc(int OldPosition ,int NewPosition)
    {
        DesiredXPos.Value = NewPosition;
        
    }

    [Rpc(SendTo.Owner)]
    private void ChangeSliderPosition_Rpc(int NewPosition)
    {

    }

    public bool OnClick()
    {
        return false;
    }

    public void OnDrag()
    {
        Debug.Log("Drag Logic");
        //Do Something :)
    }
}
