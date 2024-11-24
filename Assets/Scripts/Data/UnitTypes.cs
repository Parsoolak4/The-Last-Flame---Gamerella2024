using System;
using UnityEngine;

// Put shared types in a separate file
public static class UnitTypes
{
    [Serializable]
    public struct Movement
    {
        public Path[] paths;
    }

    [Serializable]
    public struct Path
    {
        public Direction[] directions;
    }

    public enum Direction
    {
        N, E, W, S, NE, NW, SE, SW
    }
}