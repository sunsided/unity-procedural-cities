using JetBrains.Annotations;
using UnityEngine;

public class WallBlockScript : MonoBehaviour
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