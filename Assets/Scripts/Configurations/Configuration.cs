using System;
using Unity.Netcode;
using UnityEngine;

public class Configuration : MonoBehaviour
{
    public event Action<bool> OnConfigurationUpdated; //True - Working | False - Not Working
    public event Action OnConfigurationSabotaged; 

    bool IsCompleted;
}
