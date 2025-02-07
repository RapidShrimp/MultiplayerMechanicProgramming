using Unity.Netcode;
using UnityEngine;

public class ToggleOutline : NetworkBehaviour, IHoverable
{

    [SerializeField] Outline[] m_Outlines;
    [SerializeField] protected bool CanOthersPress = true;
    void Awake()
    {
        m_Outlines = GetComponents<Outline>();
        if (m_Outlines.Length == 0) { m_Outlines = GetComponentsInChildren<Outline>(); }
        foreach (Outline Line in m_Outlines)
        {
            Line.enabled = false;
        }
    }

    public void OnHover(bool Hovered)
    {
        if (!CanOthersPress && !IsOwner) { return; }
        foreach (Outline Line in m_Outlines)
        {
            Line.enabled = Hovered;
        }
    }
}
