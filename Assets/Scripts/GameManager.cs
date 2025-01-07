using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : NetworkBehaviour
{
 /*   public event Action OnStartGame;
    public event Action OnPauseGame;
    public event Action OnResumeGame;
    public event Action OnExitGame;*/

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
            UIMenu.OnStartGame += StartGame;
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

    public void StartGame()
    {
        NetworkManager.SceneManager.LoadScene("GameScene", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
}
