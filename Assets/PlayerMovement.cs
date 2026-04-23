using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;

    private Vector2 moveInput;
    private CharacterController controller;
    private Vector3 velocity;
    public float gravity = -9.81f;
    public float jumpHeight = 2f;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }
    public void OnJump(InputValue value)
    {
        if (value.isPressed && controller.isGrounded)
        {
            Jump();
        }
    }

    void Update()
    {
        Move();
        ApplyGravity();
    }

    void Move()
    {
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);

        // Ajuste tipo Beat 'em up (menos profundidad)
        move.z *= 0.5f;

        controller.Move(move * speed * Time.deltaTime);

        if (move != Vector3.zero)
        {
            transform.forward = move;
        }
    }
    void Jump()
    {
        velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }

    void ApplyGravity()
    {
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}