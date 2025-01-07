using UnityEngine;
using Unity.Netcode;
using System;
using UnityEngine.SceneManagement;

public class NetworkManagerUI : NetworkBehaviour
{

    public event Action OnStartGame;
    [SerializeField] private UI_MainMenu MainMenuUI;
    [SerializeField] private UI_LobbyMenu LobbyMenuUI;

    private int PlayersActive;

    private void Awake()
    {
        if (!MainMenuUI) { return; }
        if (!LobbyMenuUI) { return; }

        MainMenuUI.transform.localScale = Vector3.one;
        LobbyMenuUI.transform.localScale = Vector3.zero;

        LobbyMenuUI.OnStartGame += () => {OnStartGame?.Invoke();};

    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        MainMenuUI.transform.localScale = Vector3.zero;
        LobbyMenuUI.transform.localScale = Vector3.one;
    }

    public void SetPlayerCount(int Players)
    {
        if (!LobbyMenuUI) { return ; }

        PlayersActive = Players;
        LobbyMenuUI.UpdatePlayerCount(Players);

    }

}
