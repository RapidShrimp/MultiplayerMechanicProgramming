using System;
using System.Xml.Linq;
using UnityEngine;

public class SFX_Item : MonoBehaviour
{
    public event Action<AudioSource,float,float> OnPlaySFX;
    [SerializeField] protected AudioSource[] m_AudioSources;
    public void Awake()
    {
        m_AudioSources = GetComponents<AudioSource>();
        if(m_AudioSources.Length == 0) 
        {
            Debug.LogError($"No Audio Source in {gameObject.name}");
            return; 
        }
    }

    public void PlaySFX(int Key = 0, string ByName = null, float volume = 0.5f, float pitch = 1)
    {
        Key = Mathf.Clamp(Key,0, m_AudioSources.Length-1);
        AudioSource DesiredSource = m_AudioSources[Key];
        if (ByName != null) 
        { 
            AudioSource audioSource = FindAudioByName(ByName);
            if (audioSource != null)
            {
                DesiredSource = audioSource;
            }
        }
        OnPlaySFX?.Invoke(DesiredSource,volume,pitch);
    }

    public AudioSource FindAudioByName(string name)
    {
        AudioSource DesiredSource = m_AudioSources[0]; //Fallback Audio Source
        foreach (AudioSource source in m_AudioSources)
        {
            if (source.resource.name.Contains(name))
            {
                DesiredSource = source;
                break;
            }
        }
        return DesiredSource;
    }
}
