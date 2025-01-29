using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class UI_Game : NetworkBehaviour
{
    Camera cam;


    [SerializeField] RenderTexture DesiredRenderTo;
    [SerializeField] bool RenderAuto = false;
    [SerializeField] TextMeshPro ScoreCounter;
    public Camera GetUICamera()
    {
        return cam; 
    }
    private void Awake()
    {
        cam = GetComponent<Canvas>().worldCamera;
        cam.enabled = false;
        ToggleActiveRender(RenderAuto);
    }

    public void ToggleActiveRender(bool Active)
    {
        if (DesiredRenderTo == null) { return; }

        cam.targetTexture = Active ? DesiredRenderTo : null;
        cam.enabled = Active;
    }

    public override void OnNetworkSpawn()
    {
        ChangeScore_Rpc(GetInstanceID());
    }

    public override void OnNetworkDespawn()
    {
    }

    [Rpc(SendTo.Everyone)]
    public void ChangeScore_Rpc(int NewScore)
    {
        if (ScoreCounter == null) { Debug.Assert(false, "No Score Counter"); return; }

        ScoreCounter.text = NewScore.ToString();
    }

}
