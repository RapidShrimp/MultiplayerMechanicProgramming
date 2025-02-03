using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class UI_RenderTarget : NetworkBehaviour
{
    protected Camera cam;
    [SerializeField] protected RenderTexture DesiredRenderTo;
    [SerializeField] protected bool RenderAuto = false;
    private void Awake()
    {
        cam = GetComponent<Canvas>().worldCamera;
        cam.enabled = false;
        ToggleActiveRender(RenderAuto);
    }

    public virtual void ToggleActiveRender(bool Active)
    {
        if (DesiredRenderTo == null) { return; }

        cam.targetTexture = Active ? DesiredRenderTo : null;
        cam.enabled = Active;
    }
}
