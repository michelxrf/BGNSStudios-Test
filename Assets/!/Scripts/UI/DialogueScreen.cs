using TMPro;
using UnityEngine;

/// <summary>
/// Handles the dialogue screen UI for NPC interactions
/// </summary>
public class DialogueScreen : MonoBehaviour
{
    [SerializeField] private TMP_Text _npcNameText;
    [SerializeField] private TMP_Text _npcTextText;
    
    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        DialogueManager.instance.SetInteractionScreen(this);
        Hide();
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

    public void Hide()
    {
        _canvasGroup.alpha = 0f;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
    }
}