using System.Numerics;

namespace aengine.physics2D; 

public readonly struct Transform2D {
    public readonly float x;
    public readonly float y;
    public readonly float cos;
    public readonly float sin;

    public readonly static Transform2D Zero = new Transform2D(0, 0, 0);

    public Transform2D(Vector2 position, float angle) {
        x = position.X;
        y = position.Y;

        cos = MathF.Cos(angle);
        sin = MathF.Sin(angle);
    }
    
    public Transform2D(float x, float y, float angle) {
        this.x = x;
        this.y = y;

        cos = MathF.Cos(angle);
        sin = MathF.Sin(angle);
    }
    
}