using UnityEngine;
using Unity.Netcode;
using System;
using UnityEngine.SceneManagement;

public class NetworkManagerUI : NetworkBehaviour
{

    public event Action OnStartGame;
    public event Action<GameSettings> OnUpdateGameSettings;
    private UI_MainMenu MainMenuUI;
    private UI_LobbyMenu LobbyMenuUI;
    private UI_SettingsMenu SettingsMenuUI;
    private int PlayersActive;

    private void Awake()
    {
        MainMenuUI = GetComponentInChildren<UI_MainMenu>();
        LobbyMenuUI = GetComponentInChildren<UI_LobbyMenu>();
        SettingsMenuUI = GetComponentInChildren<UI_SettingsMenu>();
        if (!MainMenuUI) { return; }
        if (!LobbyMenuUI) { return; }

        SettingsMenuUI.OnSettingsChanged += (Settings) => { OnUpdateGameSettings?.Invoke(Settings); };
        SettingsMenuUI.gameObject.SetActive(false);

        //TODO - Refactor - dont rescale, not very good
        MainMenuUI.transform.localScale = Vector3.one;
        LobbyMenuUI.transform.localScale = Vector3.zero;
        SettingsMenuUI.transform.localScale = Vector3.zero;

        LobbyMenuUI.OnStartGame += () => {OnStartGame?.Invoke();};

    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        MainMenuUI.transform.localScale = Vector3.zero;
        LobbyMenuUI.transform.localScale = Vector3.one;
        if (IsHost) 
        { 
            SettingsMenuUI.transform.localScale = Vector3.one;
        }

    }

    public void SetPlayerCount(int Players)
    {
        if (!LobbyMenuUI) { return ; }

        PlayersActive = Players;
        LobbyMenuUI.UpdatePlayerCount(Players);
        if (IsHost)
        {
            SettingsMenuUI.gameObject.SetActive(true);
        }

    }

}
