using TMPro;
using Unity.Netcode;
using UnityEngine;

public class UI_PlayerIdentifier : NetworkBehaviour
{
    TextMeshProUGUI PlayerText;
    private void Awake()
    {
        PlayerText = GetComponent<TextMeshProUGUI>();
    }


    [Rpc(SendTo.Everyone)]
    public void InitPlayerIdentifier_Rpc(int PlayerID)
    {
        if (IsOwner) 
        { 
            PlayerText.text = $"Player {PlayerID} (YOU)"; 
        }
        else
        {
            PlayerText.text = $"Player {PlayerID}";
        }

    }
}