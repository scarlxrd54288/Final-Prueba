using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CarData", menuName = "Traffic/CarData")]
public class CarDataSO : ScriptableObject
{
    public GameObject prefab;
    public float baseDamage;
    public float speed;
    public Vector2Int size = new Vector2Int(2, 1);
}
