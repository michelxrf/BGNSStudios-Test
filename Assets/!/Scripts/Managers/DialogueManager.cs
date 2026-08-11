using System;
using Unity.AppUI.UI;
using UnityEngine;

/// <summary>
/// Handles dialogue logic
/// </summary>
public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;

    public bool IsDialogueActive { get; private set; } = false;
    private DialogueScreen _dialogScreen;

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

    public void StartDialogue(NpcDialogueData dialogue)
    {
        IsDialogueActive = true;
        _dialogScreen.Show(dialogue);
    }

    public void EndDialogue()
    {
        _dialogScreen.Hide();
        IsDialogueActive = false;
    }
}
