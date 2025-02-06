using System.Collections;
using UnityEngine;

public class ScreenFade : MonoBehaviour
{
    bool isFading = false;
    float FadeAlpha = 0;


    [SerializeField] protected CanvasRenderer FadeCanvas;

    private void Awake()
    {
        FadeCanvas = GetComponentInChildren<CanvasRenderer>();
        StartCoroutine(FadeGUI(0.5f, true));
    }
    public IEnumerator FadeGUI(float FadeTime, bool FadeIn)
    {
        isFading = true;
        float TimeElapsed = 0;
        float FadeFrom = FadeAlpha;
        float FadeTo = FadeIn ? 0 : 1;

        while (TimeElapsed < FadeTime)
        {
            yield return new WaitForFixedUpdate();
            TimeElapsed += Time.deltaTime;
            FadeAlpha = Mathf.Clamp(Mathf.Lerp(FadeFrom,FadeTo,TimeElapsed/FadeTime),0,1);
            FadeCanvas.SetAlpha(FadeAlpha);
        }
        if (FadeIn) { isFading = false; }
        yield return new WaitForEndOfFrame();
    }


}
