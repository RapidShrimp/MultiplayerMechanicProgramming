using System;
using Unity.Netcode;
using UnityEngine;

public class Configuration : NetworkBehaviour
{
    public event Action<bool> OnConfigurationUpdated; //True - Working | False - Not Working
    public event Action OnConfigurationSabotaged;

    public NetworkVariable<bool> IsCompleted = new NetworkVariable<bool>(
        value:false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner);
    virtual public void StartModule() { }
}
