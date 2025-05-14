using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Unit Data")]

public class UnitData : ScriptableObject
{
    public GameObject unitPrefab;
    public int cost;
    public string unitName;
    public Color spawnMessageColor = Color.yellow;
    public float spawnDelay = 3f;
}
