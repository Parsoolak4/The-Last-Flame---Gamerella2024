using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Data/Unit", order = 1)]
public class UnitData : ScriptableObject
{
    [SerializeField] GameObject prefab;
    [SerializeField] Movement movements;

    [SerializeField] Vector2Int[] startPoints;

    public GameObject Prefab => prefab;
    public Movement Movements => movements;
    public Vector2Int[] Startpoints => startPoints;
    [Serializable]
    public struct Movement {
        public Path [] types;
    }
    [Serializable]
    public struct Path {
        public Direction[] directions;
    }

    public enum Direction
    {
        N, E, W, S, NE, NW, SE, SW
    }
}