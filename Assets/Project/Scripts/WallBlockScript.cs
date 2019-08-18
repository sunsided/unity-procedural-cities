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