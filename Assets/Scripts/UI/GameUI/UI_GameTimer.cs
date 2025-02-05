using TMPro;
using Unity.Netcode;
using UnityEngine;

public class UI_GameTimer : MonoBehaviour
{
    protected TextMeshProUGUI GameTimerText;

    private void Awake()
    {
        GameTimerText = GetComponent<TextMeshProUGUI>();
    }
    private void OnEnable()
    {
        GameManager.OnGameTimerUpdated += UpdateGameTimer;

    }

    private void OnDisable()
    {
        GameManager.OnGameTimerUpdated -= UpdateGameTimer;
    }

    [Rpc(SendTo.Everyone)]
    void UpdateGameTimer(int NewTime)
    {
        if (!isActiveAndEnabled) { return; }
        GameTimerText.text = NewTime.ToString();
    }
}
