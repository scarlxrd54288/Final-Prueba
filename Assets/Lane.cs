using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Lane
{
    public Transform spawnPoint;
    public Vector3 direction = Vector3.forward;
    public float speed = 3f;
    public bool active = true;
}
