using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Camera))]
[SelectionBase]
public class PlayerCharacterController : NetworkBehaviour
{
    Camera PlayerCam;
    PlayerInputActions PlayerInput;

    private void OnEnable()
    {
        PlayerInput = new PlayerInputActions();
        PlayerInput.Enable();

        //Bind Player Events
        PlayerInput.Player.Jump.performed += Handle_PlayerJump;
        Debug.Log("Bound");
    }
    private void OnDisable()
    {

        //Unbind Player Events
        PlayerInput.Player.Jump.performed -= Handle_PlayerJump;
    }

    private void Handle_PlayerJump(InputAction.CallbackContext context)
    {
        if(!IsOwner) { return; }
        Debug.Log($"Player input on {gameObject.name}");
    }



    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
