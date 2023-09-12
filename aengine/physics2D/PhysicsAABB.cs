using System.Numerics;

namespace aengine.physics2D; 

public readonly struct PhysicsAABB {
    public readonly Vector2 min;
    public readonly Vector2 max;

    public PhysicsAABB(Vector2 min, Vector2 max) {
        this.min = min;
        this.max = max;
    }
    
    public PhysicsAABB(float minX, float minY, float maxX, float maxY) {
        min = new Vector2(minX, minY);
        max = new Vector2(maxX, maxY);
    }
    
}