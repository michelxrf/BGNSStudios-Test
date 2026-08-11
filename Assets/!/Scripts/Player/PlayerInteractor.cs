using UnityEngine;
using UnityEngine.InputSystem;


/// <summary>
/// Handles the player's interaction with objects in the game world
/// </summary>
public class PlayerInteractor : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _origin;
    [SerializeField] private GameObject _interactionTip;

    [Header("Interaction Settings")]
    [SerializeField] private float _range;

    private PlayerInput _playerInput;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
    }

    private void Update()
    {
        /// Prevent movement while in inventory
        if (InventorySystem.instance.IsInventoryOpen) return;

        // prevent movement while paused
        if (PauseManager.instance.IsPaused) return;

        // prevent movement while in dialogue
        if (DialogueManager.instance.IsDialogueActive) return;

        // Perform raycast from origin in forward direction
        Ray ray = new Ray(_origin.position, _origin.forward);
        bool hit = Physics.Raycast(ray, out RaycastHit hitInfo, _range);

        CollectableObject collectableObject = hitInfo.collider?.GetComponent<CollectableObject>();
        NpcTalk npc = hitInfo.collider?.GetComponent<NpcTalk>();

        if (hit && (collectableObject != null || npc != null))
        {
            _interactionTip.SetActive(true);

            // Check for Interact action and call Interact
            if (collectableObject != null && _playerInput.actions["Interact"].triggered)
            {
                _interactionTip.SetActive(false);
                collectableObject.Interact();
                return;
            }

            if (npc != null && _playerInput.actions["Interact"].triggered)
            {
                _interactionTip.SetActive(false);
                npc.Talk();
                return;
            }
        }
        else
        {
            _interactionTip.SetActive(false);
        }
    }
}
