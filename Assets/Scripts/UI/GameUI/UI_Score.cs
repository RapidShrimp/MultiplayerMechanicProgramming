using TMPro;
using Unity.Netcode;
using UnityEngine;

public class UI_Score : MonoBehaviour
{
    [SerializeField] protected TextMeshProUGUI ScoreCounter;

    private void Awake()
    {        
        ScoreCounter = GetComponent<TextMeshProUGUI>();
        if (ScoreCounter != null) Debug.Log("cry");
        else Debug.Log(ScoreCounter.name);
    }

    [Rpc(SendTo.Everyone)]
    public void ChangeScore_Rpc(int NewScore)
    {
        Debug.Log("We got to the UI Score Script");
        if (ScoreCounter == null) { Debug.Assert(false, "No Score Counter"); return; }

        ScoreCounter.text = NewScore.ToString();
    }
}
