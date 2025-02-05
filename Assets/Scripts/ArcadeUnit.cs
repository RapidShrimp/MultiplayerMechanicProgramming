using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class ArcadeUnit : NetworkBehaviour
{
    private Configuration[] Configurations;
    [SerializeField] protected int ConfigsCompleted;


    Coroutine CR_GameTimer;
    Coroutine CR_ConfigurationScramble;

    ArcadeButton[] Buttons;
    [SerializeField] GameObject JoystickPivot;
    Quaternion DesiredJoystickRotation;
    Coroutine CR_Joystick;

    [SerializeField] protected UI_Game PlayerUI; //The UI Component


    private NetworkVariable<int> Score = new NetworkVariable<int>(
    value: 0,
    NetworkVariableReadPermission.Everyone,
    NetworkVariableWritePermission.Owner);
    public int GetScore() { return Score.Value; }
    private void Awake()
    {

        Buttons = GetComponentsInChildren<ArcadeButton>();
        for(int i = 0; i < Buttons.Length; i++)
        {
            Color color = Color.white;
            switch(i)
            {
                case 0: color = Color.yellow;
                    break;
                case 1: color = Color.red;
                    break;
                case 2: color = Color.green;
                    break;
                case 3: color = Color.blue;
                    break;
            }
            Buttons[i].InitButton(color, i);
            Buttons[i].OnButtonPressed += PressButton;
        }
    }



    public override void OnNetworkSpawn()
    {

        Score.OnValueChanged += Handle_ScoreChanged;

        PlayerUI.transform.position = new Vector3(transform.position.x, 500);
        PlayerUI.OnScoreUpdated += Handle_UpdateScore;

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
        PlayerUI.OnScoreUpdated -= Handle_UpdateScore;

    }

    public void ReadyGame()
    {
        if (!IsOwner) { return; }
        PlayerUI.ToggleActiveRender(true);
        Score.Value = 0;
        foreach (Configuration config in Configurations)
        {
            config.StartModule();
        }
    }
    public void StartGame()
    {
        if (!IsOwner) { return; }
        Debug.Log("Started");
        PlayerUI.StartNewPuzzle();
        if (CR_ConfigurationScramble != null) { StopCoroutine(CR_ConfigurationScramble); }
        CR_ConfigurationScramble = StartCoroutine(RandomiseConfiguration());
    }

    public void GameEnded(bool IsWinner, int WinnerIndex)
    {
        StopAllCoroutines();
        PlayerUI.OnGameEnded(IsWinner, WinnerIndex);
    }


    #region Configurations
    private void Handle_ConfigurationUpdated(bool IsActive)
    {
        if (IsActive) { ConfigsCompleted++; }
        else ConfigsCompleted-- ;

        PlayerUI.ConfigurationSet = ConfigsCompleted == Configurations.Length;
    }
    private void Handle_ConfigurationSabotaged(int SabotageScore)
    {
        if(!IsOwner) { return; }
        Score.Value += SabotageScore;
    }

    IEnumerator RandomiseConfiguration()
    {
        while (true) 
        {
            yield return new WaitForSeconds(15);
            Configurations[UnityEngine.Random.Range(0,Configurations.Length)].StartModule();
        }
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


    #region UI
    public UI_Game GetArcadeUI()
    {
        return PlayerUI;
    }

    private void Handle_UpdateScore(int ScoreChange)
    {
        if (!IsOwner) { return; }
        Score.Value += ScoreChange;
    }
    private void Handle_ScoreChanged(int oldScore, int newScore)
    {
        if (!PlayerUI) { Debug.Assert(false); return; }
        PlayerUI.UpdateScore(newScore);
    }

    #endregion
}
