using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public string displayName;
    public string itemDescription;
    public Sprite itemIcon;
    public GameObject itemPrefab;
    public bool usable;
}
