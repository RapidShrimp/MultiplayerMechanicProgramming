using System;
using System.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : NetworkBehaviour
{
    public static event Action<int> OnPlayerCountUpdated;
    public static event Action OnReadyGame;
    public static event Action OnStartGame;
    public static event Action OnPauseGame;
    public static event Action OnGameFinished;

    public static event Action OnResumeGame;
    public static event Action OnExitGame;

    public static event Action<int> OnGameTimerUpdated;
    protected Coroutine GameTimer;

    [SerializeField] protected float GameLength = 10;
    //private SO_GameSettings SelectedSettings;

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
    public override void OnNetworkSpawn()
    {
        NetworkManager.Singleton.SceneManager.OnLoadComplete += (a, b, c) =>
        {
            if(NetworkManager.Singleton.LocalClientId == a)
            {
                OnReadyGame?.Invoke();
                if (IsServer) { GameTimeRemaining.Value = GameLength;}
            }

            if (!IsServer || NetworkManager.Singleton.LocalClientId != a) { return; }
            StartCoroutine(StartGameCountdown(3));
        };
        
        //Server Client Updates
        NetworkManager.Singleton.OnConnectionEvent += (a,b) =>
        {
            /*Needs a Check to see if the disconnecting client was the host**/
            //SceneManager.LoadScene(0);
        };
        if (IsServer)
        {

            UIMenu.OnStartGame += TransitionLevel;
            NetworkManager.Singleton.OnClientConnectedCallback += (id) =>
            {
                PlayerCount.Value++;
                for(int ID = 0; ID< NetworkManager.Singleton.ConnectedClientsList.Count; ID++)
                {
                    GameObject NewPlayer = NetworkManager.Singleton.ConnectedClientsList[ID].PlayerObject.gameObject;
                    NewPlayer.GetComponent<PlayerCharacterController>().AssignPlayerIndex_Rpc(ID);
                }
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
            //Debug.Log($"Time {newValue}");
            OnGameTimerUpdated?.Invoke((int)newValue);
        };

    }
    [Rpc(SendTo.Everyone)]
    public void SetGameSettings_Rpc(int settingsIndex)
    {
        //Call to the arcade unit to update looks - Scope Creep Here :Skull:
    }

    public void TransitionLevel()
    {
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
        if (IsOwner) 
        { 
            if(GameTimer == null)
            {
                GameTimer = StartCoroutine(CountdownGameTimer(120));
            }
        }
    }



    IEnumerator CountdownGameTimer(int GameLength)
    {
        if (!IsServer) { yield break; } 
        while (GameTimeRemaining.Value > 0)
        {
            yield return new WaitForSeconds(1);
            GameTimeRemaining.Value--;
        }
        OnGameFinished?.Invoke();
    }
}
