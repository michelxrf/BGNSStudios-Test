using UnityEngine;

public class CollectableObject : MonoBehaviour
{
    [SerializeField] private ItemData data;

    public void Interact()
    {
        Debug.Log($"Interacted with {data.itemName}");
    }
}
