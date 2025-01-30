using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class UI_Game : UI_RenderTarget
{
    protected UI_Score ScoreCounter;
    public Camera GetUICamera()
    {
        return cam; 
    }
    private void Awake()
    {
        cam = GetComponent<Canvas>().worldCamera;
        ScoreCounter = GetComponentInChildren<UI_Score>();
        cam.enabled = false;
        ToggleActiveRender(RenderAuto);
    }

    public override void OnNetworkSpawn()
    {
        ScoreCounter.ChangeScore_Rpc(GetInstanceID());
    }

    public override void OnNetworkDespawn()
    {
    }



}
