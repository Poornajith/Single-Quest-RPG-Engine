using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InventorySaveData : MonoBehaviour
{
    //only save the ID and the amount, not the ItemData object itself.
    public string itemID;
    public int amount;
}
// Optional: A container to save the whole list easily
[Serializable]
public class InventoryDataList
{
    public List<InventorySaveData> items = new List<InventorySaveData>();
}
