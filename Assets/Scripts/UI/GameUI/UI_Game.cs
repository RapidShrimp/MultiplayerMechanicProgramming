using TMPro;
using Unity.Netcode;
using UnityEngine;

public class UI_Game : NetworkBehaviour
{
    Camera cam;
    TextMeshPro TestText;
    [SerializeField] RenderTexture DesiredRenderTo;
    public Camera GetUICamera()
    {
        return cam; 
    }
    private void Awake()
    {
        cam = GetComponent<Canvas>().worldCamera;
        cam.enabled = false;
        TestText = GetComponentInChildren<TextMeshPro>();
        if(TestText == null) { return; }
        TestText.text = Random.Range(0, 500).ToString();


    }

    public void ToggleActiveRender(bool Active)
    {
        if (DesiredRenderTo == null) { return; }

        cam.targetTexture = DesiredRenderTo;
        cam.enabled = true;
    }
    public override void OnNetworkSpawn()
    {
    }

    public override void OnNetworkDespawn()
    {
    }

}
