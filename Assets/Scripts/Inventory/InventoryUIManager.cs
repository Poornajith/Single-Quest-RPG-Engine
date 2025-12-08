using System.Collections.Generic;
using UnityEngine;

public class InventoryUIManager : MonoBehaviour
{
    private void OnEnable()
    {
        // 💡 Subscribe to the event when this script is active
        InventoryController.OnInventoryChanged += OnInventoryUpdate;
    }

    private void OnDisable()
    {
        // 💡 Unsubscribe when this script is disabled to prevent memory leaks
        InventoryController.OnInventoryChanged -= OnInventoryUpdate;
    }

    // This method is called every time the inventory changes
    private void OnInventoryUpdate(ItemData item, int amount)
    {
        Debug.Log("--- UI Update Triggered ---");

        // 1. Update the specific item that changed (useful for showing notifications)
        if (item != null)
        {
            string action = amount > 0 ? "gained" : "removed";
            Debug.Log($"Notification: Player {action} {Mathf.Abs(amount)}x {item.itemName}.");
        }

        // 2. Refresh the entire UI display (the standard approach)
        RefreshInventoryDisplay();
    }

    private void RefreshInventoryDisplay()
    {
        if (InventoryController.Instance == null) return;

        // Get the current state of the inventory
        List<InventoryItem> currentItems = InventoryController.Instance.GetAllItems();

        // --- This is where your actual UI logic goes ---

        Debug.Log($"--- Displaying {currentItems.Count} unique item stacks: ---");

        foreach (InventoryItem inventoryItem in currentItems)
        {
            // Update the UI slot corresponding to inventoryItem.item.icon
            Debug.Log($"SLOT: {inventoryItem.item.itemName} x{inventoryItem.amount}");
        }
        Debug.Log("-----------------------------------------------------");
    }
}
