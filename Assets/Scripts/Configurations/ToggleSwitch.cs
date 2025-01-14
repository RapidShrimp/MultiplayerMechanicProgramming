using System;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class ToggleSwitch : NetworkBehaviour , IInteractable
{
    public event Action<bool> OnToggle;

    [SerializeField] private GameObject Switch;
    [SerializeField] private MeshRenderer Light;

    //True = Up | False = Down;
    [SerializeField] private NetworkVariable<bool> b_CurrentPosition = new NetworkVariable<bool>(
        value: false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner);

    [SerializeField]
    private NetworkVariable<bool> b_CorrectPosition = new NetworkVariable<bool>(
        value: false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner);

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        b_CurrentPosition.OnValueChanged += OnPositionChange;
        b_CorrectPosition.OnValueChanged += OnPositionChange;
        OnPositionChange(false,false);

    }
    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        b_CurrentPosition.OnValueChanged -= OnPositionChange;
        b_CorrectPosition.OnValueChanged -= OnPositionChange;
    }
    public void SetCorrectPosition(bool correctPosition)
    {
        if (!IsOwner) { return; }
        b_CorrectPosition.Value = correctPosition;
    }

    void OnPositionChange(bool old, bool newval)
    {
        //Toggle State
        bool IsCorrect = b_CorrectPosition.Value == b_CurrentPosition.Value ? true : false;
        Light.materials[0].color = IsCorrect ? Color.green : Color.red;
        Switch.transform.localEulerAngles = IsCorrect ? new Vector3(40,0,0) : new Vector3(-40,0,0);
        OnToggle?.Invoke(IsCorrect);
    }

    public bool OnClick()
    {
        if (!IsOwner) { return false; }
        b_CurrentPosition.Value = !b_CurrentPosition.Value;
        return true;
    }

    public void OnDrag()
    {
        
    }
}
