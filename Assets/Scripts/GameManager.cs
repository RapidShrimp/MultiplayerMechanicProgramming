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
    
    public static event Action<int> OnLoadingLevel; //Int is the fade time
    public static event Action OnReadyGame;
    public static event Action OnStartGame;

    public static event Action<int> OnGameFinished; //Int is the winning player

    public static event Action OnPauseGame;
    public static event Action OnResumeGame;
    public static event Action OnExitGame;

    public static event Action<int> OnGameTimerUpdated;
    protected Coroutine GameTimer;
    [SerializeField] protected float GameLength = 120;
    [SerializeField] protected float LevelTransitionDelay = 3;
    //private SO_GameSettings SelectedSettings;

    protected SFX_Item ConnectionSounds;
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
        ConnectionSounds = GetComponentInChildren<SFX_Item>();
    }
    public override void OnNetworkSpawn()
    {

        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
        }
        NetworkManager.Singleton.SceneManager.OnLoadComplete += OnLevelLoaded;
        PlayerCount.OnValueChanged += PlayersUpdated;
        UIMenu.OnStartGame += OnUIStartGame; 

        GameTimeRemaining.OnValueChanged += (float previousValue, float newValue) =>
        {
            OnGameTimerUpdated?.Invoke((int)newValue);
        };

    }

    private void OnUIStartGame()
    {
        OnLoadingLevel?.Invoke(2);
        if (IsServer)
        {
            StartCoroutine(LevelFade(2));
        }
    }

    IEnumerator LevelFade(float FadeTime)
    {
        yield return new WaitForSeconds(FadeTime);
        TransitionLevel("GameScene");
    }

    private void OnClientDisconnect(ulong Id)
    {
        PlayerCount.Value--;
        SFX_AudioManager.Singleton.PlaySoundToPlayer(ConnectionSounds.FindAudioByName("Leave"), 1, 1);
    }

    private void OnClientConnected(ulong Id)
    {
        PlayerCount.Value++;
        UpdatePlayerIDs();
        SFX_AudioManager.Singleton.PlaySoundToPlayer(ConnectionSounds.FindAudioByName("Join"),1,1);

    }
    private void PlayersUpdated(int previousValue, int newValue)
    {
        OnPlayerCountUpdated?.Invoke(newValue);
        if (UIMenu)
        {
            UIMenu.SetPlayerCount(PlayerCount.Value);
        }
    }

    public void UpdatePlayerIDs()
    {
        for (int ID = 0; ID < NetworkManager.Singleton.ConnectedClientsList.Count; ID++)
        {
            GameObject NewPlayer = NetworkManager.Singleton.ConnectedClientsList[ID].PlayerObject.gameObject;
            NewPlayer.GetComponent<PlayerCharacterController>().AssignPlayerIndex_Rpc(ID);
        }
    }

    [Rpc(SendTo.Everyone)]
    public void SetGameSettings_Rpc(int settingsIndex)
    {
        //Call to the arcade unit to update looks - Scope Creep Here :Skull:
    }

    public void TransitionLevel(string LevelName = "MainMenu")
    {
        if (!IsServer) { return; }
        NetworkManager.Singleton.SceneManager.LoadScene(LevelName, LoadSceneMode.Single);
    }

    private void OnLevelLoaded(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
    {
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            OnReadyGame?.Invoke();
            if (IsServer) { GameTimeRemaining.Value = GameLength; }
        }

        if (!IsServer || NetworkManager.Singleton.LocalClientId != clientId) { return; }
        StartCoroutine(StartGameCountdown((int)LevelTransitionDelay));
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

    protected int GetHighestScoringPlayer()
    {
        int HighestScoringPlayer = 0;
        int HighsestScore = 0;
        for (int ID = 0; ID < NetworkManager.Singleton.ConnectedClientsList.Count; ID++)
        {

            GameObject NewPlayer = NetworkManager.Singleton.ConnectedClientsList[ID].PlayerObject.gameObject;
            int PlayerScore = NewPlayer.GetComponent<PlayerCharacterController>().GetArcadeScore();
            if ( PlayerScore > HighsestScore) 
            { 
                HighsestScore = PlayerScore;
                HighestScoringPlayer = ID;
            };
        }

        return HighestScoringPlayer;
    }

    IEnumerator CountdownGameTimer(int GameLength)
    {
        if (!IsServer) { yield break; } 
        while (GameTimeRemaining.Value > 0)
        {
            GameTimeRemaining.Value--;
            yield return new WaitForSeconds(1);
        }

        EndGame_Rpc(GetHighestScoringPlayer());
    }
    
    
    [Rpc(SendTo.Everyone)]
    public void EndGame_Rpc(int WinnerID)
    {
        OnGameFinished?.Invoke(WinnerID);
    }
}
