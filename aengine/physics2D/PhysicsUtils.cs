using System.Numerics;

namespace aengine.physics2D; 

public class PhysicsUtils {
    public static Vector2 transform(Vector2 v, PhysicsTransform physicsTransform) {
        return new Vector2(
            physicsTransform.cos * v.X - physicsTransform.sin * v.Y + physicsTransform.x, 
            physicsTransform.sin * v.X + physicsTransform.cos * v.Y + physicsTransform.y);
    }

    public static float lengthSquared(Vector2 v) {
        return v.X * v.X + v.Y * v.Y;
    }
    
}