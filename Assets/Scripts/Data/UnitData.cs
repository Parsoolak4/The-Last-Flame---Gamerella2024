using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Unit", menuName = "Game/Unit")]
public class UnitData : ScriptableObject
{
    [SerializeField] GameObject prefab;
    [SerializeField] Movement movements;

    public GameObject Prefab => prefab;
    public Movement Movements => movements;

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