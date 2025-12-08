using UnityEngine;

public class WindMillController : MonoBehaviour, IInteractable
{
    [SerializeField] private string windmillFixAnimationTrigger = "Fix";

    private Animator windmillAnimator;
    private void Start()
    {
        windmillAnimator = GetComponent<Animator>();
    }
    public void Interact()
    {
       windmillAnimator.SetTrigger(windmillFixAnimationTrigger);
       QuestController.Instance.CompleteStep();
    }
}
