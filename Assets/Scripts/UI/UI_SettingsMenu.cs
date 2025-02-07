using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_SettingsMenu : MonoBehaviour
{
    public event Action<GameSettings> OnSettingsChanged;
    GameSettings m_Settings;
    [SerializeField] TextMeshProUGUI m_ApplySettingsText;
    [SerializeField] TextMeshProUGUI m_TimeText;
    private void Awake()
    {
        m_Settings = new GameSettings(true,true,true,120);
    }

    public void TimeChange(int TimerIncrease)
    {
        m_Settings.GameTime = Mathf.Clamp(m_Settings.GameTime + TimerIncrease, 60, 300);
        m_TimeText.text = $"Time:{m_Settings.GameTime}";
        m_ApplySettingsText.text = "Apply Settings";
        OnSettingsChanged?.Invoke(m_Settings);

    }

    public void ToggleConfigsForPuzzle(Image TickboxImage)
    {
        m_Settings.ConfigurationRequired = !m_Settings.ConfigurationRequired;
        TickboxImage.enabled = m_Settings.ConfigurationRequired;
        m_ApplySettingsText.text = "Apply Settings";
        OnSettingsChanged?.Invoke(m_Settings);

    }

    public void ToggleSabotageScoring(Image TickBoxImage)
    {
        m_Settings.SabotageScoring = !m_Settings.SabotageScoring;
        TickBoxImage.enabled = m_Settings.SabotageScoring;
        m_ApplySettingsText.text = "Apply Settings";
        OnSettingsChanged?.Invoke(m_Settings);

    }

    public void ToggleConfigurationScramble(Image TickboxImage)
    {
        m_Settings.ScrambleConfiguration = !m_Settings.ScrambleConfiguration;
        TickboxImage.enabled = m_Settings.ScrambleConfiguration;
        m_ApplySettingsText.text = "Apply Settings";
        OnSettingsChanged?.Invoke(m_Settings);

    }

    public void OnSettingsApply()
    {
        m_ApplySettingsText.text = "Settings Applied";
        OnSettingsChanged?.Invoke(m_Settings);
    }
}
