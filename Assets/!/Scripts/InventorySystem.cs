using UnityEngine;
using System.Collections.Generic;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem instance;

    private const int INVENTORY_CAPACITY = 9;
    private const string INVENTORY_SAVE_KEY = "InventoryData";

    private List<ItemData> inventory = new List<ItemData>();

    private bool isInventoryOpen = false;

    public bool IsInventoryOpen { get { return isInventoryOpen; } }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        LoadInventory();
    }

    public List<ItemData> Inventory { get { return inventory; } }

    /// <summary>
    /// Adds an item to the inventory by index of the ItemData
    /// </summary>
    public void AddItem(int index)
    {
        if (inventory.Count >= INVENTORY_CAPACITY)
        {
            Debug.Log("Inventory is full. Cannot add item.");
            return;
        }

        // This assumes you have a reference to the item (you may need to adjust this)
        // For now, this is a placeholder implementation
        Debug.Log($"Item at index {index} was added to the inventory.");
        SaveInventory();
    }

    /// <summary>
    /// Adds an item to the inventory by item name
    /// </summary>
    public void AddItem(string itemName)
    {
        if (inventory.Count >= INVENTORY_CAPACITY)
        {
            Debug.Log("Inventory is full. Cannot add item.");
            return;
        }

        Debug.Log($"Item '{itemName}' was added to the inventory.");
        SaveInventory();
    }

    /// <summary>
    /// Removes an item from the inventory by index
    /// </summary>
    public void RemoveItem(int index)
    {
        if (index < 0 || index >= inventory.Count)
        {
            Debug.Log($"Item at index {index} was not found in the inventory.");
            return;
        }

        inventory.RemoveAt(index);
        Debug.Log($"Item at index {index} was removed from the inventory.");
        SaveInventory();
    }

    /// <summary>
    /// Removes an item from the inventory by item name
    /// </summary>
    public void RemoveItem(string itemName)
    {
        ItemData itemToRemove = inventory.Find(item => item.itemName == itemName);

        if (itemToRemove == null)
        {
            Debug.Log($"Item '{itemName}' was not found in the inventory.");
            return;
        }

        inventory.Remove(itemToRemove);
        Debug.Log($"Item '{itemName}' was removed from the inventory.");
        SaveInventory();
    }

    /// <summary>
    /// Saves the inventory to PlayerPrefs
    /// </summary>
    private void SaveInventory()
    {
        string json = JsonUtility.ToJson(new SerializableItemList(inventory));
        PlayerPrefs.SetString(INVENTORY_SAVE_KEY, json);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Loads the inventory from PlayerPrefs
    /// </summary>
    private void LoadInventory()
    {
        inventory.Clear();

        if (!PlayerPrefs.HasKey(INVENTORY_SAVE_KEY))
        {
            Debug.Log("No saved inventory found.");
            return;
        }

        string json = PlayerPrefs.GetString(INVENTORY_SAVE_KEY);
        SerializableItemList savedList = JsonUtility.FromJson<SerializableItemList>(json);

        ItemData[] allItems = Resources.LoadAll<ItemData>("!");

        foreach (string itemName in savedList.itemNames)
        {
            ItemData item = System.Array.Find(allItems, i => i.itemName == itemName);
            if (item != null)
            {
                inventory.Add(item);
            }
        }

        Debug.Log($"Inventory loaded with {inventory.Count} items.");
    }

    public void ToggleInventory(bool newState)
    {
        isInventoryOpen = newState;
        Debug.Log($"Inventory is now {(isInventoryOpen ? "open" : "closed")}.");
    }
}

[System.Serializable]
public class SerializableItemList
{
    public List<string> itemNames = new List<string>();

    public SerializableItemList(List<ItemData> items)
    {
        foreach (ItemData item in items)
        {
            itemNames.Add(item.itemName);
        }
    }
}
