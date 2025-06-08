using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CarData", menuName = "Traffic/CarData")]
public class CarDataSO : ScriptableObject
{
    public GameObject prefab;
    public float baseDamage;
    public float speed;
}
