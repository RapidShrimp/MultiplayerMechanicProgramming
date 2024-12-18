using TMPro;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(NetworkObject))]
public class PlayerManager : NetworkBehaviour
{
    [SerializeField] private NetworkVariable<int> PlayerCount = new NetworkVariable<int>(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);
 
    private TextMeshProUGUI PlayerCountUI;

    private void Awake()
    {
        PlayerCountUI = GetComponentInChildren<TextMeshProUGUI>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += (id) =>
            {
                PlayerCount.Value++;
            };
            NetworkManager.Singleton.OnClientDisconnectCallback += (id) =>
            {
                PlayerCount.Value--;
            };
            PlayerCount.OnValueChanged += (int previousValue, int newValue) =>
            {
                Debug.Log($"Player count is {PlayerCount.Value}");
                PlayerCountUI.text = $"Player Count: {PlayerCount.Value}";
            };
        }
        else if(IsClient) 
        {
            PlayerCount.OnValueChanged += (int PreviousValue, int newValue) =>
            {
                PlayerCountUI.text = $"Player Count: {PlayerCount.Value}";
            };
        }
    }
}