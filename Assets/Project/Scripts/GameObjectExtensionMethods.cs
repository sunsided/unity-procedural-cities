using JetBrains.Annotations;
using UnityEngine;

internal static class GameObjectExtensionMethods
{
    public static Vector3 CalculateBounds([NotNull] this GameObject obj)
    {
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
}