using UnityEngine;
using Unity.Netcode;
using System;
using UnityEngine.SceneManagement;

public class NetworkManagerUI : NetworkBehaviour
{
    [SerializeField] private UI_MainMenu MainMenuUI;
    [SerializeField] private UI_LobbyMenu LobbyMenuUI;

    private int PlayersActive;

    private void Awake()
    {
        if (!MainMenuUI) { return; }
        if (!LobbyMenuUI) { return; }


        LobbyMenuUI.OnStartGame += () => 
        { 
            SceneManager.LoadScene(1);
        };

    }
    public void SetPlayerCount(int Players)
    {
        PlayersActive = Players;
        if (LobbyMenuUI != null) 
        {
            LobbyMenuUI.UpdatePlayerCount(Players);
        }
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
