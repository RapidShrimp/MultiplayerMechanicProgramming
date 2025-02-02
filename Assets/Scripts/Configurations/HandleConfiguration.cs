using System.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class HandleConfiguration : Configuration, IInteractable
{

    [SerializeField] GameObject HandleMesh;
    float TopYPos;
    float LerpValue;
    Coroutine CR_DesiredRotation;
    Coroutine CR_ResetLever;
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        TopYPos = GetComponent<Collider>().transform.position.y;
    }
    public override void OnNetworkDespawn()
    {

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

    public bool OnDrag(Vector3 WorldPos)
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
            HandleMesh.transform.rotation = Quaternion.RotateTowards(HandleMesh.transform.rotation, Quaternion.Euler(Rotation, 0, 0), 2);
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator ResetPosition(bool ReachedBottom)
    {
        yield return new WaitForSeconds(2);
        StopCoroutine(CR_DesiredRotation);
        CR_DesiredRotation = null;

        while (HandleMesh.transform.rotation != Quaternion.identity) 
        {
            yield return new WaitForFixedUpdate();
            HandleMesh.transform.rotation = Quaternion.RotateTowards(HandleMesh.transform.rotation, Quaternion.identity, 2);
        }
        CR_ResetLever = null;   
    }
}
