using System.Numerics;

namespace aengine.physics2D; 

public readonly struct PhysicsManifold {
    public readonly PhysicsBody bodyA;
    public readonly PhysicsBody bodyB;
    public readonly Vector2 normal;
    public readonly float depth;
    public readonly Vector2 contactOne;
    public readonly Vector2 contactTwo;
    public readonly int contactCount;

    public PhysicsManifold(PhysicsBody bodyA, PhysicsBody bodyB, Vector2 normal, float depth, Vector2 contactOne, Vector2 contactTwo, int contactCount) {
        this.bodyA = bodyA;
        this.bodyB = bodyB;
        this.normal = normal;
        this.depth = depth;
        this.contactOne = contactOne;
        this.contactTwo = contactTwo;
        this.contactCount = contactCount;
    }
}