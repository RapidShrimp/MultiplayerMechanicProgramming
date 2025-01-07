using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(NetworkObject))]
public class PlayerManager : NetworkBehaviour
{
    

    public event Action<bool> OnStartConditionUpdated;
    private TextMeshProUGUI PlayerCountUI;

    private void Awake()
    {
        PlayerCountUI = GetComponentInChildren<TextMeshProUGUI>();
    }

    public override void OnNetworkSpawn()
    {
        
    }
}