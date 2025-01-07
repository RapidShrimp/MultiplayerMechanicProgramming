using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;


public class UI_LobbyMenu : NetworkBehaviour
{
    public event Action OnStartGame;

    [SerializeField] private TextMeshPro PlayerCounter;
    [SerializeField] private Button m_StartButton;
    
    protected bool bCanStartGame;
    private void OnEnable()
    {
        SetCanStartGame(false);
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsServer)
        {
            m_StartButton.onClick.AddListener(() =>
            {
                OnStartGame?.Invoke();
            });
        }

    }
    public void SetCanStartGame(bool CanStart)
    {
        bCanStartGame = CanStart;
        if (!IsServer)
        {
            m_StartButton.interactable = false;
            return;
        }
        
        m_StartButton.interactable = bCanStartGame;
    }

    public void UpdatePlayerCount(int Players) 
    {
        if (!PlayerCounter) { return; }
        PlayerCounter.text = $"Players In Lobby:{Players}";
    }
}
