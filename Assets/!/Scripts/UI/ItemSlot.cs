using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


/// <summary>
/// Handles the UI for an item slot in the inventory, including displaying the item icon and handling user interactions.
/// </summary>
public class ItemSlot : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler
{
    [Header("References")]
    [SerializeField] GameObject _iconPrefab;
    [SerializeField] GameObject _selectedHighlight;
    
    private GameObject _itemIcon;
    private ItemData _itemData;
    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        // makes sure the highlight is not visible at the start
        Deselect();
    }

    /// <summary>
    /// Allocates an item to the slot
    /// </summary>
    /// <param name="itemData">The item that will be placed</param>
    public void SetItem(ItemData itemData)
    {
        Clear();
        _itemData = itemData;

        // Sets to an actual item
        if(itemData != null )
        {
            // Instantiates the icon prefab and sets it as a child of this slot
            GameObject newIcon = Instantiate(_iconPrefab, transform);
            newIcon.GetComponent<Image>().sprite = itemData.itemIcon;
            _itemIcon = newIcon;
        }
        // Since it's also used to set slots to empty we need to handle null items by just clearing the slot
        else
        {
            Clear();
        }
    }

    /// <summary>
    /// Clears the slot of any item and removes the icon
    /// </summary>
    public void Clear()
    {
        _itemData = null;
        Destroy(_itemIcon);
    }

    /// <summary>
    /// Used to get the data of the item in this slot
    /// </summary>
    /// <returns>The ItemData of the item in this slot</returns>
    public ItemData GetItemData()
    {
        return _itemData;
    }

    /// <summary>
    /// Handles the initial logic when the player starts dragging an item from this slot
    /// </summary>
    public void OnBeginDrag(PointerEventData eventData)
    {
        // prevent dragging from empty slots
        if (_itemData == null) return;

        // simple effect of scaling the icon up when dragging starts
        LeanTween.scale(_itemIcon, Vector3.one * 1.2f, 0.1f).setEase(LeanTweenType.easeOutBack);

        // sets the out of its hierachy so it's visible on top of other UI elements while dragging
        _itemIcon.transform.SetParent(transform.parent.transform.parent);
        _itemIcon.transform.SetAsLastSibling();
    }

    /// <summary>
    /// Just make the icon follow the mouse while dragging
    /// </summary>
    public void OnDrag(PointerEventData eventData)
    {
        // prevent dragging from empty slots, likely overkill since it's already on OnBeginDrag
        if (_itemData == null) return;

        _itemIcon.transform.position = eventData.position;
    }


    /// <summary>
    /// Handles the logic when the player stops dragging an item
    /// </summary>
    public void OnEndDrag(PointerEventData eventData)
    {
        // gather data from the current mouse position to determine where the item was dropped
        RaycastResult raycastResult = eventData.pointerCurrentRaycast;

        // if the item was dropped on a valid slot, we can handle the item transfer
        if (raycastResult.gameObject != null)
        {
            ItemSlot targetSlot = raycastResult.gameObject.GetComponent<ItemSlot>();
            if (targetSlot != null && targetSlot != this)
            {
                // performs a swap of the items between the two slots, works for empty slots as well
                InventorySystem.instance.SwapItems(FindSlotIndex(this), FindSlotIndex(targetSlot));
                Deselect();
                _itemIcon.transform.localScale = Vector3.one;
                return;
            }
        }

        // reset the item, if it wasn't dropped on a valid slot, we just return it to its original position
        _itemIcon.transform.SetParent(transform);
        _itemIcon.transform.localPosition = Vector3.zero;
        _itemIcon.transform.localScale = Vector3.one;
        Deselect();
    }

    /// <summary>
    /// Finds the index of this slot in the inventory grid
    /// </summary>
    /// <param name="slot"></param>
    /// <returns></returns>
    public int FindSlotIndex(ItemSlot slot)
    {
        // gets all slots
        ItemSlot[] children = transform.parent.GetComponentsInChildren<ItemSlot>();

        // loops through the slots to find the index of the current slot
        for (int i = 0; i < transform.parent.childCount; i++)
        {
            if(children[i] == slot) return i;
        }

        return -1;
    }

    /// <summary>
    /// Selects the slot as the player clickt on it, highlighting it and playing a sound effect
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerDown(PointerEventData eventData)
    {
        // calls the screen so it unselects all other slots, then selects this one
        InventoryScreenUI inventoryUI = FindFirstObjectByType<InventoryScreenUI>();
        if (inventoryUI != null)
        {
            inventoryUI.SetSelectedSlot(this);
        }
        _audioSource.Play();
    }

    /// <summary>
    /// Just activates the visual highlight for this slot
    /// </summary>
    public void Select()
    {
        _selectedHighlight.SetActive(true);
    }

    /// <summary>
    /// Just deactivates the visual highlight for this slot
    /// </summary>
    public void Deselect()
    {
        _selectedHighlight.SetActive(false);
    }
}
