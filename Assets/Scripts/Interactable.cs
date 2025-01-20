using UnityEngine;
using UnityEngine.InputSystem;

public interface IInteractable
{
    public bool OnClick();
    public bool OnDrag(Vector3 WorldPos);

}
