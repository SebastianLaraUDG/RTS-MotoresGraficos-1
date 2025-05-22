using System.Threading.Tasks.Sources;
using UnityEngine;
using UnityEngine.AI;

/*
 * Clase base de las unidades
 * que se dirigen hacia un punto
*/

[RequireComponent(typeof(NavMeshAgent), typeof(Outline))]
public abstract class Unidad : MonoBehaviour
{
    protected NavMeshAgent navMeshAgent;
    [HideInInspector] public Outline outline;

    // Tiempo entre cada ataque
    [Header("Gameplay")]
    
    protected float velocidadMovimiento { get => navMeshAgent.speed; set { navMeshAgent.speed = value; } }
    [SerializeField] protected float tiempoEntreAtaques = 2f; // Cooldown ataques
    [SerializeField] protected float danio = 2f;
    [SerializeField] protected float distanciaAceptableAtaque = 5f;
    [SerializeField, Min(0f), Tooltip("El costo de producir esta unidad en oro")] public int costoProduccion = 1;
    [Tooltip("Esta unidad sera controlada por IA?")] public bool controladoPorIA = false;

    // Tiempo transcurrido desde el ultimo ataque
    protected float tiempoTranscurridoUltimoAtaque = 0f;

    // Salud de la unidad
    public float salud = 10f;

    // El objeto al que debe atacar esta unidad
    protected GameObject entidadObjetivoActual = null;

    public Equipo equipoDuenio = null;

    [Header("Internos")]
    [Tooltip("NO modificar desde el editor. Debug Readonly")]
    public bool bEstaSeleccionada = false;

    protected Unidad ultimoEnemigoAtacado = null;
    protected Edificio ultimoEdificioAtacado = null;

    protected virtual void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.updateRotation = true;

        outline = GetComponent<Outline>();
        // Las unidades siempre inician como No Seleccionadas
        outline.enabled = false;
    }


    // Update is called once per frame
    protected virtual void Update()
    {

        if (entidadObjetivoActual == null) return;


        // Saber si la unidad llego a su objetivo
       /* float dist = navMeshAgent.remainingDistance;
        if (dist != Mathf.Infinity && navMeshAgent.pathStatus == NavMeshPathStatus.PathComplete && navMeshAgent.remainingDistance == 0f)
        {
            Debug.LogWarning("Termine mi trayecto");
        }
       */
        if(controladoPorIA)
        {
            entidadObjetivoActual = GameManager.IaManager.FindClosestUnit(transform.position).gameObject;
        }

    }


    /// <summary>
    /// Setea la entidad a la que la unidad debe atacar.
    /// </summary>
    /// <param name="entidadPorAtacar">Un edificio o unidad</param>
    public void SetEntidadObjetivo(GameObject entidadPorAtacar)
    {
        posObjetivo = entidadPorAtacar.transform.position;
        entidadObjetivoActual = entidadPorAtacar;
    }

    // La posicion a la que se dirige la unidad.
    public Vector3 posObjetivo
    {
        get => navMeshAgent.destination;
        set
        {
            navMeshAgent.destination = value;
        }
    }

    public void RecibeDanio(float danioRecibido)
    {
        salud -= danioRecibido;
        // Caso de muerte
        if (salud <= 0f)
        {
            Debug.Log("Me han matado");

            // Quitalo de la lista
            if (equipoDuenio != null && equipoDuenio.unidades.Contains(this))
            {
                equipoDuenio.DeseleccionaUnidad(this);
                equipoDuenio.unidades.Remove(this);
            }

            if (equipoDuenio != null && equipoDuenio.unidadesSeleccionadas.Contains(this))
            {
                equipoDuenio.DeseleccionaUnidad(this);
                equipoDuenio.unidadesSeleccionadas.Remove(this);
            }
            // Borralo
            Destroy(gameObject);
        }
    }

}
