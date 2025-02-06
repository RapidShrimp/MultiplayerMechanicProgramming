using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] protected AudioSource MainMenuMusic;
    [SerializeField] protected AudioSource GameplayMusic;

    private void Start()
    {
        SFX_AudioManager.Singleton.SwapToMusic(MainMenuMusic, 2, 0.5f);
        GameManager.OnLoadingLevel += (Time) =>
        {
            SFX_AudioManager.Singleton.SwapToMusic(GameplayMusic,Time/2,Time/2,0.1f,Time/2);
        };        
    }

}
