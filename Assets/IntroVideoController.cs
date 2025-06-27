using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class IntroVideoController : MonoBehaviour
{
    public VideoPlayer videoPlayer;       // Asigna desde el Inspector
    public GameObject menuCanvas;         // El Canvas del menú que aparece después

    private bool menuShown = false;

    void Start()
    {
        videoPlayer.loopPointReached += OnVideoEnd; // Se ejecuta al terminar el video
        menuCanvas.SetActive(false); // Asegúrate de que esté desactivado al inicio
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        if (!menuShown)
        {
            Debug.Log("Video terminado. Activando menú.");
            menuCanvas.SetActive(true);
            menuShown = true; // Para que no se repita si el video se reinicia
        }
    }

    public void GoToGame()
    {
        SceneManager.LoadScene("GameScene"); // Reemplaza con el nombre correcto
    }
}
