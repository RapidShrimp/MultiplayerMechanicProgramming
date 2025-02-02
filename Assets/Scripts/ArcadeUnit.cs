using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class ArcadeUnit : NetworkBehaviour
{
    private Configuration[] Configurations;

    Coroutine CR_GameTimer;

    [SerializeField] GameObject JoystickPivot;
    Quaternion DesiredJoystickRotation;
    Coroutine CR_Joystick;

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

    private void Awake()
    {
        PlayerUI.transform.position = new Vector3(transform.position.x, 500);
    }
    public override void OnNetworkSpawn()
    {

        Score.OnValueChanged += Handle_ScoreChanged;

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
        Score.OnValueChanged -= Handle_ScoreChanged;

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
        PlayerUI.StartNewPuzzle();
    }

    #region Configurations
    private void Handle_ConfigurationUpdated(bool IsActive)
    {
    }
    private void Handle_ConfigurationSabotaged(int SabotageScore)
    {
        if(!IsOwner) { return; }
        Score.Value += SabotageScore;
    }

    #endregion

    public void PressButton(int ButtonIndex,bool Performed)
    {
        PlayerUI.ButtonPressed(ButtonIndex,Performed);
    }

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
    private void Handle_ScoreChanged(int oldScore, int newScore)
    {
        if (!PlayerUI) { Debug.Assert(false); return; }

        PlayerUI.UpdateScore(newScore);
    }
    #endregion
}
