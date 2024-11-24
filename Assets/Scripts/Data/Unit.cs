using System;
using UnityEngine;

public class Unit : MonoBehaviour{

    [SerializeField] UnitTypes.Movement movements;
    public Vector2Int Index { get; set; }

    public UnitTypes.Movement Movements => movements;

    public void Initialize(UnitTypes.Movement movementData)
    {
        movements = movementData;
    }
    
}

