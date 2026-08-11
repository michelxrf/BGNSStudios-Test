using UnityEngine;

/// <summary>
/// Defines the data structure for npc dialogues
/// </summary>
[CreateAssetMenu(fileName = "NpcDialogueData", menuName = "NPCs/NpcDialogueData", order = 1)]
public class NpcDialogueData : ScriptableObject
{
    public string npcName;
    public string npcText;
    public AudioClip npcVoice;
}
