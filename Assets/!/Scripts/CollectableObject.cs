using UnityEngine;

public class CollectableObject : MonoBehaviour
{
    [SerializeField] private ItemData data;

    public void Interact()
    {
        if(InventorySystem.instance.IsFull())
        {
            Debug.Log("Inventory is full. Cannot collect item.");
            return;
        }

        InventorySystem.instance.AddItem(data);
        Destroy(gameObject);
    }
}
