using Cinemachine;
using StarterAssets;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class AnvilController : MonoBehaviour, IInteractable
{
    public static AnvilController Instance;

    [Header("Anvil Settings")]
    [SerializeField] private CinemachineVirtualCamera anvilCamera;
    [SerializeField] private float forgingDuration = 3f;
    [SerializeField] private GameObject player;
    [SerializeField] private Transform playerForgingTransform;

    [Header("Related Quest Data")]
    [SerializeField] private QuestStep relatedQuestStep;
    [SerializeField] private QuestData relatedQuest;

    [Header("Audio and Effects")]
    [SerializeField] private AudioSource anvilAudioSource;
    [SerializeField] private VisualEffect forgeParticles;

    [Header("Crafting Data")]
    [SerializeField] private ItemData CraftingRequiredItem;
    [SerializeField] private string CraftingAnimName = "IsForging";

    private CharacterController playerController;
    private ThirdPersonController playerThirdPersonController;
    private Animator playerAnimator;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        playerAnimator = player.GetComponent<Animator>();
        playerController = player.GetComponent<CharacterController>();
        playerThirdPersonController = player.GetComponent<ThirdPersonController>();
        forgeParticles.Stop();
    }
    public void Interact()
    {
        Debug.Log("Starting Forge Process...");

        WeaponEquipController.instance.EquipWeapon();

        playerAnimator.SetBool(CraftingAnimName, true);
        playerController.enabled = false;
        playerThirdPersonController.enabled = false;
        player.transform.position = playerForgingTransform.position;
        player.transform.rotation = playerForgingTransform.rotation;
        anvilCamera.Priority = 20;
        anvilAudioSource.Play();
        forgeParticles.Play();

        Invoke(nameof(EndForging), forgingDuration);
    }

    private void EndForging()
    {
        playerAnimator.SetBool(CraftingAnimName, false);
        playerController.enabled = true;
        playerThirdPersonController.enabled = true;
        anvilCamera.Priority = 0;
        anvilAudioSource.Stop();
        forgeParticles.Stop(); 
        WeaponEquipController.instance.UnEquipWeapon();

        if (QuestController.Instance.GetCurrentQuest() == relatedQuest
            && QuestController.Instance.GetCurrentStep().objectiveType == relatedQuestStep.objectiveType)
        {
            QuestController.Instance.CompleteStep();
        }
        else
        {
            InventoryController.Instance.AddItem(CraftingRequiredItem, 1);
            Debug.Log($"Crafted {CraftingRequiredItem.itemName} and added to inventory.");
        }
        PlayerHUDController.Instance.HideInteractHint();
    }
}
