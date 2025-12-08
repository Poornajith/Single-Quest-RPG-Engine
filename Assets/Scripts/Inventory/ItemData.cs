using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    // A unique ID to identify this item across game sessions
    public string itemID;

    public string itemName;      // Display name
    public Sprite icon;          // UI icon
    public bool stackable = true;

    // Optional: Use this method in the Editor to generate a unique GUID
    [ContextMenu("Generate New ID")]
    private void GenerateID()
    {
        itemID = System.Guid.NewGuid().ToString();
    }
}
