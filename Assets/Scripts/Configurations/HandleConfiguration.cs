using System.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class HandleConfiguration : Configuration, IInteractable
{

    [SerializeField] GameObject HandleMesh;
    float DistanceTolerance = 0.025f;

    public override void OnNetworkSpawn()
    {

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
        Debug.Log("Here");
        if (IsOwner)
        {
            
            HandleMesh.transform.position = new Vector3 (HandleMesh.transform.position.x,WorldPos.y, HandleMesh.transform.position.z);
        }
        //Do Something :)
        return true;
    }

}
