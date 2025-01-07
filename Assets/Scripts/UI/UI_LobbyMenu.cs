using System;
using UnityEngine;
using UnityEngine.UI;


public class UI_LobbyMenu : MonoBehaviour
{
    [SerializeField] private Button m_StartButton;
    public event Action OnStartGame;
    protected bool bCanStartGame;
    public void SetCanStartGame(bool CanStart)
    {
        bCanStartGame = CanStart;
        UpdateStartButtonUI();
    }
    void Start()
    {
        m_StartButton.onClick.AddListener(() =>
        {
            OnStartGame?.Invoke();
        });
        SetCanStartGame(false);
    }

    protected void UpdateStartButtonUI()
    {
        if (bCanStartGame)
        {
            m_StartButton.interactable = true;
        }
        else
        {
            m_StartButton.interactable = false;
        }
    }
}
