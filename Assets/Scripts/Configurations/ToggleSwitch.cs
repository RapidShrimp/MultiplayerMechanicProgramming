using System;
using UnityEngine;

public class ToggleSwitch : MonoBehaviour
{
    public event Action<bool> OnToggle;

    [SerializeField] private GameObject Switch;
    [SerializeField] private MeshRenderer Light;

    bool b_CurrentPosition = false; //True = Up | False = Down;
    bool b_CorrectPosition = false;


    public void SetCorrectPosition(bool correctPosition)
    {
        b_CorrectPosition = correctPosition;
        OnPositionChange(b_CurrentPosition);
    }

    void OnPositionChange(bool newPosition)
    {
        b_CurrentPosition = newPosition;
        bool IsCorrect = b_CorrectPosition == b_CurrentPosition ? true : false;

        //Toggle State
        Light.materials[0].color = IsCorrect ? Color.green : Color.red;
        OnToggle?.Invoke(IsCorrect);
    }
}
