using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
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
        var unit = Object.Instantiate(unitDatas[0].Prefab, GameManager.Instance.Grid[startPoint.x, startPoint.y].gameObject.transform.position, Quaternion.identity).AddComponent<Unit>();
        unit.Initialize(unitDatas[0].Movements);

        units.Add(unit);

    }

    public void Move()
    {
        Unit saman = units[0];
        List<UnitTypes.Path> validPaths = new List<UnitTypes.Path>();

        foreach (UnitTypes.Path path in saman.Movements.paths)
        {
            if (IsPathValid(saman, path))
            {
                validPaths.Add(path);
            }
        }

        if (validPaths.Count == 0)
        {
            Debug.Log("No valid paths available");
            return;
        }

        // Randomly select one of the valid paths
        int randomIndex = UnityEngine.Random.Range(0, validPaths.Count);
        UnitTypes.Path selectedPath = validPaths[randomIndex];

        // Execute the selected path
        ExecutePath(saman, selectedPath);
        
        //GameManager.Instance.Grid[currPoint.x, currPoint.y].Unit = saman;
        //saman.transform.position = GameManager.Instance.Grid[currPoint.x, currPoint.y].gameObject.transform.position;
        
    }



    private bool IsPathValid(Unit unit, UnitTypes.Path path)
    {
        Vector2Int finalPosition = unit.Index;

        // Simulate movement along the path
        //inefficient, make this better Mehrdad
        foreach (UnitTypes.Direction dir in path.directions)
        {
            Vector2Int nextPosition = finalPosition;

            switch (dir)
            {
                case UnitTypes.Direction.N:
                    nextPosition.y += 1;
                    break;
                case UnitTypes.Direction.S:
                    nextPosition.y -= 1;
                    break;
                case UnitTypes.Direction.E:
                    nextPosition.x += 1;
                    break;
                case UnitTypes.Direction.W:
                    nextPosition.x -= 1;
                    break;
                case UnitTypes.Direction.NE:
                    nextPosition.x += 1;
                    nextPosition.y += 1;
                    break;
                case UnitTypes.Direction.NW:
                    nextPosition.x -= 1;
                    nextPosition.y += 1;
                    break;
                case UnitTypes.Direction.SE:
                    nextPosition.x += 1;
                    nextPosition.y -= 1;
                    break;
                case UnitTypes.Direction.SW:
                    nextPosition.x -= 1;
                    nextPosition.y -= 1;
                    break;
            }

            // Check grid bounds
            if (nextPosition.x < 0 || nextPosition.x >= 4 ||
                nextPosition.y < 0 || nextPosition.y >= 8)
            {
                return false;
            }

            // Check collision with other units
            if (GameManager.Instance.Grid[nextPosition.x, nextPosition.y].Unit != null)
            {
                return false;
            }

            finalPosition = nextPosition;
        }

        return true;
    }

    private void ExecutePath(Unit unit, UnitTypes.Path path)
    {
        foreach (UnitTypes.Direction dir in path.directions)
        {
            Vector2Int currPoint = unit.Index;
            GameManager.Instance.Grid[currPoint.x, currPoint.y].Unit = null;

            switch (dir)
            {
                case UnitTypes.Direction.N:
                    currPoint.y += 1;
                    break;
                case UnitTypes.Direction.S:
                    currPoint.y -= 1;
                    break;
                case UnitTypes.Direction.E:
                    currPoint.x += 1;
                    break;
                case UnitTypes.Direction.W:
                    currPoint.x -= 1;
                    break;
                case UnitTypes.Direction.NE:
                    currPoint.x += 1;
                    currPoint.y += 1;
                    break;
                case UnitTypes.Direction.NW:
                    currPoint.x -= 1;
                    currPoint.y += 1;
                    break;
                case UnitTypes.Direction.SE:
                    currPoint.x += 1;
                    currPoint.y -= 1;
                    break;
                case UnitTypes.Direction.SW:
                    currPoint.x -= 1;
                    currPoint.y -= 1;
                    break;
            }

            unit.Index = currPoint;
            GameManager.Instance.Grid[currPoint.x, currPoint.y].Unit = unit;
            unit.transform.position = GameManager.Instance.Grid[currPoint.x, currPoint.y].gameObject.transform.position;
        }
    }
}