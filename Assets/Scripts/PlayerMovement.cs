using System;
using ScriptableObjectsScripts;
using Sirenix.OdinInspector;
using UnityEngine;
using Unity.Netcode;
using Unity.Cinemachine;


public class PlayerMovement : NetworkBehaviour
{
    private const float GravityValue = -9.81f;

    [Title("Player Camera Settings")]
    [SerializeField] private CinemachineCamera cinemachineCamera;

    [Title("Player Animation Settings")]
    [SerializeField] private float animationSmoothTime = 0.1f;

    [Title("Animation Rig Settings")]
    [SerializeField] private Transform aimTarget;
    [SerializeField] private float aimDistance;

    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;

    private InputManager inputManager;
    private Transform cameraTransform;
    private Animator animator;

    private int moveXAnimationParameterId;
    private int moveYAnimationParameterId;

    private Vector2 currentAnimationBlendVector;
    private Vector2 animationVelocity;

    private Player player;
    private PlayerData playerData;

    private float playerSpeed;
    private float sprintSpeed;

    private void Start()
    {
        if (!IsOwner) return;

        player = GetComponent<Player>();
        playerData = player.GetPlayerData();
        animator = GetComponent<Animator>();
        Initialize();
    }

    private void Initialize()
    {
        inputManager = InputManager.Instance;
        if (Camera.main != null) cameraTransform = Camera.main.transform;

        if (IsLocalPlayer)
        {
            cinemachineCamera.Priority = 10;
        } 
        else
        {
            cinemachineCamera.Priority = 0;
        }

        controller = gameObject.GetComponent<CharacterController>();

        moveXAnimationParameterId = Animator.StringToHash("MoveX");
        moveYAnimationParameterId = Animator.StringToHash("MoveY");

        playerSpeed = playerData.playerSpeed;
        sprintSpeed = playerData.sprintSpeed;
    }

    private void Update()
    {
        if (!IsOwner) return;

        Move();
    }

    private void Move()
    {
        groundedPlayer = controller.isGrounded;
        
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = -2f; // Ensure the player stays grounded
        }

        Vector2 input = inputManager.GetPlayerMovement();

        // Calculate the camera forward and right directions
        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;

        // Project camera directions onto the horizontal plane (y = 0)
        cameraForward.y = 0f;
        cameraRight.y = 0f;
        cameraForward.Normalize();
        cameraRight.Normalize();

        aimTarget.position = cinemachineCamera.transform.position + cinemachineCamera.transform.forward * aimDistance;

        currentAnimationBlendVector = Vector2.SmoothDamp(currentAnimationBlendVector, input, ref animationVelocity, animationSmoothTime);
        
        // Calculate the movement direction based on camera directions
        Vector3 moveDirection = cameraForward * input.y + cameraRight * input.x;

        animator.SetFloat(moveXAnimationParameterId, currentAnimationBlendVector.x);
        animator.SetFloat(moveYAnimationParameterId, currentAnimationBlendVector.y);

        moveDirection.Normalize();

        float currentSpeed = inputManager.GetSprint() ? sprintSpeed : playerSpeed;

        // Move the player
        controller.Move(moveDirection * (currentSpeed * Time.deltaTime));

        // Apply gravity
        playerVelocity.y += GravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

}
