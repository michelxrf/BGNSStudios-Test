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

    private CanvasGroup _canvasGroup;
    private ItemSlot _selectedSlot;
    private bool isVisible = false;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        Hide(true);
        InventorySystem.instance.OnInventoryChanged += RefreshItems;
    }

    private void Update()
    {
        // prevents opening inventory while paused or in dialogue
        if (DialogueManager.instance.IsDialogueActive) return;
        if (PauseManager.instance.IsPaused) return;

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if(isVisible)
            {
                Hide();
            }
            else
            {
                Show();
            }
        }
    }

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
            _canvasGroup.alpha = 0f;
        }

        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.interactable = false;
        isVisible = false;
        Cursor.lockState = CursorLockMode.Locked;
        InventorySystem.instance.ToggleInventory(false);
    }

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
            _canvasGroup.alpha = 1f;
        }

        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.interactable = true;
        isVisible = true;
        RefreshItems();
        Cursor.lockState = CursorLockMode.None;
        InventorySystem.instance.ToggleInventory(true);
    }

    public void RefreshItems()
    {
        for (int i = 0; i < _itemSlots.Length; i++)
        {
            if(_itemSlots[i] != null)
                _itemSlots[i].SetItem(InventorySystem.instance.GetItemByIndex(i));
            else
                _itemSlots[i].Clear();
        }

        DeselectSlots();
    }

    private void OnDestroy()
    {
        InventorySystem.instance.OnInventoryChanged -= RefreshItems;
    }

    public void DeselectSlots()
    {
        if (_selectedSlot != null)
        {
            _selectedSlot.Deselect();
            _selectedSlot = null;
        }

        _itemName.text = "";
        _itemDescription.text = "Click an item to know more!";
        _useButton.interactable = false;
        _dropButton.interactable = false;
    }

    public void SetSelectedSlot(ItemSlot slot)
    {
        DeselectSlots();

        if (slot == null)
        {
            _selectedSlot = null;
            return;
        }

        _selectedSlot = slot;
        _selectedSlot.Select();

        ItemData item = slot.GetItemData();

        if (item != null)
        {
            _itemName.text = slot.GetItemData().itemName;
            _itemDescription.text = slot.GetItemData().itemDescription;

            _useButton.interactable = item.usable;
            _dropButton.interactable = true;
        }
        else
        {
            DeselectSlots();
        }
    }

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
            popup.transform.position = new Vector3(Screen.width / 2, Screen.height / 2, 0);

            // initialize the popup with the item name and start fade
            popup.GetComponent<ItemUsedPopup>().Show(item.displayName);
        }
    }

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
