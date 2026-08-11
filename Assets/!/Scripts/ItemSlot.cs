using UnityEngine;

public class ItemSlot : MonoBehaviour
{
    public void SetItem(ItemData itemData)
    {
        // Set the item data for this slot
        // You can update the UI elements (like icon, name, etc.) here based on the itemData
        Debug.Log($"Item '{itemData.itemName}' set in the slot.");
    }

    public void Clear()
    {
        // Clear the item data for this slot
        // You can reset the UI elements here
        Debug.Log("Item slot cleared.");
    }
}
