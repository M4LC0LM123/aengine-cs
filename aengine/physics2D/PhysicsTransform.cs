using System.Numerics;

namespace aengine.physics2D; 

public readonly struct PhysicsTransform {
    public readonly float x;
    public readonly float y;
    public readonly float cos;
    public readonly float sin;

    public readonly static PhysicsTransform Zero = new PhysicsTransform(0, 0, 0);

    public PhysicsTransform(Vector2 position, float angle) {
        x = position.X;
        y = position.Y;

        cos = MathF.Cos(angle);
        sin = MathF.Sin(angle);
    }
    
    public PhysicsTransform(float x, float y, float angle) {
        this.x = x;
        this.y = y;

        cos = MathF.Cos(angle);
        sin = MathF.Sin(angle);
    }
    
}