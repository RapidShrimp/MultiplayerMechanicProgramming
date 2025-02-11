using System;
using System.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class HandleConfiguration : Configuration, IInteractable
{

    MeshRenderer HandleMesh;

    float TopYPos;
    float LerpValue;
    Coroutine CR_DesiredRotation;
    Coroutine CR_ResetLever;
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        TopYPos = GetComponent<Collider>().transform.position.y;
        HandleMesh = GetComponentInChildren<MeshRenderer>();
        IsCompleted.OnValueChanged += Handle_CompleteUpdated;
        Handle_CompleteUpdated(false, true);
    }


    public override void OnNetworkDespawn()
    {
        IsCompleted.OnValueChanged -= Handle_CompleteUpdated;

    }
    override public void StartModule()
    {
        if (IsOwner)
        {
            IsCompleted.Value = false;
        }
    }

    [Rpc(SendTo.Owner)]
    protected void SetModuleActive_Rpc()
    {
        if (!IsOwner) { return; }
        IsCompleted.Value = false;

    }

    private void Handle_CompleteUpdated(bool previousValue, bool newValue)
    {
        Color NewColour = newValue ? Color.green : Color.red;
        HandleMesh.materials[1].color = NewColour;
    }


    public bool OnClick()
    {
        if (IsOwner) { return false; }
        else
        {
            if (IsCompleted.Value) { Sabotage_Rpc(); }
            SetModuleActive_Rpc();
            return true;       
        }
    }

    public bool OnDrag(Vector3 WorldPos, bool IsHeld)
    {
        if (IsOwner && !IsCompleted.Value)
        {
            LerpValue = WorldPos.y - TopYPos;
            if (CR_DesiredRotation == null) 
            { 
                CR_DesiredRotation = StartCoroutine(MoveTowardsDesiredLerp());
            }

            if(CR_ResetLever != null)
            {
                StopCoroutine(CR_ResetLever);
                CR_ResetLever = null;
            }
            CR_ResetLever = StartCoroutine(ResetPosition(true));
            return true; 
        }
        else
        {
            return false;
        }
    }
    IEnumerator MoveTowardsDesiredLerp()
    {
        while(true) 
        { 
            //Debug.Log($"Rotation {transform.rotation.eulerAngles.x}");
            if(transform.rotation.eulerAngles.x == 270)
            {
                IsCompleted.Value = true;
                yield break;
            }
            float Rotation = Mathf.Lerp(-90, 0, LerpValue);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(Rotation, 0, 0), 2);
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator ResetPosition(bool ReachedBottom)
    {
        yield return new WaitForSeconds(2);
        StopCoroutine(CR_DesiredRotation);
        CR_DesiredRotation = null;

        while (transform.rotation != Quaternion.identity) 
        {
            yield return new WaitForFixedUpdate();
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.identity, 2);
        }
        CR_ResetLever = null;   
    }
}
