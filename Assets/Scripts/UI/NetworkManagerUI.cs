using UnityEngine;
using Unity.Netcode;
using System;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

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

        //Exit if none
        if (!MainMenuUI || !LobbyMenuUI || !SettingsMenuUI) { return; }

        
        MainMenuUI.gameObject.SetActive(true);
        LobbyMenuUI.gameObject.SetActive(false);
        LobbyMenuUI.OnUIRequestStartGame += Handle_OnStartGame;

        SettingsMenuUI.gameObject.SetActive(false);
        SettingsMenuUI.OnSettingsChanged += Handle_OnSettingSchanged; 

    }

    private void Handle_OnStartGame()
    {
        SettingsMenuUI.OnSettingsApply();
        OnStartGame?.Invoke();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        MainMenuUI.gameObject.SetActive(false);
        if (IsHost) 
        { 
            SettingsMenuUI.gameObject.SetActive(true); 
            LobbyMenuUI.IsHost = true;
        }
    }

    public void SetPlayerCount(int Players)
    {
        if (!LobbyMenuUI) { return ; }
        LobbyMenuUI.gameObject.SetActive(true);
        PlayersActive = Players;
        LobbyMenuUI.UpdatePlayerCount(Players);
    }

    public void OnLoadGameSettings(GameSettings LoadedSettings)
    {
        SettingsMenuUI.OnLoadSettings(LoadedSettings);
    }

    private void Handle_OnSettingSchanged(GameSettings settings)
    {
        OnUpdateGameSettings?.Invoke(settings);

        PlayerPrefs.SetInt("GameTime", settings.GameTime);
        PlayerPrefs.SetInt("ScrambleConfigs", settings.ScrambleConfiguration == true ? 1 : 0);
        PlayerPrefs.SetInt("RequireConfigs", settings.ConfigurationRequired == true ? 1 : 0);
        PlayerPrefs.SetInt("SabotagingScores", settings.SabotageScoring == true ? 1 : 0);
        PlayerPrefs.Save();
    }
}
