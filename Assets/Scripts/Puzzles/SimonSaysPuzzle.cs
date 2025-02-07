using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;


public class SimonSaysPuzzle : PuzzleModule
{

    Coroutine DisplaySequence;

    [SerializeField] List<int> CorrectSequence = new List<int>();
    [SerializeField] List<int> Curr_Sequence = new List<int>();
    [SerializeField] Image[] ProgressSprites;
    SimonSaysLight[] m_Lights;

    private void Awake()
    {
        GetComponentInParent<UI_Game>().OnButtonPressRecieved += OnButtonPressed;
        m_Lights = GetComponentsInChildren<SimonSaysLight>();
        ProgressSprites = GetComponentInChildren<HorizontalLayoutGroup>().GetComponentsInChildren<Image>();
    }


    public override void StartPuzzleModule()
    {
        base.StartPuzzleModule();
        if (!IsOwner) { return; }
        int[] Sequence = new int[5];
        for (int i = 0; i < Sequence.Length; i++)
        {
            Sequence[i] = UnityEngine.Random.Range(0, 4);
        }
        OnCorrectSequenceUpdated_Rpc(Sequence);
        Curr_Sequence.Clear();
        UpdateProgress_Rpc(MakeIntArrayfromList(Curr_Sequence));
    }

    [Rpc(SendTo.Everyone)]
    public void OnCorrectSequenceUpdated_Rpc(int[] Array)
    {
        CorrectSequence.Clear();
        for (int i = 0; i < Array.Length; i++) 
        { 
            //Debug.Log(Array[i]);
            CorrectSequence.Add(Array[i]);
        }
        Curr_Sequence.Clear();
        DisplaySequence = StartCoroutine(ShowSimonSaysSequence());
        if (!isActiveAndEnabled) { return; }
    }

    public override void FailModule()
    {
        base.FailModule();
        Curr_Sequence.Clear();
    }
    private void OnButtonPressed(int ButtonIndex, bool Performed)
    {
        if(!isActiveAndEnabled || !Performed) { return; }

        if (CorrectSequence[Curr_Sequence.Count] != ButtonIndex) 
        {
            ErrorPuzzle();
            Curr_Sequence.Clear();
            UpdateProgress_Rpc(MakeIntArrayfromList(Curr_Sequence));
            return;
        }
        Curr_Sequence.Add(ButtonIndex);
        PuzzleAudios.PlaySFX(0, "SimonSays", 0.5f, (ButtonIndex / 2) + 0.5f);
        UpdateProgress_Rpc(MakeIntArrayfromList(Curr_Sequence));
        if (Curr_Sequence.Count == 5) { CompleteModule(); }

    }

    IEnumerator ShowSimonSaysSequence()
    {
        while (true)
        {
            //For Traditional Simon Says - Out of Scope
            /*yield return new WaitForSeconds(2);
            for (int i = 0; i <= Curr_Sequence.Count; i++)
            {
                yield return new WaitForSeconds(0.1f);
                m_Lights[CorrectSequence[i]].OnLightToggle(true);
                UpdateUIRender();
                yield return new WaitForSeconds(0.2f);
                m_Lights[CorrectSequence[i]].OnLightToggle(false);
                UpdateUIRender();
            }*/

            yield return new WaitForSeconds(2);
            for (int i = 0; i <= CorrectSequence.Count-1; i++)
            {
                yield return new WaitForSeconds(0.3f);
                m_Lights[CorrectSequence[i]].OnLightToggle(true);
                UpdateUIRender();
                PuzzleAudios.PlaySFX(0, "SimonSays", 0.5f, ((float)CorrectSequence[i]/2)+0.5f);
                yield return new WaitForSeconds(0.3f);
                m_Lights[CorrectSequence[i]].OnLightToggle(false);
                UpdateUIRender();
            }
        }
    }

    [Rpc(SendTo.Everyone)]
    public void UpdateProgress_Rpc(int[] CurrentProgress)
    {
        if(!IsOwner)
        {
            Curr_Sequence = MakeListFromIntArray(CurrentProgress);
        }

        for (int i = 0; i < 5; i++)
        {
            if (i<Curr_Sequence.Count) 
            {
                Color DesiredColour = Color.white;
                switch (Curr_Sequence[i])
                {
                    case 0:
                        DesiredColour = Color.yellow;
                        break;
                    case 1:
                        DesiredColour = Color.red;
                        break;
                    case 2:
                        DesiredColour = Color.green;
                        break;
                    case 3: 
                        DesiredColour = Color.blue;
                        break;
                }
                ProgressSprites[i].color = DesiredColour;
            }
            else
            {
                ProgressSprites[i].color = Color.white;
            }
        }
    }

    public override void RequestUIChanges()
    {

    }

    int[] MakeIntArrayfromList(List<int> list)
    {
        int[] ints = new int[list.Count];
        for (int i = 0; i < ints.Length; i++)
        {
            ints[i] = list[i];
        }

        return ints;
    
    }

    List<int> MakeListFromIntArray(int[] ints)
    {
        List<int> list = new List<int>();

        for (int i = 0;i < ints.Length; i++)
        {
            list.Add(ints[i]);
        }
        return list;
    }
    
}