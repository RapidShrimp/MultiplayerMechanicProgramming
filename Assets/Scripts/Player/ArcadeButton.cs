using System;
using System.Collections;
using System.Drawing;
using UnityEngine;

public class ArcadeButton : MonoBehaviour, IInteractable
{
    public event Action<int,bool> OnButtonPressed;
    protected int buttonIndex = 0;
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

    public bool OnDrag(Vector3 WorldPos)
    {
        return true;
    }

    IEnumerator HoldingCheck()
    {
        yield return new WaitForSeconds(0.5f);
        OnButtonPressed?.Invoke(buttonIndex,false);
    }
}
