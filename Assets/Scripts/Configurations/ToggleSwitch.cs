using System;
using Unity.Netcode;
using UnityEngine;

public class ToggleSwitch : Configuration
{
    public event Action<bool> OnToggle;

    [SerializeField] private GameObject Switch;
    [SerializeField] private MeshRenderer Light;

    bool b_CurrentPosition = false; //True = Up | False = Down;
    bool b_CorrectPosition = false;


    public void SetCorrectPosition(bool correctPosition)
    {
        b_CorrectPosition = correctPosition;
        OnPositionChange_Rpc(b_CurrentPosition);
    }


    [Rpc(SendTo.Everyone)]
    void OnPositionChange_Rpc(bool newPosition)
    {
        b_CurrentPosition = newPosition;
        bool IsCorrect = b_CorrectPosition == b_CurrentPosition ? true : false;

        Switch.transform.localEulerAngles = IsCorrect ? new Vector3(40,0,0) : new Vector3(-40,0,0);
        //Toggle State
        Light.materials[0].color = IsCorrect ? Color.green : Color.red;
        OnToggle?.Invoke(IsCorrect);
    }
}
