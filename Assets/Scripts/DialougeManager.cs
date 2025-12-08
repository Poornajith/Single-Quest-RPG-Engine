using UnityEngine;
using UnityEngine.Events;

public class DialougeManager : MonoBehaviour
{
    public static DialougeManager instance;

    // Public Events for decoupling the UI
    public static event System.Action<string> OnDialogueLineRequested;
    public static event System.Action OnDialogueEnd;

    private string[] currentDialouge;
    private int currentLineIndex = 0;
    private bool isDialougeActive = false;

    private void Awake()
    {
        instance = this;
    }

    public void SetDialouge(string[] dialougeLines)
    {
        currentDialouge = dialougeLines;
        currentLineIndex = 0;
    }
    public void StartDialogue()
    {
        ResetDialouge(); // Reset to the start

        isDialougeActive = true;

        // Use event instead of direct static call
        if (currentDialouge != null && currentDialouge.Length > 0)
        {
            OnDialogueLineRequested?.Invoke(currentDialouge[0]);
        }
    }
    public void EndDialouge()
    {
        ResetDialouge();
        isDialougeActive = false;

        // Use event instead of direct static call
        OnDialogueEnd?.Invoke();
    }

    public void NextDialougeLine()
    {
        if (currentDialouge != null && currentLineIndex < currentDialouge.Length - 1)
        {
            currentLineIndex++;
            Debug.Log("Current Line Index: " + currentLineIndex);

            // Use event instead of direct static call
            OnDialogueLineRequested?.Invoke(currentDialouge[currentLineIndex]);
        }
        else
        {
            EndDialouge();
        }
    }
    public void ClearDialouge()
    {
        currentLineIndex = 0;
        currentDialouge = null;
    }

    public void ResetDialouge()
    {
        currentLineIndex = 0;
        // Use event for reset display
        if (currentDialouge != null && currentDialouge.Length > 0)
        {
            OnDialogueLineRequested?.Invoke(currentDialouge[currentLineIndex]);
        }
    }
    public bool IsDialogueActive()
    {
        return isDialougeActive;
    }
}
