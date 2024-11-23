using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Unit", menuName = "Game/Unit")]
public class UnitData : ScriptableObject
{
    public Movement movements;

    [Serializable]
    public struct Movement {
        public Path [] types;
    }

    [Serializable]
    public struct Path {
        public Direction[] directions;
    }
    //[SerializeField] 

    public string itemName;
    public string description;
    public enum Direction
    {
        N, E, W, S, NE, NW, SE, SW
    }
    public int x;
    public int y;
    public Sprite icon;
    
    public void PrintItemInfo()
    {
        Debug.Log($"Item: {itemName}, X: {x}, Y:{y}");
    }
}