using System;
using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class GameTimer : NetworkBehaviour
{
    NetworkVariable<float> GameTimeRemaining = new NetworkVariable<float>(
        value: 120.0f,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
        );

    protected TextMeshPro GameTimeRemainingUI;

    private void Awake()
    {
        GameTimeRemainingUI = GetComponentInChildren<TextMeshPro>();
    }
    public override void OnNetworkSpawn()
    {
        GameTimeRemaining.OnValueChanged += (float previousValue, float newValue) =>
        {
            GameTimeRemainingUI.text = newValue.ToString();
        };
    }

    IEnumerator ChangeGameTimer()
    {
        while (GameTimeRemaining.Value > 0)
        {
            yield return new WaitForSecondsRealtime(1);
        }
    }
}
