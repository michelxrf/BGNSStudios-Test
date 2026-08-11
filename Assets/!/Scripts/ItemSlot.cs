using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler
{
    [SerializeField] GameObject iconPrefab;
    
    GameObject itemIcon;
    private ItemData _itemData;

    public void SetItem(ItemData itemData)
    {
        Clear();
        _itemData = itemData;

        if(itemData != null )
        {
            GameObject newIcon = Instantiate(iconPrefab, transform);
            newIcon.GetComponent<Image>().sprite = itemData.itemIcon;
            itemIcon = newIcon;
        }
        else
        {
            Clear();
        }
    }

    public void Clear()
    {
        _itemData = null;
        Destroy(itemIcon);
    }

    private void OnMouseDown()
    {
        
    }

    public ItemData GetItemData()
    {
        return _itemData;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(eventData.delta.magnitude < 2f)
        {
            return;
        }

        if (_itemData == null) return;

        itemIcon.transform.SetParent(transform.parent.transform.parent);
        itemIcon.transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_itemData == null) return;

        itemIcon.transform.position = eventData.position;
    }


    public void OnEndDrag(PointerEventData eventData)
    {
        RaycastResult raycastResult = eventData.pointerCurrentRaycast;

        if (raycastResult.gameObject != null)
        {
            ItemSlot targetSlot = raycastResult.gameObject.GetComponent<ItemSlot>();
            if (targetSlot != null && targetSlot != this)
            {
                InventorySystem.instance.SwapItems(FindSlotIndex(this), FindSlotIndex(targetSlot));
                return;
            }
        }

        itemIcon.transform.SetParent(transform);
        itemIcon.transform.localPosition = Vector3.zero;
    }

    public int FindSlotIndex(ItemSlot slot)
    {
        ItemSlot[] children = transform.parent.GetComponentsInChildren<ItemSlot>();

        for (int i = 0; i < transform.parent.childCount; i++)
        {
            if(children[i] == slot) return i;
        }

        return -1;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        InventoryScreenUI inventoryUI = FindFirstObjectByType<InventoryScreenUI>();
        if (inventoryUI != null)
        {
            inventoryUI.SetSelectedSlot(this);
        }
    }
}
