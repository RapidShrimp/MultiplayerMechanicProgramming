using TMPro;
using Unity.Netcode;
using UnityEngine;

public class UI_Score : MonoBehaviour
{
    protected TextMeshProUGUI ScoreCounter;

    private void Awake()
    {        
        ScoreCounter = GetComponent<TextMeshProUGUI>();
    }

    [Rpc(SendTo.Everyone)]
    public void ChangeScore_Rpc(int NewScore)
    {
        ScoreCounter.text = $"Score: {NewScore.ToString()}";
    }
}
