using UnityEngine;

/// <summary>
/// Component added to on the world items so they can be collected by the player when interacted with
/// </summary>
public class CollectableObject : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ItemData data;

    /// <summary>
    /// Called by the player to attempt to collect the item from the ground
    /// </summary>
    public void Interact()
    {
        if(InventorySystem.instance.IsFull())
        {
            Debug.Log("Inventory is full. Cannot collect item.");
            return;
        }

        // add the item to inventory system and destroy prefab
        InventorySystem.instance.AddItem(data);
        Destroy(gameObject);
    }
}
