using System;
using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    [SerializeField] private NetworkVariable<int> PlayerCount = new NetworkVariable<int>(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    NetworkVariable<float> GameTimeRemaining = new NetworkVariable<float>(
        value: 120.0f,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
        );

    [SerializeField] NetworkManagerUI UIMenu;
    private SO_GameSettings SelectedSettings;
    protected ArcadeUnit[] Units;
    public override void OnNetworkSpawn()
    {
        //Bind UI Elements
        if(UIMenu == null)
        {
            throw new Exception("Couldnt Find Network UI Script");
        }

        UIMenu.OnGameStarted += () => StartGame();

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
                Debug.Log($"Player count is {PlayerCount.Value}");
                UIMenu.SetPlayerCount(newValue);
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
        foreach(ArcadeUnit unit in Units)
        {
            unit.StartGame(SelectedSettings);
        }
        GameTimeRemaining.Value = SelectedSettings.GameTime;
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
