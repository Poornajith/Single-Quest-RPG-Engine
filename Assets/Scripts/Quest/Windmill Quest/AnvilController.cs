using Cinemachine;
using StarterAssets;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class AnvilController : MonoBehaviour, IInteractable
{
    public static AnvilController Instance;

    [SerializeField] private string anvilUseAnimationTrigger = "Forge";
    [SerializeField] private CinemachineVirtualCamera anvilCamera;
    [SerializeField] private float forgingDuration = 3f;
    [SerializeField] private GameObject player;
    [SerializeField] private Transform playerForgingTransform;

    [Header("Audio and Effects")]
    [SerializeField] private AudioSource anvilAudioSource;
    [SerializeField] private VisualEffect forgeParticles;

    [Header("Crafting Data")]
    [SerializeField] private ItemData CraftingRequiredItem;

    private CharacterController playerController;
    private ThirdPersonController playerThirdPersonController;
    private Animator playerAnimator;

    [SerializeField] private string QuestHint = "Buy a new hammer from the blacksmith to start forging.";

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

        SwitchHammer();

        playerAnimator.SetBool("IsForging", true);
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
        playerAnimator.SetBool("IsForging", false);
        playerController.enabled = true;
        playerThirdPersonController.enabled = true;
        anvilCamera.Priority = 0;
        anvilAudioSource.Stop();
        forgeParticles.Stop();

        QuestController.Instance.CompleteStep();
        PlayerHUDController.Instance.HideInteractHint();
    }

    private void SwitchHammer() 
    {         
        WeaponEquipController.instance.EquipWeapon();
    }
}
