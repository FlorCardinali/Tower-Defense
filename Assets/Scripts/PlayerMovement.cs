using JetBrains.Annotations;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float depthMultiplier = 0.5f;

    [Header("Jump & Gravity")]
    [SerializeField] private float jumpHeight = 1.8f;
    [SerializeField] private float gravity = -20f;

    private CharacterController controller;

    private Vector2 input;
    private Vector3 velocity; 
    private bool isGrounded;
 
    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        UpdateGroundedState();
        HandleMovement();
        HandleRotation();
        ApplyGravity();
    }

    public void OnMove(InputValue value)
    {
        input = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        if (value.isPressed && isGrounded)
        {
            Jump();
        }
    }

    private void HandleMovement()
    {
        Vector3 move = new Vector3(input.x, 0f, input.y);
        move.z *= depthMultiplier;
        Vector3 horizontalVelocity = move * moveSpeed;
        Vector3 finalVelocity = horizontalVelocity + velocity;
        controller.Move(finalVelocity * Time.deltaTime);
    }

    private void ApplyGravity()
    {
        if (isGrounded && velocity.y < 0f)
        {
            velocity.y = -2f; // mantener pegado al suelo
        }

        velocity.y += gravity * Time.deltaTime;
    }

    private void Jump()
    {
        velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }

    private void HandleRotation()
    {
        Vector3 moveDirection = new Vector3(input.x, 0f, input.y);

        if (moveDirection.sqrMagnitude > 0.01f)
        {
            transform.forward = moveDirection;
        }
    }

    private void UpdateGroundedState()
    {
        isGrounded = controller.isGrounded;
    }
}