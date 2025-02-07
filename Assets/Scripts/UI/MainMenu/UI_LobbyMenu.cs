using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;


public class UI_LobbyMenu : MonoBehaviour
{
    public event Action OnUIRequestStartGame;

    [SerializeField] private Button m_StartButton;
    private TextMeshProUGUI StartButtonText;
    [SerializeField] private Animator Throbber;
    [SerializeField] protected int RequiredPlayersToStart = 2;

    protected SFX_Item Audios;

    public bool IsHost = false;

    private void Awake()
    {
        StartButtonText = GetComponentInChildren<TextMeshProUGUI>(m_StartButton);
        Audios = GetComponentInChildren<SFX_Item>();

        m_StartButton.onClick.AddListener(() =>
        {
            OnUIRequestStartGame?.Invoke();
            SFX_AudioManager.Singleton.PlaySoundToPlayer(Audios.FindAudioByName("StartGame"));
        });

        SetCanStartGame(false);
    }
    protected void SetCanStartGame(bool CanStart)
    {
        m_StartButton.interactable = CanStart;

        if (!CanStart)
        {
            Throbber.SetBool("IsWaiting", true);
            StartButtonText.text = "Waiting for Players...";
        }

        if (IsHost) 
        {
            StartButtonText.text = "Start Game";
            Throbber.SetBool("IsWaiting", false);
        }
        else
        {
            StartButtonText.text = "Waiting for Host to Start...";
        }

    }

    public void UpdatePlayerCount(int Players) 
    {
        if (Players >= RequiredPlayersToStart)
        {
            SetCanStartGame(true);
        }
        else
        {
            SetCanStartGame(false);
        }

    }
}
