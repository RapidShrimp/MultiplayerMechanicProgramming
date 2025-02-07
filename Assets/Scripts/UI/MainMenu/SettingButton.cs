using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingButton : MonoBehaviour
{
    public event Action<bool> OnSettingToggled;
    [SerializeField] protected Image TickBoxImage;
    protected TextMeshProUGUI m_SettingText;
    protected Button m_Button;
    private bool ActiveSetting;
   

    public void Awake()
    {
        m_Button = GetComponentInChildren<Button>();
        m_SettingText = GetComponentInChildren<TextMeshProUGUI>();
        if(m_Button == null) { return; }

        m_Button.onClick.AddListener(() => { OnClicked(); });
    }

    public void LoadSetting(bool IsActive)
    {
        ActiveSetting = IsActive;
        SetTickBoxState(ActiveSetting);
    }

    public void OnClicked() 
    {
        ActiveSetting = !ActiveSetting;
        Debug.Log($"IsButtonActive {ActiveSetting}");
        SetTickBoxState(ActiveSetting);
        OnSettingToggled?.Invoke(ActiveSetting);
    }

    public void SetTickBoxState(bool IsActive)
    {
        if (TickBoxImage == null) { return; }
        TickBoxImage.enabled = IsActive;
    }

    public void SetDesiredText(string Text)
    {
        if(m_SettingText == null) { return; }
        m_SettingText.text = Text;
    }
}
