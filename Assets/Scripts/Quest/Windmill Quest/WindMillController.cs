using UnityEngine;

public class WindMillController : MonoBehaviour, IInteractable
{
    [SerializeField] private string windmillFixAnimationTrigger = "Fix";

    [Header("Related Quest Data")]
    [SerializeField] private QuestStep relatedQuestStep;
    [SerializeField] private QuestData relatedQuest;

    private Animator windmillAnimator;
    private void Start()
    {
        windmillAnimator = GetComponent<Animator>();
    }
    public void Interact()
    {
        windmillAnimator.SetTrigger(windmillFixAnimationTrigger);
        if (QuestController.Instance.GetCurrentQuest() == relatedQuest
            && QuestController.Instance.GetCurrentStep().objectiveType == relatedQuestStep.objectiveType)
        {
            QuestController.Instance.CompleteStep();
        }
    }
}
