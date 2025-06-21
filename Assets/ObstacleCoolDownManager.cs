using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ObstacleCoolDownManager : MonoBehaviour
{
    public ObjectDatabaseSO objectDatabase;

    private List<ObjectData> obstacles;

    void Start()
    {
        obstacles = objectDatabase.objectsData;
    }

    void Update()
    {
        foreach (var obj in obstacles)
        {
            if (obj.CooldownTimer > 0)
            {
                obj.CooldownTimer -= Time.deltaTime;
                if (obj.CooldownTimer < 0)
                    obj.CooldownTimer = 0;
            }
        }
    }
}
