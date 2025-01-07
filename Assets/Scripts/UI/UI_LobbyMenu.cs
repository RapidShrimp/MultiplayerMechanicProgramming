using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;


public class UI_LobbyMenu : NetworkBehaviour
{
    public event Action OnStartGame;

    [SerializeField] private TextMeshPro PlayerCounter;
    [SerializeField] private Button m_StartButton;
    [SerializeField] private TextMeshProUGUI StartButtonText;
    
    protected bool bCanStartGame;
    private void OnEnable()
    {
        StartButtonText = GetComponentInChildren<TextMeshProUGUI>(m_StartButton);
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

        if (IsServer)
        {
            m_StartButton.interactable = bCanStartGame;
        }

        if (bCanStartGame)
        {
            if (IsServer) 
            {
                StartButtonText.text = "StartGame";
            }
            else
            {
                StartButtonText.text = "Waiting for Host...";
            }
        }
        else
        {
            m_StartButton.interactable = false;
            StartButtonText.text = "Waiting for Players...";
        }
    }

    public void UpdatePlayerCount(int Players) 
    {
        if (Players >= 2)
        {
            SetCanStartGame(true);
        }
        else
        {
            SetCanStartGame(false);
        }

        if (!PlayerCounter) { return; }
        PlayerCounter.text = $"Players In Lobby:{Players}";
    }
}
