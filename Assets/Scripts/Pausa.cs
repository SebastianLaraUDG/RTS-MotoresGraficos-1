using UnityEngine;
using UnityEngine.Rendering;

/*
 * Este script esta en la camara y no en el canvas
 * ya que se usa al interactuar el jugador.
*/
public class Pausa : MonoBehaviour
{
    [SerializeField] private GameObject menuPausa;
    [SerializeField] private Volume postProcessVolBlur;
    private bool bIsPaused = false;

    private void Start()
    {
        // Oculta el menu de pausa al iniciar
        menuPausa.SetActive(false);
        // Desactiva el post process al iniciar
        postProcessVolBlur.enabled = false;
    }

    // Update is called once per frame
    private void Update()
    {
        if(Input.anyKey)
        {
            // Presiona Escape para ocultar o mostrar el menu de pausa.
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                bIsPaused = !bIsPaused;
                if (bIsPaused) PausaJuego();
                else ReanudaJuego();
            }
        }
    }

    // Pausa el juego y muestra el menu de pausa
    public void PausaJuego()
    {
        // Pausa el juego
        Time.timeScale = 0f;
        // Muestra el menu de pausa
        menuPausa.SetActive(true);
        
        postProcessVolBlur.enabled = true;
    }
    // Oculta el menu de pausa y reanuda el juego
    public void ReanudaJuego()
    {   
        // Reanuda el juego
        Time.timeScale = 1f;
        // Oculta el menu de pausa
        menuPausa.SetActive(false);

        postProcessVolBlur.enabled = false;
    }
}
