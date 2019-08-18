using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine;
using Debug = UnityEngine.Debug;

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

    [SerializeField]
    private GameObject[] wallsWithDoors;

    [SerializeField]
    private GameObject[] roofBlocks;

    [SerializeField]
    private GameObject[] windowBlocks;

    /// <summary>
    ///     The probability of a block to have a door.
    /// </summary>
    /// <remarks>
    ///     Note that a block can only have one door at most,
    ///     or no door at all. Doors are never created on pruned walls.
    /// </remarks>
    [Header("Appearance")]
    [SerializeField]
    [Range(0f, 1f)]
    private float blockDoorProbability = .4f;

    /// <summary>
    ///     The probability of a wall to have a window.
    /// </summary>
    [SerializeField]
    [Range(0f, 1f)]
    private float windowProbability = 0.5f;

    [SerializeField]
    private bool mixedRoofs;

    private int[,,] _grid;

    public bool HasRoot => houseRoot != null;

    public void RebuildHouses()
    {
        if (!HasRoot)
        {
            Debug.LogError("No root object was assigned.");
            return;
        }
        DestroyHouses();
        SetupGrid();
        GenerateHouse();
    }

    public void DestroyHouses()
    {
        // In edit mode, we can't use Destroy() since the delayed destruction
        // would never be invoked. (See https://docs.unity3d.com/ScriptReference/Object.DestroyImmediate.html)
        // However, we can't just destroy items from the enumerable we're iterating over,
        // so we need to collect the items into a collection first.
        var children = (
            from Transform child in houseRoot.transform
            select child.gameObject).ToList();

        // Note that outside the editor we're actually supposed to call Destroy
        // instead of DestroyImmediate.
        children.ForEach(DestroyImmediate);
    }

    private void GenerateHouse()
    {
        var wallBlock = houseBlocks[Random.Range(0, houseBlocks.Length)];
        var roofBlock = roofBlocks[Random.Range(0, roofBlocks.Length)];
        var wallBounds = wallBlock.CalculateBounds();

        for (var currentFloor = 0; currentFloor < maxFloors; ++currentFloor)
        {
            for (var row = 0; row < rows; ++row)
            {
                for (var column = 0; column < columns; ++column)
                {
                    var cellPosition = new CellPosition(row, column, currentFloor);
                    if (_grid[row, column, currentFloor] <= 0) continue;

                    // Select a new roof prefab.
                    if (mixedRoofs)
                    {
                        roofBlock = roofBlocks[Random.Range(0, roofBlocks.Length)];
                    }

                    // Create the block.
                    var spawnPosition = new Vector3(
                        row * wallBounds.x,
                        currentFloor * wallBounds.y,
                        column * wallBounds.z);
                    var spawnedBlock = Instantiate(wallBlock, spawnPosition, Quaternion.identity);
                    spawnedBlock.transform.parent = houseRoot.transform;

                    // Remove unused walls if they are not required.
                    var bl = spawnedBlock.GetComponent<HouseBlockScript>();
                    RemoveDoubleWalls(bl, cellPosition);

                    // Add roof if it's the top floor
                    AddRoofIfTopFloor(bl, roofBlock, cellPosition);

                    // Adding doors.
                    GenerateDoorIfGroundFloor(bl, spawnedBlock, cellPosition);

                    // Adding windows.
                    GenerateWindows(bl, spawnedBlock);
                }
            }
        }
    }

    private void AddRoofIfTopFloor(HouseBlockScript bl, GameObject roofBlock, in CellPosition pos)
    {
        if (pos.Floor != maxFloors - 1 && _grid[pos.Row, pos.Column, pos.Floor + 1] != 0) return;

        var roofInstance = Instantiate(roofBlock, bl.RoofAnchor, true);
        roofInstance.transform.localPosition = Vector3.zero;
        roofInstance.transform.localScale = new Vector3(1, 1, 1);

        bl.SetRoof(roofInstance, false);
    }

    private void RemoveDoubleWalls([NotNull] HouseBlockScript bl, in CellPosition pos)
    {
        var (row, column, floor) = pos;
        var hasWestWall = row == 0 || _grid[row - 1, column, floor] == 0;
        var hasEastWall = row == rows - 1 || _grid[row + 1, column, floor] == 0;
        var hasNorthWall = column == columns - 1 || _grid[row, column + 1, floor] == 0;
        var hasSouthWall = column == 0 || _grid[row, column - 1, floor] == 0;

        bl.DestroyWestWallIf(!hasWestWall);
        bl.DestroyEastWallIf(!hasEastWall);
        bl.DestroyNorthWallIf(!hasNorthWall);
        bl.DestroySouthWallIf(!hasSouthWall);
    }

    private void GenerateDoorIfGroundFloor([NotNull] HouseBlockScript bl, [NotNull] GameObject block,
        in CellPosition position)
    {
        if (position.Floor != 0 || !(Random.Range(0f, 1f) <= blockDoorProbability)) return;
        var wallPrefab = wallsWithDoors[Random.Range(0, wallsWithDoors.Length)];
        bl.ReplaceRandomWallWithPrefab(wallPrefab, block.transform);
    }

    private void GenerateWindows([NotNull] HouseBlockScript bl, [NotNull] GameObject block)
    {
        var windowPrefab = windowBlocks[Random.Range(0, windowBlocks.Length)];
        bl.AddWindowToRandomWall(windowPrefab, block.transform, windowProbability);
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
                    if (_grid[row, column, floor] == 0) continue;

                    var cellWest = row == 0 ? 0 : _grid[row - 1, column, floor];
                    var cellEast = row == rows - 1 ? 0 : _grid[row + 1, column, floor];
                    var cellNorth = column == columns - 1 ? 0 : _grid[row, column + 1, floor];
                    var cellSouth = column == 0 ? 0 : _grid[row, column - 1, floor];

                    // The first check determines whether there are connected components on
                    // the same level, i.e. in either horizontal direction.
                    // By keeping connected blocks, we're encouraging clustering - i.e. houses
                    // that are not just one block wide.
                    var isConnectedHorizontally = cellWest > 0 || cellEast > 0 || cellNorth > 0 || cellSouth > 0;
                    if (isConnectedHorizontally) continue;

                    // The next check ensures that horizontally disconnected blocks are only removed
                    // if they are either on the ground or if they are not on top of another block.
                    // If we remove this check, we enforce that that a floor must always be at least two blocks wide,
                    // preventing the formation of towers.
                    var hasNothingUnderneath = floor == 0 || _grid[row, column, floor - 1] == 0;
                    if (hasNothingUnderneath)
                    {
                        _grid[row, column, floor] = 0;
                    }
                }
            }
        }
    }

    [DebuggerDisplay("Row: {Row}, Column: {Column}, Floor: {Floor}")]
    private readonly struct CellPosition
    {
        public readonly int Row;

        public readonly int Column;

        public readonly int Floor;

        public CellPosition(int row, int column, int floor)
        {
            Debug.Assert(row >= 0, "row >= 0");
            Debug.Assert(column >= 0, "column >= 0");
            Debug.Assert(floor >= 0, "floor >= 0");
            Row = row;
            Column = column;
            Floor = floor;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Deconstruct(out int row, out int column, out int floor)
        {
            row = Row;
            column = Column;
            floor = Floor;
        }
    }
}