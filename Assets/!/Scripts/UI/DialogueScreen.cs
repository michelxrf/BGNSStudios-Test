using TMPro;
using UnityEngine;

/// <summary>
/// Handles the dialogue screen UI for NPC interactions
/// </summary>
public class DialogueScreen : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text _npcNameText;
    [SerializeField] private TMP_Text _npcTextText;
    
    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        // initializes the Manager with the refs for this UI screen, so it can update it's content
        DialogueManager.instance.SetInteractionScreen(this);
        
        // Hides the screen skipping its animation
        Hide(true);
    }

    private void Update()
    {
        if (PauseManager.instance.IsPaused) return;
        if (InventorySystem.instance.IsInventoryOpen) return;

        if (DialogueManager.instance.IsDialogueActive)
        { 
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Space)
                || Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Return))
            {
                DialogueManager.instance.EndDialogue();
            }
        }
    }

    /// <summary>
    /// Show the screen and init the text fields with the given NPC data
    /// </summary>
    /// <param name="npcData"></param>
    public void Show(NpcDialogueData npcData)
    {
        // set text
        _npcNameText.text = npcData.npcName;
        _npcTextText.text = npcData.npcText;

        // set popup effects
        transform.localScale = Vector3.zero;
        LeanTween.alphaCanvas(_canvasGroup, 1f, 0.5f).setEase(LeanTweenType.easeInOutQuad);
        LeanTween.scale(gameObject, Vector3.one, 0.5f).setEase(LeanTweenType.easeInOutQuad);

        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;
    }

    /// <summary>
    /// Hides the screen
    /// </summary>
    /// <param name="skipAnimation"></param>
    public void Hide(bool skipAnimation = false)
    {
        if(!skipAnimation)
        {
            // popup effect
            transform.localScale = Vector3.one;
            LeanTween.alphaCanvas(_canvasGroup, 0f, 0.5f).setEase(LeanTweenType.easeInOutQuad);
            LeanTween.scale(gameObject, Vector3.zero, 0.5f).setEase(LeanTweenType.easeInOutQuad);
        }
        else
        {
            _canvasGroup.alpha = 0f;
        }

        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
    }
}