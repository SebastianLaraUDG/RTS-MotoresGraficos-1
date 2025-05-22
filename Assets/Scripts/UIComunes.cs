using UnityEngine;

public class UIComunes : MonoBehaviour
{
    public void CerrarJuego()
    {
        // Cierra la aplicacion
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    public void CambiaEscena(string nombreEscena)
    {
        // Cambia la escena a la que se le pasa por parametro
        UnityEngine.SceneManagement.SceneManager.LoadScene(nombreEscena);
    }

    public void CambiaEscena(int indiceEscena)
    {
        // Cambia la escena a la que se le pasa por parametro
        UnityEngine.SceneManagement.SceneManager.LoadScene(indiceEscena);
    }
}