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
    private Animator animator;
    private PlayerAttack attackScript; // Referencia para escuchar el evento

    private Vector2 input;
    private Vector3 velocity;
    private bool isGrounded;
    private bool canMove = true; // Controla si puede moverse

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        attackScript = GetComponent<PlayerAttack>();
    }

    private void OnEnable()
    {
        // Nos suscribimos al evento cuando el objeto se activa
        if (attackScript != null)
        {
            attackScript.OnAttackStateChanged += UpdateMovementState;
        }
    }

    private void OnDisable()
    {
        // Fundamental: Nos desuscribimos para evitar errores
        if (attackScript != null)
        {
            attackScript.OnAttackStateChanged -= UpdateMovementState;
        }
    }

    // Esta función reacciona al grito del PlayerAttack
    private void UpdateMovementState(bool isAttacking)
    {
        canMove = !isAttacking;
    }

    private void Update()
    {
        UpdateGroundedState();
        HandleMovement();
        HandleRotation();
        ApplyGravity();
        UpdateAnimations();
    }

    public void OnMove(InputValue value)
    {
        input = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        if (value.isPressed && isGrounded && canMove) // Evita saltar mientras ataca
        {
            Jump();
        }
    }

    private void HandleMovement()
    {
        Vector3 move = Vector3.zero;

        if (canMove)
        {
            move = new Vector3(input.x, 0f, input.y);
            move.z *= depthMultiplier;
        }

        Vector3 horizontalVelocity = move * moveSpeed;
        Vector3 finalVelocity = horizontalVelocity + velocity;
        controller.Move(finalVelocity * Time.deltaTime);
    }

    private void ApplyGravity()
    {
        if (isGrounded && velocity.y < 0f)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
    }

    private void Jump()
    {
        velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }

    private void HandleRotation()
    {
        if (!canMove) return; 
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

    private void UpdateAnimations()
    {
        if (animator != null)
        {
            if (canMove)
            {
                animator.SetFloat("velocidad", input.magnitude);
            }
            else
            {
                // Si está atacando y no puede moverse, forzamos la velocidad a 0 
                // para que deje de reproducir la animación de correr
                animator.SetFloat("velocidad", 0f);
            }
        }
    }
}