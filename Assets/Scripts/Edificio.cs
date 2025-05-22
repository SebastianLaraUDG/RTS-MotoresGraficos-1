using System.Collections;
using UnityEngine;

/*
 * Base de todos los edificios con comportamiento
 * de spawn de unidades
*/

[RequireComponent(typeof(Outline))]
public class Edificio : MonoBehaviour
{
    [SerializeField] private GameObject unidadPrefab;
    [HideInInspector] public bool bSeleccionado = false;
    [SerializeField] private bool bControladoPorIA = false;

    public bool BControladoPorIA
    {
        get => bControladoPorIA;
        set
        {
            bControladoPorIA = value;
        }
    }

    public bool bEsBasePrincipal = false;

    public float salud = 100f;

    [Tooltip("Cantidad de oro generado cada vez que se cumple un ciclo" +
        "Esto determinado por TiempoAccion")]
    public int cantidadOroGenerado = 1;

    private enum TipoEdificio
    {
        Unidades, Oro
    }
    [SerializeField] private TipoEdificio tipoEdificio;

    // Tiempo para que genere dinero o una unidad
    [SerializeField, Tooltip("Tiempo para que genere dinero o una unidad"), Min(0.1f)]
    private float tiempoAccion;

    // Donde se spawnean las unidades en caso de que el tipo sea Unidades
    [SerializeField] private Transform posSpawnUnidades;

    public Equipo equipoDuenio = null;

    private Outline outline;
    private IEnumerator coroutine;

    private void Awake()
    {
        coroutine = SpawnUnidad(tiempoAccion);
        outline = GetComponent<Outline>();
    }

    public void Seleccionar(Equipo equipo)
    {
        outline.enabled = true;
        bSeleccionado = true;
        Activar();
        equipoDuenio = equipo;
        print("Equipo duenio: " + equipoDuenio.ToString());
    }
    public void Deseleccionar()
    {
        outline.enabled = false;
        bSeleccionado = false;
        Desactivar();
    }

    // Activa la corutina de generacion de unidades/dinero
    private void Activar()
    {
        coroutine = SpawnUnidad(tiempoAccion);
        StartCoroutine(coroutine);
    }
    // Desactiva la corutina de generacion de unidades/dinero
    private void Desactivar()
    {
        StopCoroutine(coroutine);
        coroutine = null;
    }


    private IEnumerator SpawnUnidad(float tiempo)
    {
        if (equipoDuenio == null) yield return null;

        while (bSeleccionado && equipoDuenio.unidades.Count < equipoDuenio.maximoUnidades)
        {
            // Espera n segundos
            yield return new WaitForSeconds(tiempo);

            switch (tipoEdificio)
            {
                // En caso de que sea un edificio de unidades
                case TipoEdificio.Unidades:
                    if (equipoDuenio.oro < unidadPrefab.GetComponent<Unidad>().costoProduccion)
                    {
                        Debug.Log("No hay oro suficiente para crear la unidad");
                        continue;
                    }
                    // Crea el objeto
                    GameObject gameObject = Instantiate(unidadPrefab, posSpawnUnidades.position, transform.rotation);
                    Unidad unidad = gameObject.GetComponent<Unidad>();
                    // Despues de crear la unidad la hace avanzar un poco para que no se amontonen
                    if (unidad)
                    {
                        unidad.posObjetivo = unidad.transform.position + unidad.transform.forward * 5f;
                    }
                    // Agrega la unidad a la lista de unidades del equipo
                    equipoDuenio.unidades.Add(unidad);
                    // Quita la cantidad de oro
                    equipoDuenio.oro -= unidad.costoProduccion;
                    // La unidad es controlada por el equipo de la IA
                    if (equipoDuenio == GameManager.equipoIA)
                    {
                        unidad.tag = "Entidad Enemiga";
                        unidad.controladoPorIA = true;
                    }

                    break;

                // En caso de que sea un edificio de oro
                case TipoEdificio.Oro:
                    // TODO: Recuerda casos de jugador e IA
                    equipoDuenio.oro += cantidadOroGenerado;
                    break;
            }

        }
    }

    public void RecibirDanio(float cantidadDanio)
    {

        salud -= cantidadDanio;
        // Caso de muerte
        if (salud <= 0f)
        {
            Debug.Log("Me han matado " + gameObject.name.ToString());

            // Desasignalo
            // Esto generaba un error equipoDuenio.edificioSeleccionado = null;

            if (bEsBasePrincipal)
            {
                // Fin del juego
                FindAnyObjectByType<GameManager>().EndGame(equipoDuenio);
            }

            // Borralo
            Destroy(gameObject);
        }
    }

}
