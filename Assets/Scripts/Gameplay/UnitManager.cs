using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;

public class UnitManager
{
    private List<Unit> units = new();

    public void Generate(UnitData[] unitDatas, Vector3 spawnOffset)
    {
        //, Vector3 spawnOffset
        // TODO : read unitDatas and spawn all each unit
        //GameObject unit = Instantiate(unitDatas[0].Prefab);
        //unitDatas[0].startPoints();
        Vector2Int startPoint = unitDatas[0].Startpoints[0];
        var unit = Object.Instantiate(unitDatas[0].Prefab, GameManager.Instance.Grid.Points[startPoint.x,startPoint.y].gameObject.transform.position - spawnOffset , Quaternion.identity).AddComponent<Unit>();

        units.Add(unit);

    }

    public void Move()
    {
        Unit saman = units[0];
        Path firstPath = saman.Movements.paths[0]; 
        foreach (Direction dir in firstPath.directions)
        {
            Vector3 newPosition = saman.transform.position;
           
            switch (dir)
            {
                case Direction.N:
                    break;
                case Direction.S:
                    break;
                case Direction.E:
                    break;
                case Direction.W:
                    break;
                case Direction.NE:
                    break;
                case Direction.NW:
                    break;
                case Direction.SE:
                    break;
                case Direction.SW:
                    break;
            }

            saman.transform.position = newPosition;
        }
    }