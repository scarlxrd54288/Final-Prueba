using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public TMP_Text pointsText;
    public TMP_Text stateText;
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
        stateText.text = $"E: {state}";
    }


    private void Update()
    {
        foreach (Transform child in cooldownContainer)
        {
            ObstacleCooldownIcon icon = child.GetComponent<ObstacleCooldownIcon>();
            if (icon != null)
                icon.UpdateCooldown();
        }
    }

}
