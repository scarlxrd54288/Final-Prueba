using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public TMP_Text pointsText;
    public TMP_Text stateText;

    // En lugar de texto, ahora usas un Slider
    public Slider timerSlider;

    public Transform cooldownContainer;

    private float maxTime;

    private void Start()
    {
        PointManager.Instance.OnPointsUpdated += UpdatePoints;
        PointManager.Instance.OnGameStateChanged += UpdateState;

        // Asume que el tiempo total de la partida está fijo al inicio
        maxTime = PointManager.Instance.GetMaxTime();
        timerSlider.maxValue = maxTime;
        timerSlider.minValue = 0;
    }

    private void UpdatePoints(int value)
    {
        pointsText.text = $"Puntos: {value}";
    }

    private void UpdateState(PointManager.GameState state)
    {
        switch (state)
        {
            case PointManager.GameState.Calm:
                stateText.text = "Estado: Tranquilo";
                break;
            case PointManager.GameState.Wave1:
                stateText.text = "Estado: Oleada 1";
                break;
            case PointManager.GameState.BetweenWaves:
                stateText.text = "Estado: Entre Oleadas";
                break;
            case PointManager.GameState.Wave2:
                stateText.text = "Estado: Oleada 2";
                break;
            case PointManager.GameState.Victory:
                stateText.text = "¡Victoria!";
                break;
            case PointManager.GameState.GameOver:
                stateText.text = "¡Derrota!";
                break;
        }
    }

    private void Update()
    {
        // Actualizar cooldowns visuales
        foreach (Transform child in cooldownContainer)
        {
            ObstacleCooldownIcon icon = child.GetComponent<ObstacleCooldownIcon>();
            if (icon != null)
                icon.UpdateCooldown();
        }

        // Actualizar slider si el juego sigue en curso
        if (PointManager.Instance.currentState != PointManager.GameState.Victory &&
            PointManager.Instance.currentState != PointManager.GameState.GameOver)
        {
            float timeLeft = PointManager.Instance.GetTimeLeft();
            timerSlider.value = timeLeft;
        }
        else
        {
            timerSlider.gameObject.SetActive(false); // Oculta si el juego termina
        }
    }
}



