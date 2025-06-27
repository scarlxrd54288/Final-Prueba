using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager Instance { get; private set; }

    [Header("UI Canvases")]
    public GameObject hud;
    public GameObject gameOverCanvas;
    public GameObject winCanvas;
    public GameObject pauseCanvas;

    [Header("Audio Toggles")]
    public Image musicToggleImage;
    public Sprite musicOnSprite;
    public Sprite musicOffSprite;

    public Image sfxToggleImage;
    public Sprite sfxOnSprite;
    public Sprite sfxOffSprite;

    private bool isPaused = false;
    private bool musicEnabled = true;
    private bool sfxEnabled = true;

    private float originalMusicVolume;
    private float originalSFXVolume;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        // Guardar los valores originales definidos en el AudioManager
        originalMusicVolume = AudioManager.Instance.musicVolume;
        originalSFXVolume = AudioManager.Instance.sfxVolume;

        // Aplicar sprites iniciales
        UpdateMusicIcon();
        UpdateSFXIcon();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !gameOverCanvas.activeSelf && !winCanvas.activeSelf)
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }

    public void ShowGameOver()
    {
        hud.SetActive(false);
        pauseCanvas.SetActive(false);
        gameOverCanvas.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ShowWin()
    {
        hud.SetActive(false);
        pauseCanvas.SetActive(false);
        winCanvas.SetActive(true);
        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToHome()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Home");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void PauseGame()
    {
        isPaused = true;
        pauseCanvas.SetActive(true);
        hud.SetActive(false);
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        isPaused = false;
        pauseCanvas.SetActive(false);
        hud.SetActive(true);
        Time.timeScale = 1f;
    }

    public void ToggleMusic()
    {
        musicEnabled = !musicEnabled;
        AudioManager.Instance.SetMusicVolume(musicEnabled ? originalMusicVolume : 0f);
        UpdateMusicIcon();
    }

    public void ToggleSFX()
    {
        sfxEnabled = !sfxEnabled;
        AudioManager.Instance.SetSFXVolume(sfxEnabled ? originalSFXVolume : 0f);
        UpdateSFXIcon();
    }

    private void UpdateMusicIcon()
    {
        musicToggleImage.sprite = musicEnabled ? musicOnSprite : musicOffSprite;
    }

    private void UpdateSFXIcon()
    {
        sfxToggleImage.sprite = sfxEnabled ? sfxOnSprite : sfxOffSprite;
    }
}



