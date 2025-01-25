using Unity.Netcode;
using UnityEngine;

public class UI_Game : NetworkBehaviour
{
    Camera cam;

    public override void OnNetworkSpawn()
    {
        cam = GetComponentInChildren<Camera>();
    }

    public override void OnNetworkDespawn()
    {
    }

}
