using UnityEngine;

public enum QuestObjectiveType
{
    TalkToNPC,
    BuyItem,
    CollectItem,
    CraftItem,
    GoToLocation,
    DeliverItem,
    Custom
}

[System.Serializable]
public class QuestStep
{
    public string hint;
    public QuestObjectiveType objectiveType;

    public ItemData requiredItem;  // object reference
    public NPCName npcName;        // For talk steps
}
