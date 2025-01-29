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
    Quaternion DesiredJoystickRotation;
    Coroutine CR_Joystick;

    [SerializeField] GameObject UI_ScreenPrefab; //The Render Texture
    [SerializeField] protected UI_Game PlayerUI; //The UI Component

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
    private void Handle_ConfigurationSabotaged(int SabotageScore)
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
        if (CR_Joystick == null) { CR_Joystick = StartCoroutine(MoveJoystickToLocation()); }
        DesiredJoystickRotation = Quaternion.Euler(joystickPosition.y*45,0,joystickPosition.x*-45);
    }

    public IEnumerator MoveJoystickToLocation()
    {
        while (true)
        {
            JoystickPivot.transform.localRotation = Quaternion.RotateTowards(JoystickPivot.transform.localRotation,DesiredJoystickRotation,8);
            yield return new WaitForFixedUpdate();

        }
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

    public UI_Game GetArcadeUI()
    {
        return PlayerUI;
    }

    #endregion
}
