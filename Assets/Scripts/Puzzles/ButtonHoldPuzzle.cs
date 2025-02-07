using System;
using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ButtonHoldPuzzle : PuzzleModule
{

    [SerializeField] Sprite[] ButtonSprites;
    TextMeshProUGUI OnScreenPromptText;
    Image ButtonImage;

    [SerializeField] NetworkVariable<int> HoldLength = new NetworkVariable<int>(
        value:1,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner);
    
    [SerializeField] NetworkVariable<int> DesiredButtonIndex = new NetworkVariable<int>(
    value: 0,
    NetworkVariableReadPermission.Everyone,
    NetworkVariableWritePermission.Owner);
   
    private bool b_Holding;
    Coroutine CR_HoldButton;

    private void Awake()
    {
        transform.parent.GetComponentInParent<UI_Game>().OnButtonPressRecieved += ButtonHoldPuzzle_OnButtonPressRecieved;
        OnScreenPromptText = GetComponentInChildren<TextMeshProUGUI>();
        ButtonImage = GetComponentInChildren<Image>();
    }

    private void OnEnable()
    {
        HoldLength.OnValueChanged += UIValueChanged;
        DesiredButtonIndex.OnValueChanged += UIValueChanged;
        RequestUIChanges();
    }
    private void OnDisable()
    {
        HoldLength.OnValueChanged -= UIValueChanged;
        DesiredButtonIndex.OnValueChanged -= UIValueChanged;
    }

    private void UIValueChanged(int previousValue, int newValue)
    {
        RequestUIChanges();
    }

    #region Manage Button Input 
    private void ButtonHoldPuzzle_OnButtonPressRecieved(int button,bool performed)
    {
        if(!isActiveAndEnabled) {return;}
        
        if(button != DesiredButtonIndex.Value) { return; }
        //Incorrect Button - Stop Hold
        b_Holding = performed;
        if(!b_Holding) 
        {
            ButtonImage.sprite = ButtonSprites[DesiredButtonIndex.Value];
            UpdateUIRender();
            return; 
        }

        //Correct Button pressed
        CR_HoldButton = StartCoroutine(OnHoldingButton());

    }
    #endregion

    public override void StartPuzzleModule()
    {
        base.StartPuzzleModule();
        if (!IsOwner) { return; }
        HoldLength.Value = UnityEngine.Random.Range(1, 6);
        DesiredButtonIndex.Value = UnityEngine.Random.Range(0, 4);
    }

    public override void RequestUIChanges()
    {
        OnUpdateUI();
    }


    public void OnUpdateUI()
    {

        String ButtonColour = "Yellow";
        switch (DesiredButtonIndex.Value)
        {
            case 0:
                ButtonColour = "Yellow";
                break;
            case 1:
                ButtonColour = "Red";
                break;
            case 2:
                ButtonColour = "Green";
                break;
            case 3:
                ButtonColour = "Blue";
                break;

            default: 
                return;
        }
        OnScreenPromptText.text = $"Hold the {ButtonColour} button for {HoldLength.Value} Seconds";
        ButtonImage.sprite = ButtonSprites[DesiredButtonIndex.Value];
        UpdateUIRender();
    }

    IEnumerator OnHoldingButton()
    {
        ButtonImage.sprite = ButtonSprites[DesiredButtonIndex.Value + 4];
        UpdateUIRender();
        float TimeHeld = 0;
        while (b_Holding)
        {
            TimeHeld += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
        if (TimeHeld >= HoldLength.Value) 
        {
            CompleteModule();
        }
        else
        {
            ErrorPuzzle();
        }
    }
}
