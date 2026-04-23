using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Configuracion de Input")]
    public InputActionAsset inputAsset;
    public string esquema = "player1_teclado";

    [Header("Ajustes de Personaje")]
    public float velocidad = 7f;
    public float fuerzaSalto = 6f;

    private InputAction moveAction;
    private InputAction jumpAction;
    private Vector2 inputMovimiento;
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // Configuramos el Movimiento
        moveAction = ConfigurarAccion("movimiento");

        // Configuramos el Salto y le decimos que ejecute la funcion "Saltar" al presionar
        jumpAction = ConfigurarAccion("saltar");
        jumpAction.performed += ctx => Saltar();
    }

    // Funcion para filtrar las teclas segun el esquema (P1 o P2)
    InputAction ConfigurarAccion(string nombreAccion)
    {
        var original = inputAsset.FindAction(nombreAccion);
        var nueva = new InputAction();
        foreach (var b in original.bindings) nueva.AddBinding(b);

        nueva.bindingMask = InputBinding.MaskByGroup(esquema);
        nueva.Enable();
        return nueva;
    }

    void Saltar()
    {
        // Chequeo de suelo simple: solo salta si no se esta moviendo mucho en el eje Y
        if (Mathf.Abs(rb.linearVelocity.y) < 0.01f)
        {
            rb.AddForce(Vector3.up * fuerzaSalto, ForceMode.Impulse);
        }
    }

    void Update()
    {
        // Leee el movimiento cada frame
        inputMovimiento = moveAction.ReadValue<Vector2>();
    }

    void FixedUpdate()
    {
        // Aplicamos el movimiento fisico
        Vector3 mov = new Vector3(inputMovimiento.x, 0, inputMovimiento.y) * velocidad * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + mov);

        // Rotamos la capsula hacia donde camina
        if (mov.magnitude > 0.1f)
        {
            transform.forward = mov;
        }
    }
}