using JetBrains.Annotations;
using UnityEngine;

public class HouseBlockScript : MonoBehaviour
{
    [Header("Walls")]
    [SerializeField]
    private GameObject eastWall;

    [SerializeField]
    private GameObject westWall;

    [SerializeField]
    private GameObject northWall;

    [SerializeField]
    private GameObject southWall;

    [SerializeField]
    private GameObject floor;

    [SerializeField]
    private GameObject roof;

    [SerializeField]
    private GameObject roofAnchor;

    public Transform RoofAnchor => roofAnchor.transform;

    private enum WallDirection
    {
        North,
        East,
        South,
        West
    }

    [CanBeNull]
    public void ReplaceRandomWallWithPrefab([NotNull] GameObject prefab, [NotNull] Transform parent)
    {
        // Determine the available walls.
        var availableWalls = new WallDirection[4];
        var count = 0;

        if (northWall != null) availableWalls[count++] = WallDirection.North;
        if (eastWall != null) availableWalls[count++] = WallDirection.East;
        if (southWall != null) availableWalls[count++] = WallDirection.South;
        if (westWall != null) availableWalls[count++] = WallDirection.West;

        // Select a random wall.
        if (count == 0) return;
        var selectedWallIndex = Random.Range(0, count);

        // Instantiate and re-parent the replacement wall.
        var replacement = Instantiate(prefab, parent, true);

        // Replace the wall and return the original.
        GameObject originalWall;
        switch (availableWalls[selectedWallIndex])
        {
            default:
            {
                Debug.Log($"An unknown wall type was sampled: {availableWalls[selectedWallIndex]}");
                return;
            }

            case WallDirection.North:
            {
                originalWall = northWall;
                northWall = replacement;
                break;
            }

            case WallDirection.East:
            {
                originalWall = eastWall;
                eastWall = replacement;
                break;
            }

            case WallDirection.South:
            {
                originalWall = southWall;
                southWall = replacement;
                break;
            }

            case WallDirection.West:
            {
                originalWall = westWall;
                westWall = replacement;
                break;
            }
        }

        // Reposition the new wall.
        var tf = originalWall.transform;
        replacement.transform.position = tf.position;
        replacement.transform.rotation = tf.rotation;

        // Destroy the original wall.
        DestroyImmediate(originalWall);
    }

    public void AddWindowToRandomWall([NotNull] GameObject windowPrefab, [NotNull] Transform parent,
        float windowProbability)
    {
        // Note that the order of elements is important as we're going to switch
        // on the indices later.
        var walls = new[] { northWall, eastWall, southWall, westWall };

        for (var arrayIndex = 0; arrayIndex < walls.Length; ++arrayIndex)
        {
            if (Random.Range(0f, 1f) >= windowProbability) continue;

            var wall = walls[arrayIndex];
            if (wall == null) continue;

            // We do not want to position a window on top of a door.
            // Our heuristic here is that door walls have a child object (the door).
            var canHaveWindow = wall.transform.childCount == 0;
            if (!canHaveWindow) continue;

            var window = Instantiate(windowPrefab, parent, true);
            var windowSize = window.CalculateBounds();

            var tr = wall.transform;
            window.transform.position = tr.position;
            window.transform.rotation = tr.rotation;

            // Nudge the window outside the wall so that we can actually see it.
            const float nudgeFactor = 1.1f;
            switch (arrayIndex)
            {
                case 0: // North
                {
                    window.transform.position += new Vector3(0, 0, nudgeFactor * windowSize.z);
                    break;
                }

                case 1: // East
                {
                    window.transform.position += new Vector3(nudgeFactor * windowSize.z, 0, 0);
                    break;
                }

                case 2: // South
                {
                    window.transform.position += new Vector3(0, 0, -nudgeFactor * windowSize.z);
                    break;
                }

                case 3: // West
                {
                    window.transform.position += new Vector3(-nudgeFactor * windowSize.z, 0, 0);
                    break;
                }
            }
        }
    }

    public void SetRoof([NotNull] GameObject roofInstance, bool setParent)
    {
        // Re-parent if needed.
        // A Unity performance warning indicated that setting the parent immediately
        // after an Instantiate() call is inefficient, hence the check.
        if (setParent && roof.transform.parent != roofAnchor.transform)
        {
            roof.transform.parent = roofAnchor.transform;
        }

        roof = roofInstance;
    }

    public void DestroyNorthWallIf(bool shouldDestroy)
    {
        if (!shouldDestroy) return;
        DestroyImmediate(northWall);
    }

    public void DestroySouthWallIf(bool shouldDestroy)
    {
        if (!shouldDestroy) return;
        DestroyImmediate(southWall);
    }

    public void DestroyEastWallIf(bool shouldDestroy)
    {
        if (!shouldDestroy) return;
        DestroyImmediate(eastWall);
    }

    public void DestroyWestWallIf(bool shouldDestroy)
    {
        if (!shouldDestroy) return;
        DestroyImmediate(westWall);
    }
}