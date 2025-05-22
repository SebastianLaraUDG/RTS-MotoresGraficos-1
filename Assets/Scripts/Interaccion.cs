using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Camera))]
public class Interaccion : MonoBehaviour
{
    [Header("Interaccion")]
    [SerializeField] private float distanciaMaxima = 100f;
    private Camera camara;


    [Header("Seleccion multiple de unidades")]
    // El recuadro de seleccion (una imagen del canvas)
    [SerializeField] private RectTransform recuadroDeSeleccion;

    private bool bMouseDown = false, bArrastrando = false;
    private Vector3 mousePosInicial;

    public Equipo equipoDuenio = GameManager.equipoJugador;

    private void Awake()
    {
        camara = GetComponent<Camera>();
    }

    private void Start()
    {
        // Oculta la recuadro de seleccion
        recuadroDeSeleccion.gameObject.SetActive(false);
    }

    private void Update()
    {
        // -----------------------------------------------------------------------------------------------------------------------------------------------------------------
        // SELECCION MULTIPLE DE UNIDADES POR AREA

        // Detecta el inicio de la seleccion
        if (Input.GetMouseButtonDown(2))
        {
            bMouseDown = true;
            mousePosInicial = Mouse.current.position.ReadValue();
            equipoDuenio.DeseleccionarUnidades();
        }
        // Se mantiene la seleccion?
        if (bMouseDown)
        {
            // Muestra el recuadro de seleccion si se movio el mouse una distancia minima
            if (!bArrastrando && Vector3.Distance(Mouse.current.position.ReadValue(), mousePosInicial) > 1f)
            {
                bArrastrando = true;
                recuadroDeSeleccion.gameObject.SetActive(true);
            }
            // En caso de estar arrastrando el mouse, cambia el tamanio del cuadro
            if (bArrastrando)
            {
                float anchoRecuadro = Mouse.current.position.ReadValue().x - mousePosInicial.x;
                float altoRecuadro = Mouse.current.position.ReadValue().y - mousePosInicial.y;
                // Usa absolutos ya que no puede haber ancho/alto negativos en una figura
                recuadroDeSeleccion.sizeDelta = new Vector2(Mathf.Abs(anchoRecuadro), Mathf.Abs(altoRecuadro));
                // Coloca el rectangulo entre la posicion inicial del clic y la posicion actual del mouse
                recuadroDeSeleccion.anchoredPosition = (mousePosInicial + Input.mousePosition) / 2;

                SeleccionaUnidadesRecuadro();
            }
        }
        if (Input.GetMouseButtonUp(2))
        {
            bMouseDown = false;
            bArrastrando = false;
            recuadroDeSeleccion.gameObject.SetActive(false);
        }
        // -----------------------------------------------------------------------------------------------------------------------------------------------------------------




        // -----------------------------------------------------------------------------------------------------------------------------------------------------------------
        // SELECCION INDIVIDUAL O MULTIPLE DE UNIDADES POR CLIC



        // Convierte la posicion del mouse en un rayo
        Ray rashoLaser = camara.ScreenPointToRay(Mouse.current.position.ReadValue());
        Debug.DrawRay(rashoLaser.origin, rashoLaser.direction * distanciaMaxima, Color.red);

        // Realiza el raycast
        if (!bArrastrando && Input.GetMouseButtonDown(0) && Physics.Raycast(rashoLaser, out RaycastHit hit, distanciaMaxima))
        {
            print("Objeto golpeado: " + hit.collider.name);

            // Caso de EDIFICIOS
            if (hit.collider.gameObject.CompareTag("Edificio"))
            {
                if (equipoDuenio.edificioSeleccionado == null)
                {
                    equipoDuenio.edificioSeleccionado = hit.collider.gameObject.GetComponent<Edificio>();
                    equipoDuenio.edificioSeleccionado.Seleccionar(equipoDuenio);
                }
                // En caso de que haga clic en el edificio ya seleccionado, lo deseleccionamos
                else
                {
                    equipoDuenio.edificioSeleccionado.Deseleccionar();
                    equipoDuenio.edificioSeleccionado = null;
                }
                // No necesitamos ejecutar lo siguiente si el objeto cliceado es un edificio
                return;
            }

            // CASO El objeto es de tipo unidad
            if (hit.collider.gameObject.CompareTag("Unidad"))
            {
                Unidad unidad = hit.collider.gameObject.GetComponent<Unidad>();
                if (unidad != null)
                {
                    ManejaSeleccion(unidad);
                }
            }
            // Establecer el objetivo de las unidades si es una superficie valida
            else if (hit.collider.gameObject.CompareTag("Superficie Navegacion"))
            {
                //  Limpia unidades muertas antes de procesarlas
                equipoDuenio.unidadesSeleccionadas.RemoveAll(u => u == null);

                foreach (Unidad unidadAct in equipoDuenio.unidadesSeleccionadas)
                {
                    if(unidadAct == null) continue;
                    try
                    {
                        print("Moviendo unidades...");
                        unidadAct.posObjetivo = hit.point;
                    }
                    catch (MissingReferenceException)
                    {
                        equipoDuenio.unidadesSeleccionadas.Remove(unidadAct);
                    }
                }
            }
            
            else if(hit.collider.gameObject.CompareTag("Entidad Enemiga"))
            {
                foreach(Unidad unid in equipoDuenio.unidadesSeleccionadas)
                {
                    unid.SetEntidadObjetivo(hit.collider.gameObject);
                }
            }
            // En otro caso deseleccionamos todas las unidades
            else
            {
                equipoDuenio.DeseleccionarUnidades();
            }
        }

        // -----------------------------------------------------------------------------------------------------------------------------------------------------------------
    }

    /// <summary>
    /// Funcion para seleccionar unidades por area
    /// (recuadro en pantalla)
    /// </summary>
    private void SeleccionaUnidadesRecuadro()
    {
        // Evitar continuar si no hay unidades vivas
        equipoDuenio.unidades.RemoveAll(u => u == null);

        if (equipoDuenio.unidades.Count < 1) return;

        // Posiciones limite del recuadro
        float izq = recuadroDeSeleccion.anchoredPosition.x - (recuadroDeSeleccion.sizeDelta.x / 2);
        float der = recuadroDeSeleccion.anchoredPosition.x + (recuadroDeSeleccion.sizeDelta.x / 2);
        float tope = recuadroDeSeleccion.anchoredPosition.y + (recuadroDeSeleccion.sizeDelta.y / 2);
        float fondo = recuadroDeSeleccion.anchoredPosition.y - (recuadroDeSeleccion.sizeDelta.y / 2);

        foreach (Unidad unidad in equipoDuenio.unidades)
        {
            // Convierte la posicion de la unidad a espacio en la pantalla 
            Vector3 posPantalla = camara.WorldToScreenPoint(unidad.transform.position);

            // La unidad esta dentro del recuadro?
            if (posPantalla.x > izq && posPantalla.x < der && posPantalla.y > fondo && posPantalla.y < tope)
            {
                equipoDuenio.SeleccionaUnidad(unidad);
            }
            // En caso de no estar dentro, la deselecciona
            else
            {
            equipoDuenio.DeseleccionaUnidad(unidad);
            }
        }
    }

    /// <summary>
    /// Funcion encargada de manejar la seleccion/deseleccion de unidades
    /// </summary>
    /// <param name="unid">La unidad detectada por un Raycast</param>
    private void ManejaSeleccion(Unidad unid)
    {
        // Modo de multiple seleccion
        if (Input.GetKey(KeyCode.LeftShift))
        {
            // En caso de que la unidad ya este seleccionada, debe deseleccionarse
            if (unid.bEstaSeleccionada)
            {
                equipoDuenio.DeseleccionaUnidad(unid);
            }
            // En caso de que NO este seleccionada, se selecciona
            else
            {
                  equipoDuenio.SeleccionaUnidad(unid);
            }
        }
        // Modo de seleccion individual
        else
        {
            // En caso de que ya este seleccionada la unidad y haya 2 o mas unidades ya seleccionadas
            // O no este seleccionada la unidad
            // seleccionamo SOLO a esa especificamente
            if ((unid.bEstaSeleccionada && equipoDuenio.unidadesSeleccionadas.Count >= 2) || !unid.bEstaSeleccionada)
            {
                equipoDuenio.DeseleccionarUnidades();
                equipoDuenio.SeleccionaUnidad(unid);
                return;
            }
            // O en caso de que la unidad este seleccionada y solo haya una (otra) unidad seleccionada
            // deselecciona esta
            equipoDuenio.DeseleccionaUnidad(unid);
        }
    }
}
