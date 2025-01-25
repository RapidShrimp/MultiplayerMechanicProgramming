using System.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class HandleConfiguration : Configuration, IInteractable
{

    [SerializeField] GameObject HandleMesh;
    float DistanceTolerance = 0.025f;
    float TopYPos;
    float BottomYPos;
    Coroutine CR_DesiredRotation;
    Coroutine CR_ResetLever;
    public override void OnNetworkSpawn()
    {
        TopYPos = GetComponent<Collider>().transform.position.y;
        BottomYPos = transform.position.y;
    }
    public override void OnNetworkDespawn()
    {

    }

    override public void StartModule()
    {

    }

    public bool OnClick()
    {
        return false;
    }

    public bool OnDrag(Vector3 WorldPos)
    {

        if (IsOwner)
        {
            float LerpValue = WorldPos.y - TopYPos;
            if(CR_DesiredRotation == null)
            {
                CR_DesiredRotation = StartCoroutine(MoveTowardsDesiredLerp(LerpValue));
            }

            if(CR_ResetLever != null)
            {
                StopCoroutine(CR_ResetLever);
                CR_ResetLever = null;
            }
            CR_ResetLever = StartCoroutine(ResetPosition(true));
            bool Bottom = LerpValue < 0;
            return !Bottom; 
        }
        else
        {
            return false;
        }
    }
    IEnumerator MoveTowardsDesiredLerp(float LerpValue)
    {
        float Rotation = Mathf.Lerp(-90, 0, LerpValue);
        while(true) 
        { 
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
