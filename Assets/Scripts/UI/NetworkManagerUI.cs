using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using System;

public class NetworkManagerUI : NetworkBehaviour
{
    public event Action OnGameStarted;
    [SerializeField] private UI_MainMenu MainMenuUI;
    [SerializeField] private UI_LobbyMenu LobbyMenuUI;
    private int PlayersActive;

    private void Awake()
    {
        PlayerManager m_PlayerManager = FindAnyObjectByType<PlayerManager>();
        m_PlayerManager.OnStartConditionUpdated += Handle_StartConditionUpdated;

        LobbyMenuUI.OnStartGame += () => OnGameStarted?.Invoke();
    }

    private void Handle_StartGame()
    {
        
    }

    private void Handle_StartConditionUpdated(bool Startable)
    {
        throw new NotImplementedException();
    }

    public void SetPlayerCount(int Players)
    {
        PlayersActive = Players;
        if (Players >= 2) 
        {   
            LobbyMenuUI.SetCanStartGame(true);
        }
        else
        {
            LobbyMenuUI.SetCanStartGame(false);

        }
    }

    
}
