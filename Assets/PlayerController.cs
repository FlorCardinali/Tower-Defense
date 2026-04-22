using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public InputActionAsset inputAsset;
    public string esquema = "player1_teclado";
    public float velocidad = 7f;

    private InputAction moveAction;
    private Vector2 inputMovimiento;
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // Buscamos la acción original
        var originalAction = inputAsset.FindAction("movimiento");

        // Creamos una copia para no romper el archivo original
        moveAction = new InputAction();
        foreach (var binding in originalAction.bindings)
        {
            moveAction.AddBinding(binding);
        }

        // APLICAMOS EL FILTRO: Solo las teclas que coincidan con el esquema
        moveAction.bindingMask = InputBinding.MaskByGroup(esquema);

        moveAction.Enable();
    }

    void Update()
    {
        inputMovimiento = moveAction.ReadValue<Vector2>();
    }

    void FixedUpdate()
    {
        Vector3 mov = new Vector3(inputMovimiento.x, 0, inputMovimiento.y) * velocidad * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + mov);
        if (mov != Vector3.zero) transform.forward = mov;
    }
}