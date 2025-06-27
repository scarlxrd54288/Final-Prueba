using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Texto y Timer")]
    public TMP_Text pointsText;
    public TMP_Text stateText;
    public Slider timerSlider;
    public TMP_Text timerText;

    [Header("Botones de Obstáculos")]
    public List<Image> obstacleButtons; // Imagen de cada botón

    [Header("Sprites compartidos")]
    public Sprite normalSprite;
    public Sprite evolvedSprite;

    [Header("Opcional: animador por botón")]
    public List<Animator> buttonAnimators;

    public Transform cooldownContainer;

    private float maxTime;

    [SerializeField] private Color evolvedColor = new Color(1f, 0.8f, 0.4f); // dorado suave
    [SerializeField] private float animationDuration = 0.3f;


    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        PointManager.Instance.OnObstacleEvolutionTriggered += UpdateButtonToEvolved;
    }

    private void Start()
    {
        PointManager.Instance.OnPointsUpdated += UpdatePoints;
        PointManager.Instance.OnGameStateChanged += UpdateState;

        maxTime = PointManager.Instance.GetMaxTime();
        timerSlider.maxValue = maxTime;
        timerSlider.minValue = 0;
    }

    private void Update()
    {
        foreach (Transform child in cooldownContainer)
        {
            ObstacleCooldownIcon icon = child.GetComponent<ObstacleCooldownIcon>();
            if (icon != null)
                icon.UpdateCooldown();
        }

        if (PointManager.Instance.currentState != PointManager.GameState.Victory &&
            PointManager.Instance.currentState != PointManager.GameState.GameOver)
        {
            float timeLeft = PointManager.Instance.GetTimeLeft();
            timerSlider.value = timeLeft;


            int minutes = Mathf.FloorToInt(timeLeft / 60f);
            int seconds = Mathf.FloorToInt(timeLeft % 60f);
            timerText.text = $"{minutes:00}:{seconds:00}";
        }
        else
        {
            timerSlider.gameObject.SetActive(false);
            timerText.gameObject.SetActive(false);
        }
    }


    private void UpdatePoints(int value)
    {
        pointsText.text = $"R: {value} $";
    }

    private void UpdateState(PointManager.GameState state)
    {
        switch (state)
        {
            case PointManager.GameState.Calm: stateText.text = "Estado: Tranquilo"; break;
            case PointManager.GameState.Wave1: stateText.text = "Estado: Oleada 1"; break;
            case PointManager.GameState.BetweenWaves: stateText.text = "Estado: Entre Oleadas"; break;
            case PointManager.GameState.Wave2: stateText.text = "Estado: Oleada 2"; break;
            case PointManager.GameState.Victory: stateText.text = "¡Victoria!"; break;
            case PointManager.GameState.GameOver: stateText.text = "¡Derrota!"; break;
        }
    }
    /*
    public void UpdateButtonToEvolved(int id)
    {
        if (id < 0 || id >= obstacleButtons.Count) return;
        //Sin animacion--------------------
        Button btn = obstacleButtons[id];
        
        Image img = btn.GetComponent<Image>();

        if (img != null && evolvedSprite != null)
        {
            img.sprite = evolvedSprite;
        
        }
        // Iniciar animación simple
        StartCoroutine(AnimateEvolveButton(btn));

    }*/
    public void UpdateButtonToEvolved(int id)
    {
        if (id < 0 || id >= obstacleButtons.Count) return;

        Image img = obstacleButtons[id];
        if (img != null && evolvedSprite != null)
        {
            img.sprite = evolvedSprite;
            StartCoroutine(AnimateEvolveButton(img));
        }
    }


    public void ResetAllButtonsToNormal()
    {
        foreach (var btn in obstacleButtons)
        {
            btn.sprite = normalSprite;
        }
    }


    //Animación Chafa --------------------------------
    private IEnumerator AnimateEvolveButton(Image img)
    {
        Transform t = img.transform;

        Vector3 originalScale = t.localScale;
        Vector3 targetScale = originalScale * 1.15f;

        Color originalColor = img.color;
        Color targetColor = evolvedColor;

        float time = 0f;
        float duration = animationDuration;

        while (time < duration)
        {
            float tLerp = time / duration;
            t.localScale = Vector3.Lerp(originalScale, targetScale, tLerp);
            img.color = Color.Lerp(originalColor, targetColor, tLerp);
            time += Time.deltaTime;
            yield return null;
        }

        time = 0f;
        while (time < duration)
        {
            float tLerp = time / duration;
            t.localScale = Vector3.Lerp(targetScale, originalScale, tLerp);
            img.color = Color.Lerp(targetColor, originalColor, tLerp);
            time += Time.deltaTime;
            yield return null;
        }

        t.localScale = originalScale;
        img.color = originalColor;
    }


}




