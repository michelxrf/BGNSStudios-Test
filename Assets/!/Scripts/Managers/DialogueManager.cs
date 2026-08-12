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
