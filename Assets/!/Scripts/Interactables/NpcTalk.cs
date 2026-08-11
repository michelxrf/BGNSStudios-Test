using UnityEngine;

public class NpcTalk : MonoBehaviour
{
    [SerializeField] private NpcDialogueData _npcDialogueData;

    public void Talk()
    {
        // Show the dialogue screen with the NPC's dialogue data
        DialogueManager.instance.StartDialogue(_npcDialogueData);
    }
}
