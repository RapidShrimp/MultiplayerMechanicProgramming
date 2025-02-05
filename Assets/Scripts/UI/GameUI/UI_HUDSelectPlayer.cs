using System;
using UnityEngine;
using UnityEngine.UI;

public class UI_HUDSelectPlayer : MonoBehaviour
{
    public event Action<int> OnSelectPlayer; //1 = Right // -1 = Left

    [SerializeField] private Button UI_LeftButton;
    [SerializeField] private Button UI_RightButton;

    private void Awake()
    {
        if(UI_LeftButton == null || UI_RightButton == null) { return; }
        UI_LeftButton.onClick.AddListener(() => { OnSelectPlayer?.Invoke(-1); });
        UI_RightButton.onClick.AddListener(() => { OnSelectPlayer?.Invoke(1); });
    }

    void OnGameEnd(bool PlayerWin)
    {
        if (PlayerWin) 
        { 
            //Confetti Particles
        }

    }
}
