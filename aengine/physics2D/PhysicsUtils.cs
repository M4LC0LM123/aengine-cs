using System.Numerics;

namespace aengine.physics2D; 

public class PhysicsUtils {
    
    /// <summary>
    /// for nearlyEqual function
    /// half of a millimeter if a meter is defined as 1
    /// </summary>
    public static readonly float verySmallAmount = 0.0005f;
    
    public static Vector2 transform(Vector2 v, PhysicsTransform physicsTransform) {
        return new Vector2(
            physicsTransform.cos * v.X - physicsTransform.sin * v.Y + physicsTransform.x, 
            physicsTransform.sin * v.X + physicsTransform.cos * v.Y + physicsTransform.y);
    }

    public static float lengthSquared(Vector2 v) {
        return v.X * v.X + v.Y * v.Y;
    }

    public static bool nearlyEqual(float a, float b) {
        return MathF.Abs(a - b) < verySmallAmount;
    }

    public static bool nearlyEqual(Vector2 a, Vector2 b) {
        return nearlyEqual(a.X, b.X) && nearlyEqual(a.Y, b.Y);
    }
    
}