using Unity.Netcode;
using UnityEngine;

public class SwitchConfiguration : MonoBehaviour
{

    [SerializeField] protected ToggleSwitch[] Switches;
    [SerializeField] int ActiveSwitches;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Switches = GetComponentsInChildren<ToggleSwitch>();
        foreach (ToggleSwitch sw in Switches)
        {
            bool SwitchActive = Random.Range(0, 2) == 1 ? true : false;
            if (SwitchActive == false) { ActiveSwitches++; }

            sw.SetCorrectPosition(SwitchActive);
            sw.OnToggle += OnSwitchToggled;
        }
    }

    /*
     * If an active switch comes through checks will be completed to see if the configuration 
     * module is active, if the configuration completion is correct a delegate will be fired.
     */
    private void OnSwitchToggled(bool IsActive)
    {
        if (IsActive)
        {
            ActiveSwitches++;
            if (ActiveSwitches != Switches.Length) {return;}
            //TODO - Handle Configuration Completion

        }
        else
        {
            ActiveSwitches--;
        }
    }
}
