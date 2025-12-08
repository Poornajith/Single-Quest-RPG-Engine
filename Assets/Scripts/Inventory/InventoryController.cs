using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;         // For file operations

public class InventoryController : MonoBehaviour
{
    public static InventoryController Instance;

    // Delegate definition: a method that takes an ItemData and an int
    public delegate void ItemEvent(ItemData item, int amount);

    // Static Events: Other scripts can subscribe to these
    public static event ItemEvent OnInventoryChanged;
    
    // Actual inventory storage
    private List<InventoryItem> items = new List<InventoryItem>();

    // Drag ALL your ItemData ScriptableObjects here in the Inspector!
    public ItemData[] allAvailableItemData;

    // Define a constant file path
    private string savePath => Application.persistentDataPath + "/inventory.json";

    private void Awake()
    {
        // Simple Singleton pattern
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject); // Ensures only one instance exists

        // Attempt to load on startup
        LoadInventory();
    }

    // A single, unified event for both Add/Remove
    private void TriggerInventoryUpdate(ItemData item, int amount)
    {
        OnInventoryChanged?.Invoke(item, amount);
    }
    // ---------------------------- ADD ITEM ---------------------------------
    public void AddItem(ItemData item, int amount = 1)
    {
        InventoryItem existing = items.FirstOrDefault(i => i.item == item);

        if (existing != null && item.stackable)
        {
            // Item is stackable and exists: increase amount
            existing.amount += amount;
        }
        else
        {
            // Item is new or not stackable: create a new entry
            items.Add(new InventoryItem { item = item, amount = amount });
        }

        TriggerInventoryUpdate(item, amount);
        Debug.Log($"Added: {item.itemName} x{amount}");
    }

    // ---------------------------- REMOVE ITEM ------------------------------
    public bool RemoveItem(ItemData item, int amount = 1)
    {
        InventoryItem existing = items.FirstOrDefault(i => i.item == item);

        // Check 1: Does the item exist?
        // Check 2: Do we have enough?
        if (existing == null || existing.amount < amount) 
        {
            Debug.LogWarning($"Failed to remove: Not enough {item.itemName}. Needed {amount}, found {existing?.amount ?? 0}.");
            return false;
        }           

        existing.amount -= amount;

        // Remove the container if the stack is depleted
        if (existing.amount <= 0)
            items.Remove(existing);

        TriggerInventoryUpdate(item, -amount); // Pass negative amount to indicate removal
        Debug.Log($"Removed: {item.itemName} x{amount}");
        return true;
    }

    // ---------------------------- CHECKS -----------------------------------
    public bool HasItem(ItemData item, int amount = 1)
    {
        InventoryItem existing = items.FirstOrDefault(i => i.item == item);
        return existing != null && existing.amount >= amount;
    }


    // Use Linq to get the count without exposing the internal InventoryItem class
    public int GetItemCount(ItemData item)
    {
        return items.FirstOrDefault(i => i.item == item)?.amount ?? 0;
    }

    public List<InventoryItem> GetAllItems()
    {
        // Best practice: return a copy of the list to prevent external modification
        return new List<InventoryItem>(items);
    }

    // ---------------------------- PERSISTENCE --------------------------------
    public void SaveInventory()
    {
        // 1. Convert the runtime InventoryItem list into the save-friendly list
        InventoryDataList saveList = new InventoryDataList();
        foreach (InventoryItem item in items)
        {
            saveList.items.Add(new InventorySaveData
            {
                itemID = item.item.itemID,
                amount = item.amount
            });
        }

        // 2. Serialize the data to a JSON string
        string json = JsonUtility.ToJson(saveList, true);

        // 3. Write the JSON string to a file
        File.WriteAllText(savePath, json);
        Debug.Log($"Inventory saved to: {savePath}");
    }
    public void LoadInventory()
    {
        if (!File.Exists(savePath))
        {
            Debug.LogWarning("No inventory save file found. Starting with empty inventory.");
            return;
        }

        try
        {
            // 1. Read the JSON string from the file
            string json = File.ReadAllText(savePath);

            // 2. Deserialize the JSON string into the save list
            InventoryDataList loadedList = JsonUtility.FromJson<InventoryDataList>(json);

            // 3. Clear current inventory and rebuild it
            items.Clear();

            foreach (InventorySaveData saveData in loadedList.items)
            {
                // 4. Look up the ItemData ScriptableObject using the saved ID
                ItemData itemBlueprint = allAvailableItemData.FirstOrDefault(d => d.itemID == saveData.itemID);

                if (itemBlueprint != null)
                {
                    // 5. Re-create the runtime InventoryItem
                    items.Add(new InventoryItem { item = itemBlueprint, amount = saveData.amount });
                }
                else
                {
                    Debug.LogError($"Item with ID {saveData.itemID} not found! Data ignored.");
                }
            }

            // Notify systems that the inventory has been loaded/changed
            OnInventoryChanged?.Invoke(null, 0);

            Debug.Log($"Inventory loaded successfully: {items.Count} unique stacks.");

        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to load inventory: {e.Message}");
        }
    }
}
