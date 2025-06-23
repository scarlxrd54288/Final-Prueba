using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public TMP_Text pointsText;
    public TMP_Text stateText;
    public TMP_Text timerText; // NUEVO campo para el tiempo
    public Transform cooldownContainer;

    private void Start()
    {
        PointManager.Instance.OnPointsUpdated += UpdatePoints;
        PointManager.Instance.OnGameStateChanged += UpdateState;
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

        // Mostrar tiempo restante si no ha terminado
        if (PointManager.Instance.currentState != PointManager.GameState.Victory &&
            PointManager.Instance.currentState != PointManager.GameState.GameOver)
        {
            float timeLeft = PointManager.Instance.GetTimeLeft();
            int minutes = Mathf.FloorToInt(timeLeft / 60f);
            int seconds = Mathf.FloorToInt(timeLeft % 60f);
            timerText.text = $"Tiempo: {minutes:00}:{seconds:00}";
        }
        else
        {
            timerText.text = ""; // Ocultar si ya terminó
        }
    }
}


