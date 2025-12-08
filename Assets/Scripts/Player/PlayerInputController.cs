using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
    public static PlayerInputController instance;

    [SerializeField] private InputAction interact;
    [SerializeField] private InputAction switchWeapon;

    [Header("Trigger Tags")]
    [SerializeField] private string npcTag = "NPC";

    private IInteractable currentInteractable; // Reference to the object we can interact with
    private bool isNPCInRange;

    private void Awake()
    {
        instance = this;

        interact.Enable();
        switchWeapon.Enable();

        interact.performed += OnPlayerInteract;
        switchWeapon.performed += OnSwitchWeapon;
    }

    private void OnPlayerInteract(InputAction.CallbackContext context)
    {
        Debug.Log("Interact Pressed");

        // 1. Check if an NPC is in range to start dialogue (high priority interaction)
        if (isNPCInRange)
        {
            if(DialougeManager.instance == null)
            {
                Debug.LogWarning("DialougeManager instance is null!");
                return;
            }
            if(DialougeManager.instance.IsDialogueActive())
            {
                Debug.Log("player requested next dialouge line !");
                DialougeManager.instance.NextDialougeLine();
                return;
            }
            DialougeManager.instance.StartDialogue();
            Debug.Log("Starting Dialogue with NPC");
        }
        // 2. Otherwise, check if a generic IInteractable is in range
        else if (currentInteractable != null)
        {
            currentInteractable.Interact();
        }
    }
    private void OnSwitchWeapon(InputAction.CallbackContext context)
    {
        Debug.Log("Switch Weapon Pressed");
        // You can add weapon switching logic here
        if (WeaponEquipController.instance.IsWeaponEquiped())
        {
            WeaponEquipController.instance.UnEquipWeapon();
            return;
        }
        WeaponEquipController.instance.EquipWeapon();
    }

    private void OnEnable()
    {
        interact.Enable();
        switchWeapon.Enable();
    }
    private void OnDisable()
    {
        // Unsubscribe to prevent memory leaks and unexpected behavior
        interact.performed -= OnPlayerInteract;
        switchWeapon.performed -= OnSwitchWeapon;

        interact.Disable();
        switchWeapon.Disable();
    }
    private void OnTriggerEnter(Collider other)
    {
        PlayerHUDController.Instance.ShowInteractHint();
        // Check if the object we collided with is an NPC
        if (other.CompareTag(npcTag))
        {
            isNPCInRange = true;
            Debug.Log("NPC in range for interaction");
            return;
        }
        // Check if the object implements IInteractable
        currentInteractable = other.GetComponent<IInteractable>();
        if (currentInteractable != null)
        {
            Debug.Log("Interactable object in range");
        }
    }
    private void OnTriggerExit(Collider other)
    {
        PlayerHUDController.Instance.HideInteractHint();
        // Check if we are exiting an NPC's range
        if (other.CompareTag(npcTag))
        {
            isNPCInRange = false;
            Debug.Log("NPC out of range for interaction");
            return;
        }
        // Check if we are exiting an IInteractable's range
        if (other.GetComponent<IInteractable>() == currentInteractable)
        {
            currentInteractable = null;
            Debug.Log("Interactable object out of range");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(npcTag))
        {
            //NPCController npcController = collision.GetComponent<NPCController>();
            // NOTE: The Input Action Callback (OnPlayerInteract) now handles the StartDialogue call.
            // The logic below is redundant and was based on checking `playerInteract.triggered` directly,
            // which should be avoided when using the 'performed' callback.
            // I'm keeping the LookAt logic as it's visual and harmless here.

            // LookAt logic moved to NPCController.OnTriggerStay for better encapsulation (see NPCController update)
        }
    }

}
