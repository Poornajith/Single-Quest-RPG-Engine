using UnityEngine;

public class WindMillController : BaseInteractableController
{
    [SerializeField] private string windmillFixAnimationTrigger = "Fix";

    private Animator windmillAnimator;
    private void Start()
    {
        windmillAnimator = GetComponent<Animator>();
    }
    // Implement the abstract method to define the specific behavior for the windmill.
    protected override void HandleInteractionLogic()
    {
        windmillAnimator.SetTrigger(windmillFixAnimationTrigger);
    }
    // The Interact() method is inherited from BaseInteractableController
    // which calls HandleInteractionLogic() and then CheckForQuestCompletion().
}
