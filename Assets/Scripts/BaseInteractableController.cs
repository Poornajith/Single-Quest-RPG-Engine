using UnityEngine;

public abstract class BaseInteractableController : MonoBehaviour, IInteractable
{
    // Common quest data for any interactable object.
    [Header("Related Quest Data")]
    [SerializeField]
    protected QuestStep relatedQuestStep; // 'protected' allows derived classes to access it.
    [SerializeField]
    protected QuestData relatedQuest;

    // The required method from IInteractable.
    // It is made 'virtual' so that derived classes can 'override' it,
    // allowing us to implement the common logic here first, then let the
    // derived classes add their specific logic.
    public virtual void Interact()
    {
        // 1. Execute the object-specific interaction logic.
        HandleInteractionLogic();

        // 2. Execute the common quest completion check (Polymorphism).
        CheckForQuestCompletion();
    }

    // Abstract method: Derived classes MUST implement this for their unique interaction.
    // This enforces specific behavior for each interactable object.
    protected abstract void HandleInteractionLogic();

    // Common method: Handles the shared logic of checking and completing the quest.
    protected void CheckForQuestCompletion()
    {
        // Check if the current quest and quest step match the interactable's requirements.
        if (QuestController.Instance != null && QuestController.Instance.GetCurrentQuest() == relatedQuest
            && QuestController.Instance.GetCurrentStep().objectiveType == relatedQuestStep.objectiveType)
        {
            QuestController.Instance.CompleteStep();
            Debug.Log($"Quest step completed for {gameObject.name}.");
        }
    }
}
