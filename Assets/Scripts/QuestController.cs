using NUnit.Framework.Interfaces;
using UnityEngine;

public class QuestController : MonoBehaviour
{
    public static QuestController Instance;

    [SerializeField] private QuestData[] allQuests;

    private int currentQuestIndex = -1;
    private int currentStepIndex = 0;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        StartNextQuest();
    }

    // ----------------------------- QUEST MANAGEMENT -----------------------------

    public void StartNextQuest()
    {
        currentQuestIndex++;

        if (currentQuestIndex >= allQuests.Length)
        {
            PlayerHUDController.Instance.ShowQuest("All Quests Completed!");
            return;
        }

        currentStepIndex = 0;
        PlayerHUDController.Instance.ShowQuest(GetCurrentHint());
    }

    public void CompleteStep()
    {
        if (!IsQuestActive()) return;

        currentStepIndex++;
        Debug.Log($"Completed step {currentStepIndex} of quest {currentQuestIndex}");

        // Finished the quest?
        if (currentStepIndex >= GetCurrentQuest().steps.Length)
        {
            PlayerHUDController.Instance.ShowQuest("Quest Completed!");
            StartNextQuest();
            return;
        }

        PlayerHUDController.Instance.ShowQuest(GetCurrentHint());
    }

    // ----------------------------- CHECKS & GETTERS -----------------------------

    public bool IsQuestActive()
    {
        return currentQuestIndex < allQuests.Length;
    }

    public QuestData GetCurrentQuest()
    {
        return allQuests[currentQuestIndex];
    }

    public QuestStep GetCurrentStep()
    {
        return GetCurrentQuest().steps[currentStepIndex];
    }

    public string GetCurrentHint()
    {
        return GetCurrentStep().hint;
    }

    public NPCName GetCurrentStepNPC()
    {
        return GetCurrentStep().npcName;
    }

    // ----------------------------- OBJECTIVE TRIGGERS -----------------------------

    public void TalkToNPC(NPCName npcName)
    {
        var step = GetCurrentStep();
        if (step.objectiveType == QuestObjectiveType.TalkToNPC &&
            step.npcName == npcName)
        {
            CompleteStep();
        }
    }

    public void CollectItem(string itemName)
    {
        var step = GetCurrentStep();
        if (step.objectiveType == QuestObjectiveType.CollectItem &&
            step.requiredItem.itemName == itemName)
        {
            InventoryController.Instance.AddItem(step.requiredItem);
            CompleteStep();
        }
    }

    public void CraftItem(string itemName)
    {
        var step = GetCurrentStep();
        if (step.objectiveType == QuestObjectiveType.CraftItem &&
            step.requiredItem.itemName == itemName)
        {
            CompleteStep();
        }
    }

    public void DeliverItem(string itemName)
    {
        var step = GetCurrentStep();
        if (step.objectiveType == QuestObjectiveType.DeliverItem &&
            step.requiredItem.itemName == itemName)
        {
            CompleteStep();
        }
    }

    public void CustomStepCompleted()
    {
        var step = GetCurrentStep();
        if (step.objectiveType == QuestObjectiveType.Custom)
        {
            CompleteStep();
        }
    }

    private void OnEnable()
    {
        InventoryController.OnInventoryChanged += OnItemAdded;
    }

    private void OnDisable()
    {
        InventoryController.OnInventoryChanged -= OnItemAdded;
    }

    private void OnItemAdded(ItemData item, int amount)
    {
        var step = GetCurrentStep();

        if (step.objectiveType == QuestObjectiveType.CollectItem &&
            step.requiredItem == item)
        {
            CompleteStep();
        }
    }

    // ----------------------------- OBJECTIVE HANDLERS -----------------------------

    public void HandleNPCDialogueEnd(NPCName npcName)
    {
        if (!IsQuestActive()) return;

        var step = GetCurrentStep();

        if (step.npcName != npcName) return; // Not the right NPC for the current step

        // Check the specific objective that this dialogue end fulfills
        switch (step.objectiveType)
        {
            case QuestObjectiveType.TalkToNPC:
                CompleteStep();
                break;

            case QuestObjectiveType.DeliverItem:
                // SAFETY CHECK: Ensure InventoryController exists
                if (InventoryController.Instance != null)
                {
                    if (InventoryController.Instance.HasItem(step.requiredItem))
                    {
                        InventoryController.Instance.RemoveItem(step.requiredItem);
                        CompleteStep();
                    }
                    else
                    {
                        Debug.Log("Quest: Player does not have the required item yet.");
                    }
                }
                break;

            case QuestObjectiveType.BuyItem:
                // This case might be less common on dialogue end, but if the NPC hands over an item:
                 if (step.requiredItem != null)
                {
                    // SAFETY CHECK: Ensure InventoryController exists
                    if (InventoryController.Instance != null)
                    {
                        Debug.Log("Bought an item: " + step.requiredItem + " From NPC: " + step.npcName);
                        InventoryController.Instance.AddItem(step.requiredItem);
                        CompleteStep();
                    }
                    else
                    {
                        Debug.LogError("Quest Error: Cannot add item. InventoryController is missing!");
                    }
                }
                break;

            case QuestObjectiveType.CollectItem:
                // This case might be less common on dialogue end, but if the NPC hands over an item:
                 if (step.requiredItem != null)
                {
                    // SAFETY CHECK: Ensure InventoryController exists
                    if (InventoryController.Instance != null)
                    {
                        Debug.Log("Collected item: " + step.requiredItem + " From NPC: " + step.npcName);
                        InventoryController.Instance.AddItem(step.requiredItem);
                        CompleteStep();
                    }
                    else
                    {
                        Debug.LogError("Quest Error: Cannot add item. InventoryController is missing!");
                    }
                }
                break;

            case QuestObjectiveType.Custom:
                // This allows the NPC's dialogue to serve as the Custom completion trigger
                CompleteStep();
                break;
        }
    }
}
