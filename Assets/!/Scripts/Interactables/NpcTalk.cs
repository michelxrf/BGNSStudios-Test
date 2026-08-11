using System.Collections;
using UnityEngine;

/// <summary>
/// Npc component that holds dialogue logic for the player to interact with
/// </summary>
public class NpcTalk : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private NpcDialogueData _npcDialogueData;

    private Animator _animator;
    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _animator = GetComponentInChildren<Animator>();
    }

    /// <summary>
    /// Called by the player to init a dialogue with this npc character
    /// </summary>
    /// <param name="playerTransform"></param>
    public void Talk(Transform playerTransform)
    {
        if( _npcDialogueData == null )
        {
            Debug.LogWarning($"No dialogue data assigned to {gameObject.name}");
            return;
        }

        // Rotate toward player in y axis
        Vector3 direction = (playerTransform.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);

        // Show the dialogue screen with the NPC's dialogue data
        DialogueManager.instance.StartDialogue(_npcDialogueData, this);

        // Play the NPC's voice if it exists
        if (_npcDialogueData.npcVoice != null)
        {
            _audioSource.PlayOneShot(_npcDialogueData.npcVoice);    
        }
        
        // play talk anim
        _animator.SetBool("IsTalking", true);


    }

    /// <summary>
    /// stops talking anim
    /// </summary>
    public void StopTalkingAnim()
    {
        _animator.SetBool("IsTalking", false);
    }
}
