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
    [SerializeField] private float maxGameTime = 300f; 

    public event Action<int> OnPointsUpdated;
    public event Action<GameState> OnGameStateChanged;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        foreach (var obj in objectDatabase.objectsData)
        {
            obj.Unlocked = false;
            obj.Evolved = false;
            obj.CooldownTimer = 0f;
        }
        //VoiceOverSystem.Instance.PlaySaludo();

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
                if (obj.ID != 0)
                {
                    AudioManager.Instance.PlayUnlockSound();
                }
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

        //Wave1: 50 puntos
        if (currentPoints >= 50 && currentState == GameState.Calm)
        {
            currentState = GameState.Wave1;
        }

        //BetweenWaves: 150 puntos 
        if (currentPoints >= 100 && currentState == GameState.Wave1)
        {
            currentState = GameState.BetweenWaves;
        }

        //Wave2: 300 puntos
        if (currentPoints >= 200 && currentState == GameState.BetweenWaves)
        {
            currentState = GameState.Wave2;
            VoiceOverSystem.Instance.PlayEnojado();

        }

        //Victoria: 500 puntos
        //if (AllLanesBlockedForSeconds(10) || currentPoints >= 400)
        if (currentPoints >= 400 && currentState != GameState.Victory)
        {
            currentState = GameState.Victory;
            VoiceOverSystem.Instance.PlayWinVoice();
            GameUIManager.Instance.ShowWin();
            OnGameStateChanged?.Invoke(currentState);//ver------
            return; 
        }

        //Game Over 
        if (timer >= maxGameTime && currentState != GameState.GameOver)
        {
            currentState = GameState.GameOver;
            VoiceOverSystem.Instance.PlayGameOverVoice();
            GameUIManager.Instance.ShowGameOver();
            OnGameStateChanged?.Invoke(currentState);//verrr
            return;
        }

       
        if (currentState != previousState)
        {
            OnGameStateChanged?.Invoke(currentState);
        }
    }

    /*
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
    }*/

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
    
    
    public event Action<int> OnObstacleEvolutionTriggered; 

    private void CheckEvolutions()
    {
        foreach (var obj in objectDatabase.objectsData)
        {
            if (!obj.Evolved && currentPoints >= obj.EvolutionPoints)
            {
                obj.Evolved = true;
                OnObstacleEvolutionTriggered?.Invoke(obj.ID);

                foreach (var go in FindObjectsOfType<Obstacle>())
                {
                    if (go.PoolID == obj.ID) 
                    {
                        //go.Evolve(obj.EvolutionLevel, obj.BaseResistance, obj.EvolvedDamage, obj.Durability);
                        Debug.Log($"[PointManager] Objeto ID {obj.ID} evolucionado en escena con nuevo da�o: {obj.EvolvedDamage}");
                    }
                }
            }

        }
    }
    public bool IsEvolved(int id)
    {
        var obj = objectDatabase.objectsData.Find(o => o.ID == id);
        return obj != null && obj.Evolved;
    }

    public ObjectData GetObjectData(int id)
    {
        return objectDatabase.objectsData.Find(o => o.ID == id);
    }



}

