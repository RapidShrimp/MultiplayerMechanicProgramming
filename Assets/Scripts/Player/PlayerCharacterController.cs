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
        m_ArcadeUnit.ReadyGame();
        if (Camera.main) { Camera.main.enabled = false; }
        PlayerCam = GetComponentInChildren<Camera>();
        PlayerCam.enabled = true;
        GetComponentInChildren<AudioListener>().enabled = true;
    }
    private void Handle_OnStartGame()
    {
        if(!IsOwner) { return; }   
        PlayerInput.Enable();
        m_ArcadeUnit.StartGame();
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
        GameObject tmp = GetObjectUnderMouse();
        if (tmp == null) { yield break; }
        IInteractable Interaction = tmp.GetComponentInChildren<IInteractable>();
        if (Interaction == null) { yield break; }

        if (Interaction != null)
        {
            if (Interaction.OnClick()) 
            {
                yield break;
            };
        }
        else
        {
            while (true) 
            {
                if (GetObjectUnderMouse())
                {
                    Debug.Log("Do Drag Event");
                }
                yield return new WaitForFixedUpdate();
            }
        }
        yield break;
    }

    protected GameObject GetObjectUnderMouse()
    {
        Vector3 Mouse2World = Mouse.current.position.ReadValue();
        Mouse2World.z = 200;
        //Debug.DrawLine(PlayerCam.transform.position, PlayerCam.ScreenToWorldPoint(Mouse2World));
        Ray ray = new Ray();
        ray.origin = PlayerCam.transform.position;
        ray.direction = PlayerCam.ScreenToWorldPoint(Mouse2World);
        Debug.DrawRay(ray.origin, ray.direction, Color.red, 0.5f);
        bool RayHit = Physics.Raycast(ray, out RaycastHit HitInfo, float.MaxValue, LayerMask.NameToLayer("MouseInteractable"));
        if (RayHit) 
        {
            return HitInfo.collider.gameObject;
        }

        return null;
    }
}
