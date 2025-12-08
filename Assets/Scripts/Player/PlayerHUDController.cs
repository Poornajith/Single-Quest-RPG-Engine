using TMPro;
using UnityEngine;

public class PlayerHUDController : MonoBehaviour
{
    public static PlayerHUDController Instance;

    [SerializeField] private GameObject interactHint;

    [Header("NPC Dialouges")]
    [SerializeField] private GameObject dialogueUI;
    [SerializeField] private TMP_Text dialogueText;

    [Header("Quest Details")]
    [SerializeField] private GameObject questUI;
    [SerializeField] private TMP_Text questText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        interactHint.SetActive(false);
        dialogueUI.SetActive(false);
        questUI.SetActive(true);
    }

    private void OnEnable()
    {
        // Subscribe to DialogueManager events
        DialougeManager.OnDialogueLineRequested += ShowDialogueHUD;
        DialougeManager.OnDialogueEnd += HideDialogueHUD;
    }
    private void OnDisable()
    {
        // Unsubscribe from DialogueManager events
        DialougeManager.OnDialogueLineRequested -= ShowDialogueHUD;
        DialougeManager.OnDialogueEnd -= HideDialogueHUD;
    }

    private void Start()
    {
        HideInteractHint();
        HideDialogueHUD();
    }
    public void ShowInteractHint()
    {
        interactHint.SetActive(true);
    }
    public void HideInteractHint()
    {
        interactHint.SetActive(false);
    }
    public void ShowDialogueHUD(string dialogue)
    {
        dialogueUI.SetActive(true);
        dialogueText.text = dialogue;
    }
    public void HideDialogueHUD()
    {
        dialogueUI.SetActive(false);
        dialogueText.text = string.Empty;
    }
    public void ShowQuest(string questInfo)
    {
        questUI.SetActive(true);
        questText.text = questInfo;
    }
    public void HideQuest()
    {
        questUI.SetActive(false);
        questText.text = string.Empty;
    }
}
