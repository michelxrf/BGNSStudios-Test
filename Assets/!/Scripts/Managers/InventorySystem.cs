using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

/// <summary>
/// Handles the logic of the inventory system and it's saving and loading of inventory data
/// </summary>
public class InventorySystem : MonoBehaviour
{
    private const int INVENTORY_CAPACITY = 10;
    private const string INVENTORY_SAVE_KEY = "InventoryData";
    
    public static InventorySystem instance {private set; get; }

    ItemData[] inventory = new ItemData[INVENTORY_CAPACITY];

    private bool isInventoryOpen = false;
    public bool IsInventoryOpen { get { return isInventoryOpen; } }

    public Action OnInventoryChanged;

    private void Awake()
    {
        // set it up as a partial Singleton, no need to add it to Dont Destroy for we only have one game scene
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    private void Start()
    {
        LoadInventory();
    }

    /// <summary>
    /// Counts the number of items in the inventory
    /// </summary>
    public int CountItems()
    {
        int count = 0;
        foreach (var item in inventory)
        {
            if (item != null) count++;
        }
        return count;
    }

    /// <summary>
    /// Adds an item to the inventory by item name
    /// </summary>
    public void AddItem(string itemName)
    {
        if (CountItems() >= INVENTORY_CAPACITY)
        {
            Debug.Log("Inventory is full. Cannot add item.");
            return;
        }

        ItemData item = Resources.Load<ItemData>($"Data/Items/{itemName}");
        if (item == null)
        {
            item = Resources.Load<ItemData>(itemName);
        }

        if (item == null)
        {
            ItemData[] allItems = Resources.LoadAll<ItemData>("");
            item = System.Array.Find(allItems, i => i != null && i.itemName == itemName);
        }

        if (item == null)
        {
            Debug.Log($"Item '{itemName}' not found.");
            return;
        }

        AddItem(item);
    }

    /// <summary>
    /// Add an item to the inventory from a item Scriptable Object
    /// Add an item to the inventory from a item Scriptable Object
    /// </summary>
    /// <param name="itemData"></param>
    public void AddItem(ItemData itemData)
    {
        if (CountItems() >= INVENTORY_CAPACITY)
        {
            Debug.Log("Inventory is full. Cannot add item.");
            return;
        }
        
        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i] == null) {
                inventory[i] = itemData;
                break;
            }
        }

        // Saves the inventory at every item added
        SaveInventory();
        OnInventoryChanged?.Invoke();
    }

    /// <summary>
    /// Removes an item from the inventory by index
    /// </summary>
    public ItemData RemoveItem(int index)
    {
        if (index < 0 || index >= INVENTORY_CAPACITY)
        {
            Debug.Log($"Bad index {index}.");
            return null;
        }

        ItemData removedItem = inventory[index];
        inventory[index] = null;

        // saves to file every time something is removed
        SaveInventory();
        OnInventoryChanged?.Invoke();

        // returns the item, its used for dropped and used items
        return removedItem;
    }

    /// <summary>
    /// Removes an item from the inventory by item name
    /// </summary>
    public ItemData RemoveItem(string itemName)
    {
        ItemData removedItem = null;
        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i] != null && inventory[i].itemName == itemName)
            {
                removedItem = RemoveItem(i);
                break;
            }
        }

        return removedItem;
    }

    // Removes an item by it's Scriptable Object
    public ItemData RemoveItem(ItemData itemData)
    {
        ItemData removedItem = null;
        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i] == itemData)
            {
                removedItem = RemoveItem(i);
                break;
            }
        }
        return removedItem;
    }

    /// <summary>
    /// Swaps two items in the array, can handle null
    /// </summary>
    /// <param name="indexA"></param>
    /// <param name="indexB"></param>
    public void SwapItems(int indexA, int indexB)
    {
        if (indexA < 0 || indexA >= INVENTORY_CAPACITY || indexB < 0 || indexB >= INVENTORY_CAPACITY)
        {
            Debug.Log($"Bad indices {indexA}, {indexB}.");
            return;
        }
        
        // holds the item in a temp var and then swaps
        ItemData temp = inventory[indexA];
        inventory[indexA] = inventory[indexB];
        inventory[indexB] = temp;
        
        SaveInventory();
        OnInventoryChanged?.Invoke();
    }

    /// <summary>
    /// Finds and return an item by it's index pos
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public ItemData GetItemByIndex(int index)
    {
        return inventory[index];
    }

    /// <summary>
    /// Saves the inventory to PlayerPrefs
    /// </summary>
    private void SaveInventory()
    {
        InventorySaveData saveData = new InventorySaveData();

        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i] != null)
            {
                saveData.slots.Add(new InventorySlotData
                {
                    slotIndex = i,
                    itemName = inventory[i].itemName
                });
            }
        }

        string json = JsonUtility.ToJson(saveData);
        PlayerPrefs.SetString(INVENTORY_SAVE_KEY, json);
        PlayerPrefs.Save();
        Debug.Log($"Inventory saved with {saveData.slots.Count} items.");
    }

    /// <summary>
    /// Loads the inventory from PlayerPrefs
    /// </summary>
    private void LoadInventory()
    {
        if (!PlayerPrefs.HasKey(INVENTORY_SAVE_KEY))
        {
            Debug.Log("No saved inventory found.");
            return;
        }

        Array.Clear(inventory, 0, inventory.Length);

        string json = PlayerPrefs.GetString(INVENTORY_SAVE_KEY);
        InventorySaveData saveData = JsonUtility.FromJson<InventorySaveData>(json);

        if (saveData != null && saveData.slots != null)
        {
            ItemData[] allItems = Resources.LoadAll<ItemData>("");
            Dictionary<string, ItemData> itemDict = new Dictionary<string, ItemData>();
            foreach (var item in allItems)
            {
                if (item != null && !string.IsNullOrEmpty(item.itemName) && !itemDict.ContainsKey(item.itemName))
                {
                    itemDict.Add(item.itemName, item);
                }
            }

            foreach (var slot in saveData.slots)
            {
                if (slot.slotIndex >= 0 && slot.slotIndex < inventory.Length)
                {
                    if (itemDict.TryGetValue(slot.itemName, out ItemData itemData))
                    {
                        inventory[slot.slotIndex] = itemData;
                    }
                    else
                    {
                        Debug.LogWarning($"ItemData with itemName '{slot.itemName}' was not found in Resources.");
                    }
                }
            }
        }

        OnInventoryChanged?.Invoke();
        Debug.Log($"Inventory loaded with {CountItems()} items.");
    }

    /// <summary>
    /// Verify if an item is present in the inventory, could be used for checking for keys or quest items, not actually implemented
    /// </summary>
    /// <param name="itemName"></param>
    /// <returns></returns>
    public bool HasItem(string itemName)
    {
        foreach (var item in inventory)
        {
            if (item != null && item.itemName == itemName)
            {   
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// tests for inventory capacity
    /// </summary>
    /// <returns></returns>
    public bool IsFull()
    {
        return CountItems() >= INVENTORY_CAPACITY;
    }

    /// <summary>
    /// Mostly used by player components to prevent movement while in menu
    /// </summary>
    /// <param name="newState"></param>
    public void ToggleInventory(bool newState)
    {
        isInventoryOpen = newState;
    }
}

/// <summary>
/// Data structures to save the inventory
/// </summary>
[System.Serializable]
public class InventorySlotData
{
    public int slotIndex;
    public string itemName;
}

[System.Serializable]
public class InventorySaveData
{
    public List<InventorySlotData> slots = new List<InventorySlotData>();
}
