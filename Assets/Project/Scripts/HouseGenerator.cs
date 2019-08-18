using System;
using JetBrains.Annotations;
using UnityEngine;
using Random = UnityEngine.Random;

public class HouseGenerator : MonoBehaviour
{
    [SerializeField]
    private GameObject houseRoot;

    [Header("Grid shape")]
    [SerializeField]
    private int rows;

    [SerializeField]
    private int columns;

    [SerializeField]
    private int maxFloors;

    [SerializeField]
    private int stickiness = 1;

    [Header("Grid composition")]
    [SerializeField]
    private GameObject[] houseBlocks;

    private int[,,] _grid;

    public void StartBuildingHouse()
    {
        // TODO: Clear existing houses

        SetupGrid();
        GenerateHouse();
    }

    public void GenerateHouse()
    {
        var wallBlock = houseBlocks[Random.Range(0, houseBlocks.Length)];
        var bounds = GetBounds(wallBlock);

        var currentFloor = 0;
        while (currentFloor < maxFloors)
        {
            for (var row = 0; row < rows; ++row)
            {
                for (var column = 0; column < columns; ++column)
                {
                    if (_grid[row, column, currentFloor] <= 0) continue;

                    var spawnPosition = new Vector3(
                        row * bounds.x,
                        currentFloor * bounds.y,
                        column * bounds.z);
                    var spawnedBlock = Instantiate(wallBlock, spawnPosition, Quaternion.identity);

                    spawnedBlock.transform.parent = houseRoot.transform;

                    // Remove unused walls if they are not required.
                    var hasWestWall = (row == 0) || (_grid[row - 1, column, currentFloor] == 0);
                    var hasEastWall = (row == rows - 1) || (_grid[row + 1, column, currentFloor] == 0);
                    var hasNorthWall = (column == columns - 1) || (_grid[row, column + 1, currentFloor] == 0);
                    var hasSouthWall = (column == 0) || (_grid[row, column - 1, currentFloor] == 0);

                    var bl = spawnedBlock.GetComponent<WallBlockScript>();
                    bl.DestroyWestWallIf(!hasWestWall);
                    bl.DestroyEastWallIf(!hasEastWall);
                    bl.DestroyNorthWallIf(!hasNorthWall);
                    bl.DestroySouthWallIf(!hasSouthWall);

                    // TODO: Add roof if it's the top floor
                }
            }

            ++currentFloor;
        }
    }

    private Vector3 GetBounds([NotNull] GameObject obj)
    {
        var initialPos = obj.transform.position; // TODO: This one was included in the lessons, but seems unused.
        obj.transform.position = Vector3.zero;

        // Get the renderer of the containing object.
        var houseRenderer = obj.GetComponent<Renderer>();
        var combinedBounds = houseRenderer != null
            ? houseRenderer.bounds
            : new Bounds();

        // Accumulate the bounds of all active children.
        var renderers = obj.GetComponentsInChildren<Renderer>(false);
        foreach (var childRenderer in renderers)
        {
            combinedBounds.Encapsulate(childRenderer.bounds);
        }

        return combinedBounds.size;
    }

    private void SetupGrid()
    {
        _grid = new int[rows, columns, maxFloors];
        for (var floor = 0; floor < maxFloors; ++floor)
        {
            for (var row = 0; row < rows; ++row)
            {
                for (var column = 0; column < columns; ++column)
                {
                    var randomValue = Random.Range(0, stickiness);
                    if (randomValue <= 0) continue;
                    if (floor == 0 || _grid[row, column, floor - 1] > 0)
                    {
                        _grid[row, column, floor] = randomValue;
                    }
                }
            }
        }

        // Remove the non-connected blocks
        for (var floor = 0; floor < maxFloors; ++floor)
        {
            for (var row = 0; row < rows; ++row)
            {
                for (var column = 0; column < columns; ++column)
                {
                    var currentValue = _grid[row, column, floor];
                    if (currentValue != 0)
                    {
                        var cellWest = (row == 0) ? 0 : _grid[row - 1, column, floor];
                        var cellEast = (row == rows -1) ? 0 : _grid[row + 1, column, floor];
                        var cellNorth = (column == columns - 1) ? 0 : _grid[row, column + 1, floor];
                        var cellSouth = (column == 0) ? 0 : _grid[row, column - 1, floor];

                        var isConnected = cellWest > 0 || cellEast > 0 || cellNorth > 0 || cellSouth > 0;
                        if (!isConnected)
                        {
                            var isRoof = floor == 0 || _grid[row, column, floor - 1] == 0; // TODO: Shouldn't this be "floor + 1"?
                            if (isRoof)
                            {
                                _grid[row, column, floor] = 0;
                            }
                        }
                    }
                }
            }
        }
    }

    private void Start()
    {
        StartBuildingHouse();
    }
}