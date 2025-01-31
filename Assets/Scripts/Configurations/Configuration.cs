using System;
using Unity.Netcode;
using UnityEngine;

public class Configuration : NetworkBehaviour
{
    public event Action<bool> OnConfigurationUpdated; //True - Working | False - Not Working
    public event Action<int> OnConfigurationSabotaged;
    [SerializeField] int SabotageValue = 30;
    public NetworkVariable<bool> IsCompleted = new NetworkVariable<bool>(
        value:false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner);

    virtual public void StartModule() { }

    [Rpc(SendTo.Owner)]
    protected void Sabotage_Rpc() 
    {
        Debug.Log("Sabotaged");
        OnConfigurationSabotaged?.Invoke(SabotageValue); 
    }
}
