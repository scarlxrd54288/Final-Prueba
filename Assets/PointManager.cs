using System;
using UnityEngine;

public class PointManager : MonoBehaviour
{
    public static PointManager Instance;

    public int currentPoints = 0;
    public ObjectDatabaseSO objectDatabase;

    public enum GameState { Calm, Wave1, BetweenWaves, Wave2, Victory, GameOver }
    public GameState currentState = GameState.Calm;

    private float allLanesBlockedTime = 0f;
    private float timer = 0f;
    [SerializeField] private float maxGameTime = 300f; // 5 minutos

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
        if (currentState == GameState.Victory || currentState == GameState.GameOver)
            return;

        timer += Time.deltaTime;

        AccumulatePoints();
        CheckUnlocks();
        CheckEvolutions();
        UpdateCooldowns();
        UpdateGameState();
    }

    private float pointsAccumulator = 0f;

    private void AccumulatePoints()
    {
        float pointRate = 1f;
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

        if (currentPoints >= 100 || IsUnlocked(1))
        {
            if (currentState == GameState.Calm)
            {
                currentState = GameState.Wave1;
            }
        }

        if ((currentPoints >= 300 || IsUnlocked(3)) && currentState == GameState.BetweenWaves)
        {
            currentState = GameState.Wave2;
        }

        if (AllLanesBlockedForSeconds(10) || currentPoints >= 500)
        {
            currentState = GameState.Victory;
        }

        if (timer >= maxGameTime)
        {
            currentState = GameState.GameOver;
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
        return obj != null && obj.Unlocked;
    }

    public bool CanPlaceObstacle(int id)
    {
        var obj = objectDatabase.objectsData.Find(o => o.ID == id);
        return obj != null && obj.Unlocked && obj.CooldownTimer <= 0;
    }

    public void SetCooldown(int id)
    {
        var obj = objectDatabase.objectsData.Find(o => o.ID == id);
        if (obj != null)
            obj.CooldownTimer = obj.CooldownTime;
    }

    public float GetTimeLeft()
    {
        return Mathf.Max(0, maxGameTime - timer);
    }

    public float GetMaxTime()
    {
        return maxGameTime;
    }
    
    
    public event Action<int> OnObstacleEvolutionTriggered; // ID del objeto

    private void CheckEvolutions()
    {
        foreach (var obj in objectDatabase.objectsData)
        {
            if (!obj.Evolved && currentPoints >= obj.EvolutionPoints)
            {
                obj.Evolved = true;
                OnObstacleEvolutionTriggered?.Invoke(obj.ID);

                // Buscar todos los obstáculos activos con ese ID y evolucionarlos
                foreach (var go in FindObjectsOfType<Obstacle>())
                {
                    if (go.PoolID == obj.ID) // Asegúrate que PoolID se haya asignado bien
                    {
                        go.Evolve(obj.EvolutionLevel, obj.BaseResistance, obj.EvolvedDamage, obj.Durability);
                        Debug.Log($"[PointManager] Objeto ID {obj.ID} evolucionado en escena con nuevo daño: {obj.EvolvedDamage}");
                    }
                }
            }

        }
    }


}

