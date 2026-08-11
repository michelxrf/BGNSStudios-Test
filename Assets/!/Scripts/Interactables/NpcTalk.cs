using System.Collections;
using UnityEngine;

/// <summary>
/// allow the player to talk to an NPC
/// </summary>
public class NpcTalk : MonoBehaviour
{
    [SerializeField] private NpcDialogueData _npcDialogueData;

    private Animator _animator;
    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _animator = GetComponentInChildren<Animator>();
    }

    public void Talk()
    {
        if( _npcDialogueData != null )
        {
            Debug.LogWarning($"No dialogue data assigned to {gameObject.name}");
            return;
        }
        
        // Show the dialogue screen with the NPC's dialogue data
        DialogueManager.instance.StartDialogue(_npcDialogueData, this);

        // Play the NPC's voice if it exists
        if (_npcDialogueData.npcVoice != null)
        {
            _audioSource.PlayOneShot(_npcDialogueData.npcVoice);    
        }
        
        // play talk anim
        _animator.SetBool("Talk", true);
    }

    /// <summary>
    /// stops talking anim
    /// </summary>
    public void StopTalkingAnim()
    {
        _animator.SetBool("Talk", true);
    }
}
