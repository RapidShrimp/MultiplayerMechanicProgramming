using UnityEngine;

public interface IInteractable
{
    void OnClick(); 
    void OnDoubleClick();
    void OnDragStart();
    void OnDragEnd();
}
