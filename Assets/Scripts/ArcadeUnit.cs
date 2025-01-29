using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class ArcadeUnit : NetworkBehaviour
{
    private PuzzleModule[] Puzzles;
    private PuzzleModule ActiveModule = null;
    [SerializeField] private Configuration[] Configurations;

    Coroutine CR_GameTimer;
    [SerializeField] GameObject JoystickPivot;
    [SerializeField] GameObject UI_ScreenPrefab;

    protected UI_Game PlayerUI;

    private int MaxHealth = 100;
    private NetworkVariable<int> GameTimeRemaining = new NetworkVariable<int>(
        value: 0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner);
    
    private NetworkVariable<float> Health = new NetworkVariable<float>(
    value: 100,
    NetworkVariableReadPermission.Everyone,
    NetworkVariableWritePermission.Owner);

    private NetworkVariable<int> Score = new NetworkVariable<int>(
    value: 0,
    NetworkVariableReadPermission.Everyone,
    NetworkVariableWritePermission.Owner);


    public override void OnNetworkSpawn()
    {
        if (!IsOwner) { return; }
        Configurations = GetComponentsInChildren<Configuration>();
        foreach (Configuration config in Configurations) 
        {
            config.OnConfigurationUpdated += Handle_ConfigurationUpdated;
            config.OnConfigurationSabotaged += Handle_ConfigurationSabotaged;
        }

    }
    public override void OnNetworkDespawn()
    {
    }

    [Rpc(SendTo.Server)]
    public void ServerSpawnUI_Rpc()
    {
        GameObject SpawnedUI = Instantiate(UI_ScreenPrefab);
        PlayerUI = SpawnedUI.GetComponent<UI_Game>();
        PlayerUI.GetComponent<NetworkObject>().Spawn();
        Debug.Log(PlayerUI);
    }

    public void ReadyGame()
    {
        if (!IsOwner) { return; }
 
        Debug.Log($"Readied {GetInstanceID()}");
        MaxHealth = 100;// Settings.DefaultHealth;
        Score.Value = 0;
        Health.Value = MaxHealth;
        GameTimeRemaining.Value = 120; // Settings.GameTime; // Change this later
        foreach (Configuration config in Configurations)
        {
            config.StartModule();
        }
        ServerSpawnUI_Rpc();
    }
    public void StartGame()
    {
        if (!IsOwner) { return; }
        Debug.Log("Started");
        CR_GameTimer = StartCoroutine(ArcadeTimer());
    }

    #region Configurations
    private void Handle_ConfigurationUpdated(bool IsActive)
    {
    }
    private void Handle_ConfigurationSabotaged()
    {
        Score.Value += 25;
    }

    #endregion

    #region Puzzles
    public void BindToPuzzles()
    {
        foreach (PuzzleModule Module in Puzzles)
        {
            Module.OnPuzzleComplete += Handle_PuzzleComplete;
            Module.OnPuzzleFail += Handle_PuzzleFail;
            Module.OnPuzzleError += Handle_PuzzleError;
        }
    }

    private void Handle_PuzzleComplete(float AwardedTime)
    {
        Debug.Log("Completed Puzzle");
        Score.Value += 150;
    }
    private void Handle_PuzzleFail(float PunishmentTime)
    {
        Debug.Log("Failed Puzzle");
        Score.Value -= 25;
    }
    private void Handle_PuzzleError()
    {
        Debug.Log("Errored Puzzle");
        Score.Value -= 10;
    }
    public void StartNewPuzzle()
    {
        if (ActiveModule != null)
        {
            ActiveModule.DeactivatePuzzleModule();
        }

        ActiveModule = Puzzles[UnityEngine.Random.Range(0, Puzzles.Length)];
        if (ActiveModule == null) { Debug.LogError("Couldnt Find Puzzle"); }

        ActiveModule.StartPuzzleModule();
    }
    #endregion

    public void SetDesiredJoystickPosition(Vector2 joystickPosition)
    {
        JoystickPivot.transform.localRotation = Quaternion.Euler(joystickPosition.y*45,0,joystickPosition.x*-45);
    }
    IEnumerator ArcadeTimer()
    {
        while (GameTimeRemaining.Value > 0) 
        {
            yield return new WaitForSecondsRealtime(1);
            GameTimeRemaining.Value --;
        }

    }


    #region UI

    public Camera GetUIRenderCamera()
    {
        return PlayerUI.GetUICamera(); ;
    }

    #endregion
}
