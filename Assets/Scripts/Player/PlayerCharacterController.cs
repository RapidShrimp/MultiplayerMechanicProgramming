using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCharacterController : NetworkBehaviour
{
    PlayerInputActions PlayerInput;
    Camera PlayerCam;

    ArcadeUnit m_ArcadeUnit;

    Coroutine CR_MouseDetection;

    public override void OnNetworkSpawn()
    {
        DontDestroyOnLoad(this);
        m_ArcadeUnit = GetComponentInChildren<ArcadeUnit>();
        if (!IsOwner) { return; }
        transform.position = new Vector3(transform.position.x + 2, transform.position.y, transform.position.z);
        GameManager.OnReadyGame += Handle_OnGameReady;
        GameManager.OnStartGame += Handle_OnStartGame;

        PlayerInput = new PlayerInputActions();
        //Bind Player Events
        PlayerInput.Player.Move.performed += Handle_PlayerMove;
        PlayerInput.Player.MouseLClick.started += Handle_PlayerClick;
        PlayerInput.Player.MouseLClick.canceled+= Handle_PlayerClick;

    }

    private void Handle_PlayerClick(InputAction.CallbackContext context)
    {
        if(!IsOwner) { return; }
        if (context.started) 
        { 
            if (CR_MouseDetection == null) 
            {
                CR_MouseDetection = StartCoroutine(Handle_MouseDown());
            }

        }
        else
        {
            if (CR_MouseDetection!=null)
            {
                StopCoroutine(CR_MouseDetection);
                CR_MouseDetection = null;
            }

        }
    }

    private void Handle_PlayerMove(InputAction.CallbackContext context)
    {
        if (!IsOwner) { return; }

    }

    public void Handle_OnGameReady()
    {
        if (!IsOwner) { return; }
        Debug.Log("Owned Start");
        PlayerInput.Enable();
        if (Camera.main) { Camera.main.enabled = false; }
        PlayerCam = GetComponentInChildren<Camera>();
        PlayerCam.enabled = true;
        GetComponentInChildren<AudioListener>().enabled = true;
        m_ArcadeUnit.StartGame(null);
        
    }
    private void Handle_OnStartGame()
    {
        m_ArcadeUnit.StartGame(null);
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        if(!IsOwner) { return; }
        //Unbind Player Events
        GameManager.OnReadyGame -= Handle_OnGameReady;
        GameManager.OnStartGame -= Handle_OnStartGame;

        PlayerInput.Disable();
        PlayerInput.Player.Move.performed -= Handle_PlayerMove;
        PlayerInput.Player.MouseLClick.started -= Handle_PlayerClick;
        PlayerInput.Player.MouseLClick.canceled -= Handle_PlayerClick;

    }

    IEnumerator Handle_MouseDown()
    {
        while (true) 
        {
            Vector3 Mouse2World = Mouse.current.position.ReadValue();
            Mouse2World.z = 10;
            //Debug.DrawLine(PlayerCam.transform.position, PlayerCam.ScreenToWorldPoint(Mouse2World));
            Debug.Log($"Mouse Down");

            bool RayHit = Physics.Raycast(PlayerCam.transform.position, PlayerCam.ScreenToWorldPoint(Mouse2World), out RaycastHit HitInfo);
            if (RayHit) 
            {
                Debug.Log($"Hit {HitInfo.collider.name}");
                if (HitInfo.collider.tag == "MouseInteractable")
                {
                    Debug.Log("Found Interactable");
                }
            }
            yield return new WaitForFixedUpdate();
        
        }
    }
}
