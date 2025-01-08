using System;
using System.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : NetworkBehaviour
{
    public static event Action<int> OnPlayerCountUpdated;
    public static event Action OnReadyGame;
    public static event Action OnStartGame;
    public event Action OnPauseGame;
    public event Action OnResumeGame;
    public event Action OnExitGame;

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
    
    private void Awake()
    {
        DontDestroyOnLoad( this );
    }
    private SO_GameSettings SelectedSettings;
    public override void OnNetworkSpawn()
    {

        //Server Client Updates
        NetworkManager.Singleton.OnConnectionEvent += (a,b) =>
        {
            /*Needs a Check to see if the disconnecting client was the host**/
            //SceneManager.LoadScene(0);
        };
        if (IsServer)
        {
            UIMenu.OnStartGame += TransitionLevel_Rpc;
            NetworkManager.Singleton.OnClientConnectedCallback += (id) =>
            {
                PlayerCount.Value++;

            };
            NetworkManager.Singleton.OnClientDisconnectCallback += (id) =>
            {
                PlayerCount.Value--;
            };
        }
        PlayerCount.OnValueChanged += (int previousValue, int newValue) =>
        {
            OnPlayerCountUpdated?.Invoke(newValue);
            if (UIMenu)
            {
                UIMenu.SetPlayerCount(PlayerCount.Value);
            }
        };

        GameTimeRemaining.OnValueChanged += (float previousValue, float newValue) =>
        {
            //Call to Arcades to change UI Elements
        };

    }
    [Rpc(SendTo.Everyone)]
    public void SetGameSettings_Rpc(int settingsIndex)
    {
        //Call to the arcade unit to update looks - Scope Creep Here :Skull:
    }

    [Rpc(SendTo.Everyone)]
    public void TransitionLevel_Rpc()
    {
        NetworkManager.SceneManager.OnLoadComplete += (a, b, c) =>
        {
            OnReadyGame?.Invoke();
            if (!IsServer) { return; }
            StartCoroutine(StartGameCountdown(3));
        };
        if (!IsServer) { return; }
        NetworkManager.SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
    }

    IEnumerator StartGameCountdown(int CountdownLength)
    {
        while (CountdownLength > 0) 
        {
            yield return new WaitForSeconds(1);
            CountdownLength--;
        }
        StartGame_Rpc();
        yield break;
    }

    [Rpc(SendTo.Everyone)]
    public void StartGame_Rpc()
    {
        OnStartGame?.Invoke();
    }
    
}
