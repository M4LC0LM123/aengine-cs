using System.Numerics;

namespace aengine.physics2D; 

public static class Collisions {
    public static bool CheckCircleOverlap(Vector2 centerA, float radiusA, Vector2 centerB, float radiusB, out Vector2 normal, out float depth) {
        normal = Vector2.Zero;
        depth = 0;
        
        float distance = Vector2.Distance(centerA, centerB);
        float radii = radiusA + radiusB;

        if (distance >= radii) {
            return false;
        }

        normal = Vector2.Normalize(centerB - centerA);
        depth = radii - distance;

        return true;
    }
}