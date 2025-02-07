using System;
using System.Collections;
using System.Drawing;
using Unity.Netcode;
using UnityEngine;

public class ArcadeButton : NetworkBehaviour, IInteractable
{
    public event Action<int,bool> OnButtonPressed;
    Coroutine CR_Holding;
    protected int buttonIndex = 0;
    bool LastInput = false;
    public void InitButton(UnityEngine.Color Colour, int ButtonIndex) 
    { 
        buttonIndex = ButtonIndex;
        MeshRenderer[] Renderers = transform.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer renderer in Renderers)
        {
            renderer.material.color = Colour;
        }
    }
    public bool OnClick()
    {
        return false;
    }

    public bool OnDrag(Vector3 WorldPos, bool IsHeld)
    {
        if (!IsOwner) { return false; }
        if(IsHeld == LastInput) { return true; }

        if (!IsHeld)
        {
            //Exit button inputs
            LastInput = IsHeld;
            OnButtonPressed?.Invoke(buttonIndex, false);
            return false;
        }

        LastInput = IsHeld;
        OnButtonPressed?.Invoke(buttonIndex, true);
        return true;
    }

    [Rpc(SendTo.Everyone)]
    public void PressButtonVisual_Rpc(bool Performed)
    {
        GameObject ButtonPress = transform.GetChild(0).gameObject;
        if (!Performed) 
        { 
            ButtonPress.transform.localPosition = Vector3.zero;
        }
        else
        {
            transform.GetChild(0).transform.localPosition = new Vector3(0, -0.01f);
        }

    }
}
