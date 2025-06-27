using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager Instance { get; private set; }

    public GameObject hud;
    public GameObject gameOverCanvas;
    public GameObject winCanvas;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // Solo si querés que persista entre escenas
    }

    public void ShowGameOver()
    {
        hud.SetActive(false);
        gameOverCanvas.SetActive(true);
    }

    public void ShowWin()
    {
        hud.SetActive(false);
        winCanvas.SetActive(true);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToHome()
    {
        SceneManager.LoadScene("Home");
    }
}

