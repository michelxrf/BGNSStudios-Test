using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

/// <summary>
/// Handles inventory screen
/// </summary>
public class InventoryScreenUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ItemSlot[] _itemSlots;
    [SerializeField] private TMP_Text _itemName;
    [SerializeField] private TMP_Text _itemDescription;
    [SerializeField] private Button _useButton;
    [SerializeField] private Button _dropButton;
    [SerializeField] private Transform _dropSpot;
    [SerializeField] private GameObject _usedItemPopUpPrefab;
    [SerializeField] private GameObject _panel;
    [SerializeField] private GameObject _popupSpawnPoint;

    private CanvasGroup _canvasGroup;
    private ItemSlot _selectedSlot;
    private bool _isVisible = false;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        // Hide the inventory screen at the start, skipping the pop up animation
        Hide(true);

        // Subscribe to Inventory System so the UI updates when the inventory changes
        InventorySystem.instance.OnInventoryChanged += RefreshItems;
    }

    private void Update()
    {
        // prevents opening inventory while paused or in dialogue
        if (DialogueManager.instance.IsDialogueActive) return;
        if (PauseManager.instance.IsPaused) return;

        // Toggle inventory screen visibility when the "Inventory" action is triggered
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if(_isVisible)
            {
                Hide();
            }
            else
            {
                Show();
            }
        }
    }

    /// <summary>
    /// Hides the screen
    /// </summary>
    /// <param name="skipAnimation">Used to skip the pop up animation, used to init the menus</param>
    public void Hide(bool skipAnimation = false)
    {
        if(!skipAnimation)
        {
            // popup effect
            _panel.transform.localScale = Vector3.one;
            LeanTween.alphaCanvas(_canvasGroup, 0f, 0.5f).setEase(LeanTweenType.easeInOutQuad);
            LeanTween.scale(_panel, Vector3.zero, 0.5f).setEase(LeanTweenType.easeInOutQuad);
        }
        else
        {
            // Sets the alpha to 0 when skipping the animation
            _canvasGroup.alpha = 0f;
        }

        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.interactable = false;
        _isVisible = false;

        // locks the cursor
        Cursor.lockState = CursorLockMode.Locked;

        // Tells the inventory system that the inventory is closed
        InventorySystem.instance.ToggleInventory(false);
    }

    /// <summary>
    /// Shows the screen
    /// </summary>
    /// <param name="skipAnimation">Used to skip the pop up animation</param>
    private void Show(bool skipAnimation = false)
    {
        if(!skipAnimation)
        {
            // popup effect
            _panel.transform.localScale = Vector3.zero;
            LeanTween.alphaCanvas(_canvasGroup, 1f, 0.5f).setEase(LeanTweenType.easeInOutQuad);
            LeanTween.scale(_panel, Vector3.one, 0.5f).setEase(LeanTweenType.easeInOutQuad);
        }
        else
        {
            // instantly shows the inventory screen when skipping the animation
            _canvasGroup.alpha = 1f;
        }

        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.interactable = true;
        _isVisible = true;

        // updates all slots to show the correct items
        RefreshItems();

        // allow the user to move the cursor freely
        Cursor.lockState = CursorLockMode.None;

        // Tells the inventory system that the inventory is open
        InventorySystem.instance.ToggleInventory(true);
    }

    /// <summary>
    /// Goes through all the item slots and updates them to show the correct items
    /// </summary>
    public void RefreshItems()
    {
        for (int i = 0; i < _itemSlots.Length; i++)
        {
            if (_itemSlots[i] != null)
                _itemSlots[i].SetItem(InventorySystem.instance.GetItemByIndex(i));
            else
                _itemSlots[i].Clear(); // actually overkill, since SetItem already clears the slot if the item is null
        }

        DeselectSlots();
    }

    private void OnDestroy()
    {
        // Unsubscribe from the inventory system to avoid memory leaks
        InventorySystem.instance.OnInventoryChanged -= RefreshItems;
    }

    /// <summary>
    /// Clears selection for all slots
    /// </summary>
    public void DeselectSlots()
    {
        if (_selectedSlot != null)
        {
            _selectedSlot.Deselect();
            _selectedSlot = null;
        }

        // shows a hint in the description box
        _itemName.text = "";
        _itemDescription.text = "Click an item to know more!";

        // prevent use of buttons since there is no item selected
        _useButton.interactable = false;
        _dropButton.interactable = false;
    }

    /// <summary>
    /// Called when an item slot is clicked
    /// </summary>
    /// <param name="slot"></param>
    public void SetSelectedSlot(ItemSlot slot)
    {
        // clears all selections
        DeselectSlots();

        if (slot == null)
        {
            _selectedSlot = null;
            return;
        }

        // sets the selected slot
        _selectedSlot = slot;
        _selectedSlot.Select();

        // shows item info on the description box
        ItemData item = slot.GetItemData();
        if (item != null)
        {
            _itemName.text = slot.GetItemData().itemName;
            _itemDescription.text = slot.GetItemData().itemDescription;

            // enables the buttons since there is an item selected, not all items are usabale
            _useButton.interactable = item.usable;
            _dropButton.interactable = true;
        }
        else
        {
            DeselectSlots();
        }
    }

    /// <summary>
    /// Placeholder funcitonality for using item. It mostly just deletes the item and show a pop up
    /// </summary>
    public void UseSelectedItem()
    {
        if (_selectedSlot != null && _selectedSlot.GetItemData() != null)
        {
            ItemData item = InventorySystem.instance.RemoveItem(_selectedSlot.transform.GetSiblingIndex());
            RefreshItems();

            // Instantiate and set it so it appears in front of all else
            GameObject popup = Instantiate(_usedItemPopUpPrefab);
            popup.transform.SetParent(transform.parent, false);
            popup.transform.SetAsLastSibling();
            popup.transform.position = _popupSpawnPoint.transform.position;

            // initialize the popup with the item name and start fade
            popup.GetComponent<ItemUsedPopup>().Show(item.displayName);
        }
    }

    /// <summary>
    /// Spawns the item on the game level, so it can be picked again
    /// </summary>
    public void DropSelectedItem()
    {
        if (_selectedSlot != null && _selectedSlot.GetItemData() != null)
        {
            ItemData item = InventorySystem.instance.RemoveItem(_selectedSlot.transform.GetSiblingIndex());
            Instantiate(item.itemPrefab, _dropSpot.position, _dropSpot.rotation);
            RefreshItems();
        }
    }
}
