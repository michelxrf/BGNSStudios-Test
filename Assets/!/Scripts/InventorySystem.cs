using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class InventorySystem : MonoBehaviour
{
    private const int INVENTORY_CAPACITY = 10;
    private const string INVENTORY_SAVE_KEY = "InventoryData";
    
    public static InventorySystem instance;

    ItemData[] inventory = new ItemData[INVENTORY_CAPACITY];

    private bool isInventoryOpen = false;
    public bool IsInventoryOpen { get { return isInventoryOpen; } }

    public Action OnInventoryChanged;

    private void Awake()
    {
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

        ItemData item = Resources.Load<ItemData>($"!/Data/{itemName}");
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

        Debug.Log($"Item '{itemData.itemName}' was added to the inventory.");
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
        SaveInventory();
        OnInventoryChanged?.Invoke();
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

    public void SwapItems(int indexA, int indexB)
    {
        if (indexA < 0 || indexA >= INVENTORY_CAPACITY || indexB < 0 || indexB >= INVENTORY_CAPACITY)
        {
            Debug.Log($"Bad indices {indexA}, {indexB}.");
            return;
        }
        
        ItemData temp = inventory[indexA];
        inventory[indexA] = inventory[indexB];
        inventory[indexB] = temp;
        
        SaveInventory();
        OnInventoryChanged?.Invoke();
    }

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

    public bool IsFull()
    {
        return CountItems() >= INVENTORY_CAPACITY;
    }

    public void ToggleInventory(bool newState)
    {
        isInventoryOpen = newState;
    }
}


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
