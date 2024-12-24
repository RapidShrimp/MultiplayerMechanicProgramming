using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using System;

public class NetworkManagerUI : MonoBehaviour
{

    //private Button m_ServerButton;
    [SerializeField] private Button m_HostButton;
    [SerializeField] private Button m_ClientButton;

    [SerializeField] private Button m_StartButton;

    [SerializeField] private GameObject HostSelectScreen;
    [SerializeField] private GameObject LobbyScreen;

    private void Awake()
    {

        m_StartButton.enabled = false;
        m_StartButton.gameObject.SetActive(false);

        PlayerManager m_PlayerManager = FindAnyObjectByType<PlayerManager>();
        m_PlayerManager.OnStartConditionUpdated += Handle_StartConditionUpdated;

    }

    private void Handle_StartConditionUpdated(bool CanStart)
    {
        if (CanStart)
        {
            m_StartButton.enabled = true;
            m_StartButton.gameObject.SetActive(true);
        }
    }

    void Start()
    {
        //Not Using Dedicated Servers
        /*m_ServerButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartServer();
        });*/
        m_HostButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();
        });
        m_ClientButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartClient();

        });

    }


}
