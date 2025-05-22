using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    // Equipo de jugador e IA
    public static Equipo equipoJugador = new Equipo();
    public static Equipo equipoIA = new Equipo();

    // El texto de cuanto oro tiene el jugador
    [SerializeField] private TextMeshProUGUI textoOroJugador;

    [SerializeField] private TextMeshProUGUI textoCantidadUnidades;

    [SerializeField] private Edificio basePrincipalJugador;
    [SerializeField] private Edificio basePrincipalIA;

    // Referencia a la UI de Fin de Juego
    [SerializeField] private TextMeshProUGUI textoResultadoJuego;

    private float timerManejaMemoria = 0f;



    private void Start()
    {
        // Vincula el evento de cambio de oro a la función que actualiza el texto
        equipoJugador.onOroChanged += ActualizaTextoOroJugador;

        equipoJugador.oro = 100; // Inicializa el oro del jugador
        equipoIA.oro = 800;

        equipoIA.edificioSeleccionado = GameObject.Find("Torre enemiga").GetComponent<Edificio>();
        equipoIA.edificioSeleccionado.Seleccionar(equipoIA);

        // Inicializa la base principal del jugador
        equipoJugador.basePrincipal = basePrincipalJugador;

        // Inicializa la base principal de la IA
        equipoIA.basePrincipal = basePrincipalIA;

    }

    private void Update()
    {
        // Actualiza el objetivo de las unidades de la IA
        IaManager.SetObjetivoUnidades();

        if (timerManejaMemoria > 2f)
        {
            foreach (var unidad in equipoJugador.unidades)
            {
                if (unidad == null)
                {
                    equipoJugador.unidades.Remove(unidad);
                    Debug.Log("Unidad eliminada de la lista del jugador.");
                }
            }
            timerManejaMemoria += Time.deltaTime;
        }
    }

    private void LateUpdate()
    {
        textoCantidadUnidades.text = equipoJugador.unidades.Count.ToString();
    }

    private void ActualizaTextoOroJugador(int nuevoOro)
    {
        textoOroJugador.text = nuevoOro.ToString();
    }

    public static class IaManager
    {
        public static void SetObjetivoUnidades()
        {
            // Caso de atacar edificios si no hay unidades del jugador vivas

            if (equipoJugador.unidades.Count == 0)
            {
                Edificio[] edificios = GameObject.FindObjectsByType<Edificio>(FindObjectsSortMode.None);
                List<Edificio> edificiosJugador = new List<Edificio>();
                foreach (var edificio in edificios)
                {
                    if (edificio == null) continue;
                    if (!edificio.BControladoPorIA)
                    {
                        edificiosJugador.Add(edificio);
                    }
                }

                foreach (var aiUnit in equipoIA.unidades)
                {
                    if (aiUnit == null) continue;

                    float distanciaMinima = Mathf.Infinity;
                    foreach (var edificio in edificiosJugador)
                    {
                        if (edificio == null) continue;
                        float distancia = Vector3.Distance(aiUnit.transform.position, edificio.transform.position);
                        if (distancia < distanciaMinima)
                        {
                            distanciaMinima = distancia;
                            aiUnit.SetEntidadObjetivo(edificio.gameObject);
                        }
                    }

                }
            }


            // Caso de atacar otras unidades
            foreach (var aiUnit in equipoIA.unidades)
            {
                if (aiUnit == null) continue;


                aiUnit.SetEntidadObjetivo(FindClosestUnit(aiUnit.transform.position).gameObject);
            }
        }

        public static Unidad FindClosestUnit(Vector3 aiPosition)
        {
            Unidad closestPlayer = null;
            float shortestDistance = Mathf.Infinity;


            foreach (var player in equipoJugador.unidades)
            {
                if (player == null) continue;

                float distance = Vector3.Distance(aiPosition, player.transform.position);
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    closestPlayer = player;
                }
            }

            return closestPlayer;
        }

    }


    public void EndGame(Equipo equipoPerdedor)
    {
        Time.timeScale = 0f; // Pausar el juego


        if (equipoPerdedor == equipoJugador)
        {
            textoResultadoJuego.text = "¡Victoria!"; // Invertido a proposito por que es mas facil
            Debug.Log("¡Has perdido!");
        }
        else if (equipoPerdedor == equipoIA)
        {
            textoResultadoJuego.text = "¡Derrota!";
            Debug.Log("¡Has ganado!");
        }
        textoResultadoJuego.gameObject.SetActive(true); // Mostrar el texto de resultado
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; // Reanudar el tiempo
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Recargar la escena actual
    }

}
