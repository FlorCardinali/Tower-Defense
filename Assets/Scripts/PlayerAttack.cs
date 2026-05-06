using System.Collections;
using System.Collections.Generic; // Necesario para usar List
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private float animationDuration = 0.5f;
    [SerializeField] private float cadence = 0.2f;
    [SerializeField] private Collider hitboxCollider;

    private MeshRenderer rend;
    private bool isAttacking;
    private Color colorOriginal;

    // Lista para guardar a quiénes ya golpeamos en el ataque actual para que el trigger no dispare multiples golpes
    private List<IDaniable> yaGolpeados = new List<IDaniable>();

    private void Awake()
    {
        rend = GetComponent<MeshRenderer>();
        colorOriginal = rend.material.color;
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
        yaGolpeados.Clear(); 

        rend.material.color = Color.yellow;
        if (hitboxCollider != null) hitboxCollider.enabled = true;

        yield return new WaitForSeconds(animationDuration);

        if (hitboxCollider != null) hitboxCollider.enabled = false;
        rend.material.color = colorOriginal;

        yield return new WaitForSeconds(cadence);
        isAttacking = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("toque algoo");
        if (isAttacking)
        {
            if (other.CompareTag("enemigo"))
            {
                IDaniable objetoGolpeado = other.GetComponent<IDaniable>();
                if (objetoGolpeado != null && !yaGolpeados.Contains(objetoGolpeado))
                {
                    objetoGolpeado.tomarDanio(10);
                    yaGolpeados.Add(objetoGolpeado); // Lo anotamos en la lista
                    Debug.Log("Golpe único a: " + other.name);
                }
            }
        }
    }
}