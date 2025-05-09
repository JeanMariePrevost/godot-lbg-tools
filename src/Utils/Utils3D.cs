using Godot;

namespace LBG.LBGTools.Utils;

/// <summary>
/// A helper class for various utility functions that are specific to 3D.
/// </summary>
public static class Utils3D {

    /// <summary>
    /// Determines whether a given screen point is "within" a given 3D area from a specific camera's perspective.
    /// </summary>
    /// <param name="screenPoint">The screen point in 2D coordinates to check.</param>
    /// <param name="area">The target <see cref="Area3D"/> to test for intersection.</param>
    /// <param name="camera">The <see cref="Camera3D"/> used to project the screen point into 3D space.</param>
    /// <param name="maxDistance">The maximum distance for the raycast in 3D space. Defaults to 1000 units.</param>
    /// <param name="maxHits">The maximum number of raycast hits to process. Defaults to 1 (first hit only). Increasing this allows checking for overlapping areas.</param>
    /// <returns> <c>true</c> if the screen point projects onto the specified <see cref="Area3D"/>; otherwise, <c>false</c>.</returns>
    public static bool IsScreenPointOverArea3D(Vector2 screenPoint, Area3D area, Camera3D camera, float maxDistance = 1000f, int maxHits = 1) {
        // build ray endpoints
        Vector3 from = camera.ProjectRayOrigin(screenPoint);
        Vector3 dir = camera.ProjectRayNormal(screenPoint);
        Vector3 to = from + dir * maxDistance;

        // prepare base query
        var spaceState = camera.GetWorld3D().DirectSpaceState;
        uint mask = area.CollisionLayer;
        var exclude = new Godot.Collections.Array<Rid>();
        var query = PhysicsRayQueryParameters3D.Create(from, to, mask);
        query.CollideWithAreas = true;
        query.CollideWithBodies = false;

        // iterative raycasts up to maxHits
        for (int i = 0; i < maxHits; i++) {
            query.Exclude = exclude;
            var result = spaceState.IntersectRay(query);
            if (!result.TryGetValue("collider", out var obj)) {
                break; // nothing hit, exit loop
            }

            if (obj.As<Area3D>() is Area3D hit) {
                if (hit == area) {
                    return true; // hit the area we are looking for
                }

                exclude.Add(hit.GetRid()); // hit something else, add it to the exclude list
            }
        }
        return false;
    }

    /// <summary>
    /// Checks if the cursor is "within" a given area in 3D space.
    /// </summary>
    /// <param name="area">The area to check against.</param>
    /// <param name="camera">The camera used to project the ray.</param>
    /// <param name="maxDistance">The maximum distance for the raycast.</param>
    /// <param name="maxHits">The maximum number of raycast hits to process. Defaults to 1 (first hit only). Increasing this allows checking for overlapping areas.</param>
    /// /// <returns><c>true</c> if the cursor is over the area; otherwise, <c>false</c>.</returns>
    public static bool IsCursorOverArea3D(Area3D area, Camera3D camera, float maxDistance = 1000f, int maxHits = 1) {
        Vector2 mousePos = camera.GetViewport().GetMousePosition();
        return IsScreenPointOverArea3D(mousePos, area, camera, maxDistance, maxHits);
    }

    /// <summary>
    /// Checks if a given point in 3D space is "within" a given 3D area from a specific camera's perspective.
    /// Works by unprojecting the point into screen space and then using the IsScreenPointOverArea3D method.
    /// </summary>
    /// <param name="pointIn3dSpace">The 3D point to check.</param>
    /// <param name="area">The target <see cref="Area3D"/> to test for intersection.</param>
    /// <param name="camera">The <see cref="Camera3D"/> used to project the point into screen space.</param>
    /// <param name="maxDistance">The maximum distance for the raycast in 3D space. Defaults to 1000 units.</param>
    /// /// <param name="maxHits">The maximum number of raycast hits to process. Defaults to 1 (first hit only). Increasing this allows checking for overlapping areas.</param>
    /// /// <returns> <c>true</c> if the point projects onto the specified <see cref="Area3D"/>; otherwise, <c>false</c>.</returns>
    public static bool IsUnprojectedPointOverArea3D(Vector3 pointIn3dSpace, Area3D area, Camera3D camera, float maxDistance = 1000f, int maxHits = 1) {
        Vector2 screenPoint = camera.UnprojectPosition(pointIn3dSpace);
        return IsScreenPointOverArea3D(screenPoint, area, camera, maxDistance, maxHits);
    }

    /// <summary>
    /// Projects the mouse cursor ray onto an arbitrary plane.
    /// Returns null if the ray cannot intersect the plane.
    /// </summary>
    /// <param name="planePoint">Any point on the plane.</param>
    /// <param name="planeNormal">The normal vector of the plane.</param>
    /// <param name="camera">The camera used to project the ray.</param>
    /// <param name="viewport">The viewport used to get the mouse position.</param>
    public static Vector3? GetCursorPositionOnPlane(Vector3 planePoint, Vector3 planeNormal, Viewport viewport) {
        var mousePos = viewport.GetMousePosition();
        var camera = viewport.GetCamera3D();
        var origin = camera.ProjectRayOrigin(mousePos);
        var direction = camera.ProjectRayNormal(mousePos);

        float denom = direction.Dot(planeNormal);
        if (Mathf.Abs(denom) < Mathf.Epsilon) {
            return null; // The ray is parallel to the plane, no intersection
        }

        float t = (planePoint - origin).Dot(planeNormal) / denom;
        return origin + direction * t; ;
    }

}
