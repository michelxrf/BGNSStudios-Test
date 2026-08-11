using System;
using Unity.AppUI.UI;
using UnityEngine;

/// <summary>
/// Handles dialogue logic, mostly used so other game compoents can be trigger when dialogue happens
/// </summary>
public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance { private set; get; }

    public bool IsDialogueActive { get; private set; } = false;
    private DialogueScreen _dialogScreen;
    private NpcTalk _talkingNpc;

    private void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        instance = this;
    }


    private void Update()
    {
        // Prevent dialogue closing while the game is paused
        if (PauseManager.instance.IsPaused) return;

        // Check for input to close the dialogue screen
        if (IsDialogueActive && Input.GetKeyDown(KeyCode.Space))
        {
            EndDialogue();
        }
    }

    public void SetInteractionScreen(DialogueScreen dialogScreen)
    {
        _dialogScreen = dialogScreen;
    }

    public void StartDialogue(NpcDialogueData dialogue, NpcTalk talkingNpc)
    {
        IsDialogueActive = true;
        _dialogScreen.Show(dialogue);
        _talkingNpc = talkingNpc;
    }

    public void EndDialogue()
    {
        _dialogScreen.Hide();
        IsDialogueActive = false;
        _talkingNpc.StopTalkingAnim();
    }
}
