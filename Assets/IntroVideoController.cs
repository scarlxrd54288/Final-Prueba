using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class IntroVideoController : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public GameObject menuCanvas;

    private bool menuShown = false;
    private Animator menuAnimator;

    void Start()
    {
        videoPlayer.loopPointReached += OnVideoEnd;

        if (menuCanvas != null)
        {
            menuCanvas.SetActive(false);
            menuAnimator = menuCanvas.GetComponent<Animator>();
        }
        else
        {
            Debug.LogError("No se ha asignado el menuCanvas en el inspector.");
        }
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        if (!menuShown && menuCanvas != null)
        {
            Debug.Log("Video terminado. Activando men�.");
            menuCanvas.SetActive(true);

            if (menuAnimator != null)
            {
                menuAnimator.SetTrigger("Inicio");
            }
            else
            {
                Debug.LogWarning("No se encontr� Animator en menuCanvas.");
            }

            menuShown = true;
        }
    }

    public void GoToGame()
    {
        SceneManager.LoadScene("GameScene");
    }
}

