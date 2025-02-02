using System.Collections;
using UnityEngine;

public class LoopingVisibility : MonoBehaviour
{
    [SerializeField] GameObject Object;
    [SerializeField] float LoopingTime = 0.5f;
    
    private void Awake()
    {
        if(Object == null) {return;}
        StartCoroutine(LoopVisibility());
    }

    IEnumerator LoopVisibility()
    {
        yield return new WaitForSeconds(Random.Range(0,1.0f));
        while (true)
        {
            yield return new WaitForSecondsRealtime(LoopingTime);
            Object.SetActive(false);
            yield return new WaitForSecondsRealtime(LoopingTime);
            Object.SetActive(true);
        }
    }
}
