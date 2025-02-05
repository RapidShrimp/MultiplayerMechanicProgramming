using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class MusicManager : NetworkBehaviour
{
    [SerializeField] protected AudioSource MainMenuMusic;
    [SerializeField] protected AudioSource GameplayMusic;

    private void Start()
    {
        SFX_AudioManager.Singleton.SwapToMusic(MainMenuMusic, 2, 0.5f);
        GameManager.OnReadyGame += () =>
        {
            SFX_AudioManager.Singleton.SwapToMusic(GameplayMusic,0.2f,1,0.2f);
        };        
    }

}
