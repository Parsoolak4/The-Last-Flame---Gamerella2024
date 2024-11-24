using UnityEngine;

public class Unit : MonoBehaviour{

    [SerializeField] Movement movements;
    Vector2Int currPoint;
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
    public Movement Movements => movements;

    private void Start()
    {
        
    }

}

