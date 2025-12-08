using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Events;

public class NPCController : MonoBehaviour
{
    [Header("NPC Data")]
    [SerializeField] private NPCName npcName; // Must match QuestStep.npcName

    [Header("NPC Anim Triggers")]
    [SerializeField] private string greetAnimation = "Greet";
    [SerializeField] private string inDialougeAnimation = "IsTalking";

    [Header("NPC Dialouge")]
    [SerializeField]
    private string[] dialogueText = { "Hello, traveler! Welcome to our village.",
                                            "Our Windmill was broken",
                                            "Can you fix it?" };
    private string[] nonQuestDialogueText = { "Hello, traveler! Welcome to our village.",
                                            "this place has nice scenery !",
                                            "is it?" };

    private Animator animator;
    private NPCState currentState = NPCState.Idle;

    private enum NPCState
    {
        Idle,
        InDialogue
    }
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHUDController.Instance.ShowInteractHint(); // Handled by PlayerInputController now (via IInteractable)
            
            if(QuestController.Instance.GetCurrentStepNPC() == npcName)
            {
                DialougeManager.instance.SetDialouge(dialogueText);               
            }
            else
            {
                DialougeManager.instance.SetDialouge(nonQuestDialogueText);
            }

            DialougeManager.OnDialogueEnd += OnDialogueFinished;

            GreetPlayer();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHUDController.Instance.HideInteractHint(); // Handled by PlayerInputController now (via IInteractable)

            // If dialogue is active, tell the manager to end it
            if (currentState == NPCState.InDialogue)
            {
                DialougeManager.instance.EndDialouge();
                currentState = NPCState.Idle;
            }
            animator.SetBool(inDialougeAnimation, false);

            DialougeManager.OnDialogueEnd -= OnDialogueFinished;
        }
        // PlayerHUDController.Instance.HideDialogueHUD(); // This is now handled by DialougeManager.OnDialogueEnd
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // NPC always looks at the player while they are near
            transform.LookAt(
                new Vector3(other.transform.position.x, transform.position.y, other.transform.position.z));
        }
    }
    public void GreetPlayer()
    {
        animator.SetTrigger(greetAnimation);
    }
    public void SetIsTalking()
    {
        animator.SetBool(inDialougeAnimation, true);
        currentState = NPCState.InDialogue;
    }

    private void OnDialogueFinished()
    {
        // Tell the QuestController that *this* NPC's dialogue just ended
        QuestController.Instance.HandleNPCDialogueEnd(npcName);

        // Always unsubscribe immediately after execution to prevent multi-firing
        DialougeManager.OnDialogueEnd -= OnDialogueFinished;
    }
}
