using System;
using UnityEngine;

public class ToggleSwitch : MonoBehaviour
{
    public event Action<bool> OnToggle;

    [SerializeField] private GameObject Switch;
    [SerializeField] private MeshRenderer Light;

    bool b_SwitchOn;

    private void Start()
    {
        SetSwitchState(false);
    }

    void SetSwitchState(bool state) { 
        b_SwitchOn = state; 
        OnToggle?.Invoke(state);


        Light.materials[0].color = state ? Color.green: Color.red;
    }



}
