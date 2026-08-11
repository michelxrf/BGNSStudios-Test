using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

/// <summary>
/// Handles inventory screen
/// </summary>
public class InventoryScreenUI : MonoBehaviour
{
    private CanvasGroup canvasGroup;

    [Header("References")]
    [SerializeField] private ItemSlot[] itemSlots;
    [SerializeField] private TMP_Text itemName;
    [SerializeField] private TMP_Text itemDescription;
    [SerializeField] private Button useButton;
    [SerializeField] private Button dropButton;
    [SerializeField] private Transform dropSpot;
    [SerializeField] private GameObject usedItemPopUpPrefab;

    private ItemSlot selectedSlot;
    private bool isVisible = false;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        Hide();
        InventorySystem.instance.OnInventoryChanged += RefreshItems;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
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

    public void Hide()
    {
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
        isVisible = false;
        Cursor.lockState = CursorLockMode.Locked;
        InventorySystem.instance.ToggleInventory(false);
    }

    private void Show()
    {
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
        isVisible = true;
        RefreshItems();
        Cursor.lockState = CursorLockMode.None;
        InventorySystem.instance.ToggleInventory(true);
    }

    public void RefreshItems()
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if(itemSlots[i] != null)
                itemSlots[i].SetItem(InventorySystem.instance.GetItemByIndex(i));
            else
                itemSlots[i].Clear();
        }

        DeselectSlots();
    }

    private void OnDestroy()
    {
        InventorySystem.instance.OnInventoryChanged -= RefreshItems;
    }

    public void DeselectSlots()
    {
        if (selectedSlot != null)
        {
            selectedSlot.GetComponent<Image>().color = Color.white;
        }

        itemName.text = "";
        itemDescription.text = "";
        useButton.interactable = false;
        dropButton.interactable = false;
    }

    public void SetSelectedSlot(ItemSlot slot)
    {
        DeselectSlots();

        if (slot == null)
        {
            selectedSlot = null;
            return;
        }

        selectedSlot = slot;
        slot.GetComponent<Image>().color = Color.yellow;

        ItemData item = slot.GetItemData();

        if (item != null)
        {
            itemName.text = slot.GetItemData().itemName;
            itemDescription.text = slot.GetItemData().itemDescription;

            useButton.interactable = item.usable;
            dropButton.interactable = true;
        }
        else
        {
            DeselectSlots();
        }
    }

    public void UseSelectedItem()
    {
        if (selectedSlot != null && selectedSlot.GetItemData() != null)
        {
            ItemData item = InventorySystem.instance.RemoveItem(selectedSlot.transform.GetSiblingIndex());
            RefreshItems();

            // Instantiate and set it so it appears in front of all else
            GameObject popup = Instantiate(usedItemPopUpPrefab);
            popup.transform.SetParent(transform.parent, false);
            popup.transform.SetAsLastSibling();
            popup.transform.position = new Vector3(Screen.width / 2, Screen.height / 2, 0);

            // initialize the popup with the item name and start fade
            popup.GetComponent<ItemUsedPopup>().Show(item.itemName);
        }
    }

    public void DropSelectedItem()
    {
        if (selectedSlot != null && selectedSlot.GetItemData() != null)
        {
            ItemData item = InventorySystem.instance.RemoveItem(selectedSlot.transform.GetSiblingIndex());
            Instantiate(item.itemPrefab, dropSpot.position, dropSpot.rotation);
            RefreshItems();
        }
    }
}
