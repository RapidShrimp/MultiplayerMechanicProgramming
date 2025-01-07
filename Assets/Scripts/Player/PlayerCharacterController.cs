using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCharacterController : NetworkBehaviour
{
    PlayerInputActions PlayerInput;
    Camera PlayerCam;
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsOwner) { return; }

        PlayerInput = new PlayerInputActions();
        
        //Bind Player Events
        PlayerInput.Player.Jump.performed += Handle_PlayerJump;
    }

    public void Handle_OnGameStarted()
    {
        if (!IsOwner) { return; }
        PlayerInput.Enable();
        Camera.main.enabled = false;
        PlayerCam = GetComponentInChildren<Camera>();
        PlayerCam.enabled = true;
        GetComponentInChildren<AudioListener>().enabled = true;
        
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        if(!IsOwner) { return; }
        //Unbind Player Events
        PlayerInput.Disable();
        PlayerInput.Player.Jump.performed -= Handle_PlayerJump;
    }

    private void Handle_PlayerJump(InputAction.CallbackContext context)
    {
        if(!IsOwner) { return; }
        Debug.Log($"Player input on {gameObject.name}");
    }
}
