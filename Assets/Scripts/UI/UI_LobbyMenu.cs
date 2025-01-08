using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;


public class UI_LobbyMenu : NetworkBehaviour
{
    public event Action OnStartGame;

    [SerializeField] private Button m_StartButton;
    private TextMeshProUGUI StartButtonText;
    [SerializeField] private Animator Throbber;
    



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
                StartButtonText.text = "Start Game";
                Throbber.SetBool("IsWaiting", false);
            }
            else
            {
                if (IsServer)
                {
                    Throbber.SetBool("IsWaiting", true);
                }
                StartButtonText.text = "Waiting for Host to Start...";
            }
        }
        else
        {
            //ThrobberImage.sprite = ThrobberSprites[0];
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

    }
}
