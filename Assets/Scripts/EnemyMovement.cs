using System.Collections;
using UnityEngine;
using UnityEngine.AI;


public class MovimientoEnemigoTorpe : MonoBehaviour
{
    [Header("Movimiento y Rotación")]
    [SerializeField] private float velocidadMovimiento = 2f;
    [SerializeField] private float velocidadRotacion = 2f; // Menor valor = más retardo/torpeza al girar
    [SerializeField] private float rangoAtaque = 1.5f;

    [Header("Configuración de Ataque")]
    [SerializeField] private float cadenciaAtaque = 2f;
    [SerializeField] private float retrasoDelGolpe = 0.5f;
    [SerializeField] private float descansoAtaque = 0.3f;
    private float proximoTiempoDeAtaque = 0f;
    private bool estaAtacando = false;


    [Header("Ángulo de Caminata (Torpeza)")]
    [Range(-1f, 1f)]
    [SerializeField] private float precisionDeFrente = 0.8f;

    [Header("Configuración de Objetivos")]
    [SerializeField] private float radioBusqueda = 15f;
    [SerializeField] private float margenHisteresis = 3.0f; 

    private Transform jugadorObjetivo;
    private Transform[] todosLosJugadores;
    private NavMeshAgent agente;

    void Start()
    {
        agente = GetComponent<NavMeshAgent>();
        agente.updateRotation = false;
        //Aca buscamos los jugadores para saber a donde mover el bicho
        GameObject[] objetosJugadores = GameObject.FindGameObjectsWithTag("Player");
        todosLosJugadores = new Transform[objetosJugadores.Length];

        for (int i = 0; i < objetosJugadores.Length; i++)
        {
            todosLosJugadores[i] = objetosJugadores[i].transform;
        }
    }

    void Update()
    {
        SeleccionarObjetivo();

        if (jugadorObjetivo != null)
        {
            MoverYRotarHaciaObjetivo();
        }
    }

    private void SeleccionarObjetivo()
    {
        //si no encontre nada, no hagas nada
        if (todosLosJugadores == null || todosLosJugadores.Length == 0) return;

        //reiniciamos variables 
        Transform jugadorMasCercano = null;
        float distanciaMinima = float.MaxValue;
        
        foreach (Transform jugador in todosLosJugadores)
        {
            //distancia entre yo y cada player
            float distancia = Vector3.Distance(transform.position, jugador.position);
            if (distancia < distanciaMinima)
            {
                distanciaMinima = distancia;
                jugadorMasCercano = jugador;
            }
        }

        //Si no esta a rango no voy corriendo hacia el, para no mandar a todos los enemigos de una xd
        if (distanciaMinima > radioBusqueda)
        {
            jugadorObjetivo = null;
            return;
        }

       // tonces...
        if (jugadorObjetivo == null)
        {
            //si no tenes objetivop, anda al mas cercano
            jugadorObjetivo = jugadorMasCercano;
        }
        else
        {
            //pero si ya lo fijaste, fijate que no halla otro ams cerca
            float distanciaObjetivoActual = Vector3.Distance(transform.position, jugadorObjetivo.position);
            if (distanciaMinima < distanciaObjetivoActual - margenHisteresis)
            {
                jugadorObjetivo = jugadorMasCercano;
            }
        }
    }

    private void MoverYRotarHaciaObjetivo()
    {

        if (estaAtacando) return;

        Vector3 direccion = jugadorObjetivo.position - transform.position;
        direccion.y = 0;

        float distancia = direccion.magnitude;
        Vector3 direccionNormalizada = direccion.normalized;
        float alineacion = Vector3.Dot(transform.forward, direccionNormalizada);

        if (direccion != Vector3.zero)
        {
            //si hay direccion... rotamos pero con embolia
            Quaternion rotacionObjetivo = Quaternion.LookRotation(direccion);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotacionObjetivo, velocidadRotacion * Time.deltaTime);
        }
    

        //si me alejo del player, me acerco
        if (distancia > rangoAtaque)
        {
            if (alineacion >= precisionDeFrente)
            {
                agente.speed = velocidadMovimiento;
                agente.SetDestination(jugadorObjetivo.position);
            }
            else
            {
                agente.ResetPath();
            }
        }
        else
        {
            agente.ResetPath();
            if (Time.time >= proximoTiempoDeAtaque && alineacion >= precisionDeFrente)
            {
                StartCoroutine(SecuenciaDeAtaque());
                proximoTiempoDeAtaque = Time.time + cadenciaAtaque;
            }
        }
    }

    private IEnumerator SecuenciaDeAtaque()
    {
        estaAtacando = true; 
        agente.ResetPath();

        Debug.Log("Casteando...");
        yield return new WaitForSeconds(retrasoDelGolpe);

        if (jugadorObjetivo != null)
        {
            Vector3 dirFinal = (jugadorObjetivo.position - transform.position);
            dirFinal.y = 0;

            // Comprobamos si sigue en rango y si sigue enfrente de su cara
            if (dirFinal.magnitude <= rangoAtaque && Vector3.Dot(transform.forward, dirFinal.normalized) >= precisionDeFrente)
            {
                Debug.Log($"Golpie a: {jugadorObjetivo.name}");
                //aca va lolgica de daño
            }
            else
            {
                Debug.Log("Fallo en ataque");
            }
        }
        yield return new WaitForSeconds(descansoAtaque);
        estaAtacando = false; 
    }

}