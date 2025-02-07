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

    [SerializeField] SettingButton m_SabotageScoring;
    [SerializeField] SettingButton m_ScrambleConfiguration;
    [SerializeField] SettingButton m_ConfigurationRequired;

    private void Awake()
    {
    }
    public void OnLoadSettings(GameSettings loadedSettings)
    {
        m_Settings = loadedSettings;

        m_SabotageScoring.LoadSetting(m_Settings.SabotageScoring);
        m_SabotageScoring.OnSettingToggled += ToggleSabotageScoring;

        m_ScrambleConfiguration.LoadSetting(m_Settings.ScrambleConfiguration);
        m_ScrambleConfiguration.OnSettingToggled += ToggleConfigurationScramble;

        m_ConfigurationRequired.LoadSetting(m_Settings.ConfigurationRequired);
        m_ConfigurationRequired.OnSettingToggled += ToggleConfigsForPuzzle;

        TimeChange(0);
    }

    public void TimeChange(int TimerIncrease)
    {
        m_Settings.GameTime = Mathf.Clamp(m_Settings.GameTime + TimerIncrease, 60, 300);
        m_TimeText.text = $"Time: {m_Settings.GameTime}";
    }

    public void ToggleConfigsForPuzzle(bool OptionState)
    {
        m_Settings.ConfigurationRequired = OptionState;
    }

    public void ToggleSabotageScoring(bool OptionState)
    {
        m_Settings.SabotageScoring = OptionState;
    }

    public void ToggleConfigurationScramble(bool OptionState)
    {
        m_Settings.ScrambleConfiguration = OptionState;
    }

    public void OnSettingsApply()
    {
        m_ApplySettingsText.text = "Settings Applied";
        OnSettingsChanged?.Invoke(m_Settings);
    }


}
