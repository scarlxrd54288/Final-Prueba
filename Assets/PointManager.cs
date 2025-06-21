using System;
using UnityEngine;

public class PointManager : MonoBehaviour
{
    public static PointManager Instance;

    public int currentPoints = 0;
    public ObjectDatabaseSO objectDatabase;

    public enum GameState { Calm, Wave1, BetweenWaves, Wave2, Victory }
    public GameState currentState = GameState.Calm;

    private float allLanesBlockedTime = 0f;

    // Eventos para UI y otros listeners
    public event Action<int> OnPointsUpdated;
    public event Action<GameState> OnGameStateChanged;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Update()
    {
        Debug.Log("Evaluando puntos...");
        Debug.Log($"Total puntos: {currentPoints}");
        AccumulatePoints();
        CheckUnlocks();
        UpdateCooldowns();
        UpdateGameState();
    }

    /*
    private float pointsAccumulator = 0f;
        private void AccumulatePoints()
    {
        int totalBlocked = 0;
        foreach (var lane in TrafficManager.Instance.lanes)
        {
            int blockedCount = lane.GetBlockedCarsCount();
            totalBlocked += blockedCount;
            float pointRate = 3f;
            pointsAccumulator += blockedCount * Time.deltaTime * pointRate;
        }

        int pointsGained = Mathf.FloorToInt(pointsAccumulator);
        if (pointsGained > 0)
        {
            currentPoints += pointsGained;
            pointsAccumulator -= pointsGained;
            Debug.Log($"Puntos ganados acumulados: {pointsGained}, Total puntos: {currentPoints}");
            OnPointsUpdated?.Invoke(currentPoints);
        }
    }
    */

    private float pointsAccumulator = 0f;

    private void AccumulatePoints()
    {
        float pointRate = 1f; // puntos por auto bloqueado por segundo
        int totalBlocked = 0;

        foreach (var lane in TrafficManager.Instance.lanes)
        {
            int blockedCount = lane.GetBlockedCarsCount();
            totalBlocked += blockedCount;
        }

        pointsAccumulator += totalBlocked * Time.deltaTime * pointRate;

        int pointsGained = Mathf.FloorToInt(pointsAccumulator);
        if (pointsGained > 0)
        {
            currentPoints += pointsGained;
            pointsAccumulator -= pointsGained;
            Debug.Log($"Puntos ganados acumulados: {pointsGained}, Total puntos: {currentPoints}");
            OnPointsUpdated?.Invoke(currentPoints);
        }
    }



    private void CheckUnlocks()
    {
        foreach (var obj in objectDatabase.objectsData)
        {
            if (!obj.Unlocked && currentPoints >= obj.UnlockCost)
            {
                obj.Unlocked = true;
                Debug.Log($"Obstacle {obj.Name} desbloqueado!");
            }
        }
    }

    private void UpdateCooldowns()
    {
        foreach (var obj in objectDatabase.objectsData)
        {
            if (obj.CooldownTimer > 0)
            {
                obj.CooldownTimer -= Time.deltaTime;
                if (obj.CooldownTimer < 0) obj.CooldownTimer = 0;
            }
        }
    }


    private void UpdateGameState()
    {
        GameState previousState = currentState;

        if ((currentPoints >= 100 || IsUnlocked(1)) && currentState == GameState.Calm)
        {
            currentState = GameState.Wave1;
            Debug.Log("Oleada 1 iniciada");
        }
        else if ((currentPoints >= 300 || IsUnlocked(3)) && currentState == GameState.BetweenWaves)
        {
            currentState = GameState.Wave2;
            Debug.Log("Oleada 2 iniciada");
        }

        if (AllLanesBlockedForSeconds(10) || currentPoints >= 500)
        {
            currentState = GameState.Victory;
            Debug.Log("¡Victoria!");
        }

        if (currentState != previousState)
        {
            OnGameStateChanged?.Invoke(currentState);
        }
    }

    private bool AllLanesBlockedForSeconds(float seconds)
    {
        bool allBlocked = true;
        foreach (var lane in TrafficManager.Instance.lanes)
        {
            if (!lane.IsFullyBlocked())
            {
                allBlocked = false;
                break;
            }
        }

        if (allBlocked)
        {
            allLanesBlockedTime += Time.deltaTime;
            if (allLanesBlockedTime >= seconds) return true;
        }
        else
        {
            allLanesBlockedTime = 0;
        }
        return false;
    }

    public bool IsUnlocked(int id)
    {
        var obj = objectDatabase.objectsData.Find(o => o.ID == id);
        if (obj == null) return false;
        return obj.Unlocked;
    }

    public bool CanPlaceObstacle(int id)
    {
        var obj = objectDatabase.objectsData.Find(o => o.ID == id);
        if (obj == null) return false;
        return obj.Unlocked && obj.CooldownTimer <= 0;
    }

    public void SetCooldown(int id)
    {
        var obj = objectDatabase.objectsData.Find(o => o.ID == id);
        if (obj == null) return;
        obj.CooldownTimer = obj.CooldownTime;
    }
}
