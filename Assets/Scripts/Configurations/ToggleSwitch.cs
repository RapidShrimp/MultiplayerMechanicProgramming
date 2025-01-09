using System;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class ToggleSwitch : Configuration, IInteractable
{
    public event Action<bool> OnToggle;

    [SerializeField] private GameObject Switch;
    [SerializeField] private MeshRenderer Light;

    //True = Up | False = Down;
    NetworkVariable<bool> b_CurrentPosition = new NetworkVariable<bool>(
        value: false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner);
    
    NetworkVariable<bool> b_CorrectPosition = new NetworkVariable<bool>(
        value: false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner);


    public void SetCorrectPosition(bool correctPosition)
    {
        if (!IsOwner) { return; }
        b_CorrectPosition.Value = correctPosition;
        OnPositionChange_Rpc(b_CurrentPosition.Value);
    }


    [Rpc(SendTo.Everyone)]
    void OnPositionChange_Rpc(bool newPosition)
    {
        if (IsOwner)
        {
            b_CurrentPosition.Value = newPosition;
            Debug.Log(b_CurrentPosition.Value);
        }
        bool IsCorrect = b_CorrectPosition == b_CurrentPosition ? true : false;

        Switch.transform.localEulerAngles = IsCorrect ? new Vector3(40,0,0) : new Vector3(-40,0,0);
        //Toggle State
        Light.materials[0].color = IsCorrect ? Color.green : Color.red;
        OnToggle?.Invoke(IsCorrect);
    }

    public bool OnClick()
    {
        OnPositionChange_Rpc(!b_CurrentPosition.Value);
        return true;
    }

    public void OnDrag()
    {
        
    }
}
