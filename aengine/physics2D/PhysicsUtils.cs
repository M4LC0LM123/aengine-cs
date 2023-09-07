using System.Numerics;

namespace aengine.physics2D; 

public class PhysicsUtils {
    public static Vector2 transform(Vector2 v, Transform2D transform) {
        return new Vector2(
            transform.cos * v.X - transform.sin * v.Y + transform.x, 
            transform.sin * v.X + transform.cos * v.Y + transform.y);
    }
}