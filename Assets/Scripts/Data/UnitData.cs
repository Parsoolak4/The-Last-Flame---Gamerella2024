using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Data/Unit", order = 1)]
public class UnitData : ScriptableObject
{
    [SerializeField] GameObject prefab;
    [SerializeField] UnitTypes.Movement movements;
    [SerializeField] Vector2Int[] startPoints;
    
    public GameObject Prefab => prefab;
    public UnitTypes.Movement Movements => movements;
    public Vector2Int[] Startpoints => startPoints;
    [Serializable]
    public struct Movement {
        public Path [] paths;
    }
    [Serializable]
    public struct Path {
        public Direction[] directions;
    }
    [SerializeField]
    public enum Direction
    {
        N, E, W, S, NE, NW, SE, SW
    }
}