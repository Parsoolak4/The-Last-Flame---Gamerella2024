using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager
{
    private GridData gridData;
    private List<Unit> units = new();
    private Vector3 spawnOffset;

    public void Generate(GridData gridData, UnitData[] unitDatas, Vector3 spawnOffset) {
        if (unitDatas == null || unitDatas.Length == 0) return;

        this.gridData = gridData;
        this.spawnOffset = spawnOffset;

        // TODO : read unitDatas and spawn all each unit
        //GameObject unit = Instantiate(unitDatas[0].Prefab);
        //unitDatas[0].startPoints();
        //Vector2Int startPoint = unitDatas[0].Startpoints[0];
        //var unit = Object.Instantiate(unitDatas[0].Prefab, GameManager.Instance.Grid[startPoint.x, startPoint.y].gameObject.transform.position + spawnOffset, Quaternion.identity).AddComponent<Unit>();
        //GameManager.Instance.Grid[startPoint.x, startPoint.y].Unit = unit;
        //unit.Initialize(unitDatas[0].Movements);

        foreach (UnitData unitData in unitDatas) {
            // list of valid tiles
            List<Vector2Int> validStartPoints = new List<Vector2Int>();
            foreach (Vector2Int stPoint in unitData.Startpoints) {
                if (GameManager.Instance.Grid[stPoint.x, stPoint.y].Unit == null) {
                    validStartPoints.Add(stPoint);
                }
            }

            if (validStartPoints.Count == 0) {
                Debug.Log("No valid paths available");
                continue;
            }

            int r = UnityEngine.Random.Range(0, validStartPoints.Count);
            Vector2Int startPoint = validStartPoints[r];
            var element = GameObject.Instantiate(unitData.Prefab,
                GameManager.Instance.Grid[startPoint.x, startPoint.y].gameObject.transform.position + spawnOffset,
                Quaternion.identity).AddComponent<Unit>();
            element.Index = startPoint;
            GameManager.Instance.Grid[startPoint.x, startPoint.y].Unit = element;
            element.Initialize(unitData.Movements);  // Use unitData instead of unitDatas[0]

            units.Add(element);
        }
    }

    public void Clear() {
        units.Clear();
    }

    public IEnumerator Update(Action moveCallback) {
        foreach (var unit in units) {
            yield return Move(unit, moveCallback);
        }
        yield break;
    }

    private IEnumerator Move(Unit unit, Action moveCallback) {
        List<UnitTypes.Path> validPaths = new List<UnitTypes.Path>();

        foreach (UnitTypes.Path path in unit.Movements.paths) {
            if (IsPathValid(unit, path)) {
                validPaths.Add(path);
            }
        }

        if (validPaths.Count == 0) {
            //Debug.Log("No valid paths available");
            yield break;
        }

        // Randomly select one of the valid paths
        int randomIndex = UnityEngine.Random.Range(0, validPaths.Count);
        UnitTypes.Path selectedPath = validPaths[randomIndex];

        List<Vector2Int> tiles = selectedPathTiles(unit, selectedPath);

        foreach (var tile in tiles) {
            GameManager.AudioManager.PlayUnitMove();
            yield return MoveUnitToTile(unit, GameManager.Instance.Grid[tile.x, tile.y], GameManager.Instance.UnitMoveDuration);

            if (GameManager.Instance.Grid[tile.x, tile.y].Unit == GameManager.Instance.Player) {
                GameManager.Instance.EndGame(true);
            }

            GameManager.Instance.Grid[unit.Index.x, unit.Index.y].Unit = null;
            unit.Index = new(tile.x, tile.y);
            GameManager.Instance.Grid[tile.x, tile.y].Unit = unit;
            
            moveCallback();
        }

        //GameManager.Instance.Grid[unit.Index.x, unit.Index.y] = null;
        // Execute the selected path
        // ExecutePath(unit, selectedPath);

        //GameManager.Instance.Grid[currPoint.x, currPoint.y].Unit = saman;
        //saman.transform.position = GameManager.Instance.Grid[currPoint.x, currPoint.y].gameObject.transform.position;

    }

    private IEnumerator MoveUnitToTile(Unit unit, Tile tile, float unitMoveDuration) {
        float time = 0;
        Vector3 startPos = unit.transform.position;

        while (time <= unitMoveDuration) {
            unit.transform.position = Vector3.Lerp(startPos, tile.transform.position + spawnOffset, time / unitMoveDuration);
            time += Time.deltaTime;
            yield return null;
        }
        unit.transform.position = tile.transform.position + spawnOffset;
        yield break;
    }

    private List<Vector2Int> selectedPathTiles(Unit unit, UnitTypes.Path path) {

        List<Vector2Int> selectedPathTiles = new List<Vector2Int>();

        Vector2Int currPoint = unit.Index;

        foreach (UnitTypes.Direction dir in path.directions) {
            switch (dir) {
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

            selectedPathTiles.Add(currPoint);

        }
        return selectedPathTiles;
    }

    private bool IsPathValid(Unit unit, UnitTypes.Path path) {
        Vector2Int finalPosition = unit.Index;

        // Simulate movement along the path
        //inefficient, make this better Mehrdad
        foreach (UnitTypes.Direction dir in path.directions) {
            Vector2Int nextPosition = finalPosition;

            switch (dir) {
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
            if (nextPosition.x < 0 || nextPosition.x >= gridData.Width ||
                nextPosition.y < 0 || nextPosition.y >= gridData.Height) {
                return false;
            }

            // Check collision with other units
            if (GameManager.Instance.Grid[nextPosition.x, nextPosition.y].Unit != null) {
                if (GameManager.Instance.Grid[nextPosition.x, nextPosition.y].Unit != GameManager.Instance.Player) {
                    return false;
                }
            }

            finalPosition = nextPosition;
        }

        return true;
    }

    private void ExecutePath(Unit unit, UnitTypes.Path path) {
        foreach (UnitTypes.Direction dir in path.directions) {
            Vector2Int currPoint = unit.Index;

            GameManager.Instance.Grid[currPoint.x, currPoint.y].Unit = null;
            Debug.Log("unit is moving from ");

            Debug.Log(currPoint.x);
            Debug.Log(currPoint.y);
            switch (dir) {
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
            unit.transform.position = GameManager.Instance.Grid[currPoint.x, currPoint.y].gameObject.transform.position + spawnOffset;
        }
    }
}