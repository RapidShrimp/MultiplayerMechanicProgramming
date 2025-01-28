using Unity.Netcode;
using UnityEngine;

public class UI_Game : NetworkBehaviour
{
    Camera cam;
    private void Awake()
    {
        cam = GetComponent<Canvas>().worldCamera;

    }
    public override void OnNetworkSpawn()
    {
    }

    public override void OnNetworkDespawn()
    {
    }

}
