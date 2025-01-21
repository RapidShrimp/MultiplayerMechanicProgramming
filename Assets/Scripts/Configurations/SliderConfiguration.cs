using System.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
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

    [SerializeField] int SliderMax = 9;
    [SerializeField] float Step = 0.025f;
    [SerializeField] GameObject SliderMesh;
    [SerializeField] GameObject CorrectLocMesh;
    MeshRenderer CorrectLocRenderer;
    float DistanceTolerance = 0.1f;
    public override void OnNetworkSpawn()
    {
        DesiredXPos.OnValueChanged += SetCorrectSliderPos_Rpc;
        SliderXPos.OnValueChanged += OnSliderChange;
        CorrectLocRenderer = CorrectLocMesh.GetComponentInChildren<MeshRenderer>();

    }
    public override void OnNetworkDespawn()
    {
        DesiredXPos.OnValueChanged -= SetCorrectSliderPos_Rpc;
        SliderXPos.OnValueChanged -= OnSliderChange;

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
        CorrectLocMesh.transform.localPosition = new Vector3(NewPosition * Step, 0);
        ChangeSliderPosition_Rpc(SliderXPos.Value);
    }

    [Rpc(SendTo.Everyone)]
    private void ChangeSliderPosition_Rpc(int NewPosition)
    {

        CorrectLocRenderer.materials[0].color = NewPosition != DesiredXPos.Value ? Color.red : Color.green;

        if (IsOwner)
        {
            SliderXPos.Value = NewPosition;
        }
        
    }

    private void OnSliderChange(int OldPosition ,int NewPosition)
    {
        if(!IsOwner) {return;}
        SliderMesh.transform.localPosition = new Vector3(NewPosition*Step,0) ;
        if(NewPosition == DesiredXPos.Value)
        {
        }
    }

    public bool OnClick()
    {
        return false;
    }

    public bool OnDrag(Vector3 WorldPos)
    {
        float XDiff = (SliderMesh.transform.position - WorldPos).x;
        if(Mathf.Abs(XDiff) < DistanceTolerance)
        {
            return true;
        }
        
        int SliderMove = XDiff < 0 ? 1 : -1;
        ChangeSliderPosition_Rpc(Mathf.Clamp(SliderXPos.Value + SliderMove, 0, SliderMax));
        //Do Something :)
        return true;
    }
}
