using NUnit.Framework;
using System;
using System.Collections;
using System.Linq;
using Unity.Netcode;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCharacterController : NetworkBehaviour
{
    PlayerInputActions PlayerInput;
    PlayerInputActions PlayerCheats;
    Camera PlayerCam;
    Camera CurrentlyViewing;
    ArcadeUnit m_ArcadeUnit;
    
    [SerializeField] GameObject UI_HUDPrefab;
    protected int PlayerIndex;
    protected int CurrentListID = 0;

    private UI_HUDSelectPlayer UI_HUD;
    Coroutine CR_MouseDetection;
    Coroutine CR_MoveMouse;
    Coroutine CR_MoveAction;

    public override void OnNetworkSpawn()
    {
        DontDestroyOnLoad(this);
        m_ArcadeUnit = GetComponentInChildren<ArcadeUnit>();
        PlayerCam = GetComponentInChildren<Camera>();

        if (!IsOwner) { return; }
        SFX_AudioManager.Singleton.SwapPlayer(transform.gameObject);
        GameManager.OnLoadingLevel += Handle_LevelTransition;
        GameManager.OnReadyGame += Handle_OnGameReady;
        GameManager.OnStartGame += Handle_OnStartGame;
        GameManager.OnGameFinished += Handle_OnGameEnded;
        PlayerInput = new PlayerInputActions();
        //Bind Player Events
        PlayerInput.Player.Move.performed += Handle_PlayerMove;
        PlayerInput.Player.Move.canceled += Handle_PlayerMove;
        PlayerInput.Player.MouseLClick.started += Handle_PlayerClick;
        PlayerInput.Player.MouseLClick.canceled += Handle_PlayerClick;

        PlayerInput.Player.Look.performed += Handle_PlayerLook;
        PlayerInput.Player.Look.canceled += Handle_PlayerLook;

        PlayerInput.Player.YellowAction.performed += Handle_Action1;
        PlayerInput.Player.RedAction.performed += Handle_Action2;
        PlayerInput.Player.GreenAction.performed += Handle_Action3;
        PlayerInput.Player.BlueAction.performed += Handle_Action4;
        PlayerInput.Player.YellowAction.canceled += Handle_Action1;
        PlayerInput.Player.RedAction.canceled += Handle_Action2;
        PlayerInput.Player.GreenAction.canceled += Handle_Action3;
        PlayerInput.Player.BlueAction.canceled += Handle_Action4;


        PlayerInput.Player.ShoulderLeft.performed += Handle_LB;
        PlayerInput.Player.ShoulderRight.performed += Handle_RB;

        PlayerCheats = new PlayerInputActions();
        PlayerCheats.Cheats.MuteAudio.performed += (a) => { SFX_AudioManager.Singleton.ToggleMuteAudio(); };
        PlayerCheats.Enable();
    }



    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        if (!IsOwner) { return; }
        //Unbind Player Events
        GameManager.OnLoadingLevel -= Handle_LevelTransition;
        GameManager.OnReadyGame -= Handle_OnGameReady;
        GameManager.OnStartGame -= Handle_OnStartGame;
        GameManager.OnGameFinished -= Handle_OnGameEnded;

        if(PlayerInput == null) { return; }
        PlayerInput.Disable();
        PlayerInput.Player.Move.performed -= Handle_PlayerMove;
        PlayerInput.Player.Move.canceled -= Handle_PlayerMove;
        PlayerInput.Player.MouseLClick.started -= Handle_PlayerClick;
        PlayerInput.Player.MouseLClick.canceled -= Handle_PlayerClick;

        PlayerInput.Player.Look.performed -= Handle_PlayerLook;
        PlayerInput.Player.Look.canceled -= Handle_PlayerLook;

        PlayerInput.Player.YellowAction.performed -= Handle_Action1;
        PlayerInput.Player.RedAction.performed -= Handle_Action2;
        PlayerInput.Player.GreenAction.performed -= Handle_Action3;
        PlayerInput.Player.BlueAction.performed -= Handle_Action4;

        PlayerInput.Player.YellowAction.canceled -= Handle_Action1;
        PlayerInput.Player.RedAction.canceled -= Handle_Action2;
        PlayerInput.Player.GreenAction.canceled -= Handle_Action3;
        PlayerInput.Player.BlueAction.canceled -= Handle_Action4;

        PlayerInput.Player.ShoulderLeft.performed -= Handle_LB;
        PlayerInput.Player.ShoulderRight.performed -= Handle_RB;
        
        PlayerCheats.Disable();

    }

    #region PlayerInput
    private void Handle_LB(InputAction.CallbackContext context) { Handle_OnSelectPlayer(-1); }
    private void Handle_RB(InputAction.CallbackContext context) { Handle_OnSelectPlayer(1); }
    private void Handle_Action1(InputAction.CallbackContext context)
    {
        bool IsDown = context.performed ? true : false;
        m_ArcadeUnit.PressButton(0, IsDown);
    }
    private void Handle_Action2(InputAction.CallbackContext context)
    {
        bool IsDown = context.performed ? true : false;
        m_ArcadeUnit.PressButton(1, IsDown);
    }
    private void Handle_Action3(InputAction.CallbackContext context)
    {
        bool IsDown = context.performed ? true : false;
        m_ArcadeUnit.PressButton(2, IsDown);
    }
    private void Handle_Action4(InputAction.CallbackContext context) 
    {
        bool IsDown = context.performed ? true : false;
        m_ArcadeUnit.PressButton(3,IsDown); 
    }

    private void Handle_PlayerLook(InputAction.CallbackContext context)
    {

        if (context.performed && CR_MoveMouse == null)
        {
            CR_MoveMouse = StartCoroutine(MoveMouse());
        }
        else if (context.canceled && CR_MoveMouse != null)
        {
            StopCoroutine(CR_MoveMouse);
            CR_MoveMouse = null;
        }
    }

    IEnumerator MoveMouse()
    {
        while (PlayerInput.Player.Look.IsInProgress())
        {
            yield return new WaitForFixedUpdate();


            //PlayerInput.Player.Look.ReadValue<Vector2>()
            Vector2 currentPosition = Mouse.current.position.ReadValue();
            Vector2 newPosition = currentPosition + (PlayerInput.Player.Look.ReadValue<Vector2>().normalized * 5.0f); 
            Mouse.current.WarpCursorPosition(newPosition);
        }
    }

    private void Handle_PlayerClick(InputAction.CallbackContext context)
    {
        if (!IsOwner) { return; }

        // Hold Functionality
        if (context.started)
        {
            if (CR_MouseDetection == null)
            {
                CR_MouseDetection = StartCoroutine(Handle_MouseDown());
            }

        }
        else
        {
            if (CR_MouseDetection != null)
            {
                StopCoroutine(CR_MouseDetection);
                CR_MouseDetection = null;
            }

        }
    }

    private void Handle_PlayerMove(InputAction.CallbackContext context)
    {
        if (!IsOwner) { return; }

        if (context.performed)
        {
            if(CR_MoveAction != null) { return; }
            CR_MoveAction = StartCoroutine(MoveJoystick());
        }
        else if (context.canceled)
        {
            if (CR_MoveAction!=null)
            {
                StopCoroutine(CR_MoveAction);
                CR_MoveAction = null;
            }
            m_ArcadeUnit.SetDesiredJoystickPosition(Vector2.zero,false);
        }
    }
    IEnumerator MoveJoystick()
    {
        while (true)
        {
            m_ArcadeUnit.SetDesiredJoystickPosition(PlayerInput.Player.Move.ReadValue<Vector2>(),true);
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator Handle_MouseDown()
    {
        RaycastHit Hit;
        if (!GetHitUnderMouse(CurrentlyViewing, out Hit)) { yield break; }
        GameObject tmp = Hit.collider.gameObject;
        if (tmp == null) { yield break; }
        IInteractable Interaction = tmp.GetComponentInChildren<IInteractable>();
        if (Interaction == null) { yield break; }


        if (Interaction.OnClick())
        {
            yield break;
        };

        while (PlayerInput.Player.MouseLClick.IsInProgress())
        {
            GetHitUnderMouse(CurrentlyViewing, out Hit);
            if (Interaction.OnDrag(Hit.point))
            {
                yield return new WaitForSeconds(0.1f);
            }
            else
            {
                yield break;
            }
        }
    }

    protected bool GetHitUnderMouse(Camera PlayerCamera, out RaycastHit HitResult)
    {
        Vector3 Mouse2World = Mouse.current.position.ReadValue();
        Ray ray = new Ray();
        Mouse2World.z = PlayerCamera.farClipPlane;
        ray.origin = PlayerCamera.transform.position;
        ray.direction = PlayerCamera.ScreenToWorldPoint(Mouse2World);
        Debug.DrawRay(ray.origin, ray.direction, Color.red, 0.5f);
        Debug.DrawLine(PlayerCam.transform.position, PlayerCam.ScreenToWorldPoint(Mouse2World), Color.blue, 0.5f);
        bool RayHit = Physics.Raycast(ray, out HitResult, float.MaxValue);
        return RayHit;
    }

    #endregion

    public int GetArcadeScore()
    {
        return m_ArcadeUnit.GetScore();
    }
    private void Handle_LevelTransition(int Time)
    {

        ScreenFade Fader = GetComponentInChildren<ScreenFade>();
        if (Fader == null)
        {
            return;
        }
        StartCoroutine(Fader.FadeGUI(Time/2, false));
        //Do Fading Here
    }

    private void OnClientDisconnect(ulong id)
    {
        ArcadeScreenDisplay Screen = GetComponentInChildren<ArcadeScreenDisplay>();
        Screen.ToggleScreenActive_Rpc(false);
    }
    [Rpc(SendTo.Everyone)]
    public void AssignPlayerIndex_Rpc(int ClientIndex)
    {
        PlayerIndex = ClientIndex;
        UI_Game GotUI = m_ArcadeUnit.GetArcadeUI();
        if(GotUI == null) { Debug.LogError("No UI Found"); }
        GotUI.PlayerIdentifier.InitPlayerIdentifier_Rpc(PlayerIndex);
        transform.position = new Vector3((2 * PlayerIndex), 0, 0);
    }

    public void Handle_OnGameReady()
    {
        if (!IsOwner) { return; }


        ScreenFade Fader = GetComponentInChildren<ScreenFade>();
        if (Fader != null) 
        { 
            StartCoroutine(Fader.FadeGUI(0.5f, true));
        }

        CurrentListID = PlayerIndex;
        m_ArcadeUnit.ReadyGame();
        if (Camera.main) { Camera.main.enabled = false; }
        PlayerCam.enabled = true;
        CurrentlyViewing = PlayerCam;
        GameObject Hud = Instantiate(UI_HUDPrefab);
        UI_HUD = Hud.GetComponent<UI_HUDSelectPlayer>();
        UI_HUD.OnSelectPlayer += Handle_OnSelectPlayer;

    }

    private void Handle_OnStartGame()
    {
        if (!IsOwner) { return; }
        m_ArcadeUnit.GetArcadeUI().ToggleActiveRender(true);
        PlayerInput.Enable();
        m_ArcadeUnit.StartGame();
    }

    private void Handle_OnGameEnded(int WinningPlayer)
    {
        PlayerInput.Disable();

        //Disable old Render
        if(CurrentListID != PlayerIndex) 
        {
            ToggleArcadeUnitView(CurrentListID, false);
            ToggleArcadeUnitView(PlayerIndex, true);
        }

        m_ArcadeUnit.GameEnded(PlayerIndex == WinningPlayer, WinningPlayer);
        StopAllCoroutines();

    }

    private void Handle_OnSelectPlayer(int Direction)
    {
        if(!IsOwner) { return; }
        CurrentListID = Mathf.Clamp(CurrentListID, 0,NetworkManager.Singleton.ConnectedClientsList.Count-1);
        ToggleArcadeUnitView(CurrentListID,false);
        CurrentListID = (int)Mathf.Repeat(CurrentListID + Direction, NetworkManager.Singleton.ConnectedClientsList.Count);
        ToggleArcadeUnitView(CurrentListID,true);

    }

    void ToggleArcadeUnitView(int PlayerIndex, bool IsViewing)
    {

        GameObject NextClient = NetworkManager.Singleton.ConnectedClientsList[PlayerIndex].PlayerObject.gameObject;
        if (NextClient == null) { return; }
        SFX_AudioManager.Singleton.SwapPlayer(NextClient);
        PlayerCharacterController FoundClient = NextClient.GetComponent<PlayerCharacterController>();
        ArcadeUnit FoundArcade = NextClient.GetComponentInChildren<ArcadeUnit>();
        FoundClient.PlayerCam.enabled = IsViewing;
        FoundArcade.GetArcadeUI().ToggleActiveRender(IsViewing);

        if (IsViewing) { CurrentlyViewing = FoundClient.PlayerCam; }
    }

}

