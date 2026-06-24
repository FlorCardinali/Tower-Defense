using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private float animationDuration = 0.5f;
    [SerializeField] private float cadence = 0.2f;
    [SerializeField] private Collider hitboxCollider;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip attackSound;

    // Nuestro evento para avisar a otros scripts
    public event Action<bool> OnAttackStateChanged;

    private Animator animator;
    private bool isAttacking;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        if (hitboxCollider != null) hitboxCollider.enabled = false;
    }

    public void OnAttack(InputValue value)
    {
        if (!isAttacking && value.isPressed)
        {
            StartCoroutine(Atacar());
        }
    }

    IEnumerator Atacar()
    {
        isAttacking = true;

        // Reproducimos el sonido de ataque al instante
        if (audioSource != null && attackSound != null)
        {
            audioSource.PlayOneShot(attackSound);
        }

        // Avisamos que el ataque empezó (frena el movimiento)
        OnAttackStateChanged?.Invoke(true);

        if (animator != null) animator.SetTrigger("ataque");
        if (hitboxCollider != null) hitboxCollider.enabled = true;

        yield return new WaitForSeconds(animationDuration);

        if (hitboxCollider != null) hitboxCollider.enabled = false;

        // Avisamos que el ataque terminó (devuelve el movimiento)
        OnAttackStateChanged?.Invoke(false);

        yield return new WaitForSeconds(cadence);
        isAttacking = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isAttacking)
        {
            if (other.CompareTag("enemigo"))
            {
                IDaniable objetoGolpeado = other.GetComponent<IDaniable>();
                if (objetoGolpeado != null)
                {
                    objetoGolpeado.tomarDanio(10);
                    Debug.Log("Golpe único a: " + other.name);
                }
            }
        }
    }
}