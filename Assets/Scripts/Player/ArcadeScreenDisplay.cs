using UnityEngine;

public class ArcadeScreenDisplay : MonoBehaviour
{

    [SerializeField] Material RenderTargetMaterial;
    [SerializeField] Material BlankScreenMaterial;

    public void ToggleScreenActive_Rpc(bool active)
    {
        Material Assign = active ? RenderTargetMaterial : BlankScreenMaterial;
        GetComponent<MeshRenderer>().material = Assign;
    }
}
