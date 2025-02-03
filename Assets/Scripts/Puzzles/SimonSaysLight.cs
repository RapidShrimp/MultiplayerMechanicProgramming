using UnityEngine;
using UnityEngine.UI;

public class SimonSaysLight : MonoBehaviour
{

    protected Image LightSprite;
    private void Awake()
    {
        LightSprite = GetComponent<Image>();
        OnLightToggle(false);
    }

    public void OnLightToggle(bool State)
    {
        LightSprite.color = State ? Color.white :  Color.gray;
    }
}
