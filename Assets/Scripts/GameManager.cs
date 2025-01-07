using System;
using System.Collections;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public event Action OnStartGame;
    [SerializeField] NetworkManagerUI UIMenu;

    [SerializeField] private NetworkVariable<int> PlayerCount = new NetworkVariable<int>(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    NetworkVariable<float> GameTimeRemaining = new NetworkVariable<float>(
        value: 120.0f,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
        );


    private SO_GameSettings SelectedSettings;
    public override void OnNetworkSpawn()
    {

        //Server Client Updates
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += (id) =>
            {
                PlayerCount.Value++;
            };
            NetworkManager.Singleton.OnClientDisconnectCallback += (id) =>
            {
                PlayerCount.Value--;
            };
            PlayerCount.OnValueChanged += (int previousValue, int newValue) =>
            {
                if (UIMenu)
                {
                    UIMenu.SetPlayerCount(PlayerCount.Value);
                }
                Debug.Log($"Player count is {PlayerCount.Value}");
            };
        }

        //Get Arcade Units
        GameTimeRemaining.OnValueChanged += (float previousValue, float newValue) =>
        {
            //Call to Arcades to change UI Elements
        };

    }
    public void SetGameSettings(SO_GameSettings settings)
    {
        SelectedSettings = settings;
        //Call to the arcade unit to update looks - Scope Creep Here :Skull:
    }

    public void StartGame()
    {
        OnStartGame?.Invoke();
    }

    IEnumerator ChangeGameTimer()
    {
        while (GameTimeRemaining.Value > 0)
        {
            yield return new WaitForSecondsRealtime(1);
            GameTimeRemaining.Value--;
        }
    }
}
