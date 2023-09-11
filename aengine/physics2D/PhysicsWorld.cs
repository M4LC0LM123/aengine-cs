using System.Numerics;

namespace aengine.physics2D; 

public sealed class PhysicsWorld {
    public static readonly float minBodySize = 0.01f * 0.01f;
    public static readonly float maxBodySize = 1_000_000;

    // scales the mass because screen and physics world coordinates arent the same which means the mass of a rigidbody will be high
    internal static readonly float massScalar = 10_000;

    public static readonly float minDensity = 0.25f; // g/cm^3
    public static readonly float maxDensity = 21.4f;

    private List<RigidBody2D> bodies;
    private Vector2 gravity;

    public PhysicsWorld() {
        gravity = new Vector2(0, 981f);
        bodies = new List<RigidBody2D>();
    }

    public PhysicsWorld(Vector2 gravity) {
        this.gravity = gravity;
        bodies = new List<RigidBody2D>();
    }

    public int bodyCount() {
        return bodies.Count;
    }
    
    public void addBody(RigidBody2D body) {
        bodies.Add(body);
    }

    public bool removeBody(RigidBody2D body) {
        return bodies.Remove(body);
    }

    public bool getBody(int index, out RigidBody2D body) {
        body = null;
        
        if (index < 0 || index >= bodies.Count) {
            return false;
        }

        body = bodies[index];
        
        return true;
    }

    public void tick(float dt) {
        // move tick
        for (int i = 0; i < bodies.Count; i++) {
            bodies[i].tick(dt, gravity);
        }

        // collision tick
        for (int i = 0; i < bodies.Count - 1; i++) {
            RigidBody2D bodyA = bodies[i];
                
            for (int j = i + 1; j < bodies.Count; j++) {
                RigidBody2D bodyB = bodies[j];

                if (bodyA.isStatic && bodyB.isStatic) {
                    continue;
                }
                
                if (collide(bodyA, bodyB, out Vector2 normal, out float depth)) {

                    if (bodyA.isStatic) {
                        bodyB.move(normal * depth);
                    } else if (bodyB.isStatic) {
                        bodyA.move(-normal * depth);
                    } else {
                        bodyA.move(-normal * depth / 2);
                        bodyB.move(normal * depth / 2);
                    }
                    
                    resolveCollision(bodyA, bodyB, normal, depth);
                }
            }
        }
    }

    private void resolveCollision(RigidBody2D bodyA, RigidBody2D bodyB, Vector2 normal, float depth) {
        Vector2 relativeVel = bodyB.getLinearVelocity() - bodyA.getLinearVelocity();

        if (Vector2.Dot(relativeVel, normal) > 0) {
            return;
        }
        
        float e = MathF.Min(bodyA.restitution, bodyB.restitution);
        
        float j = -(1 + e) * Vector2.Dot(relativeVel, normal);
        j /= bodyA.inverseMass + bodyB.inverseMass;

        Vector2 impulse = j * normal;
        
        bodyA.setLinearVelocity(bodyA.getLinearVelocity() - impulse * bodyA.inverseMass);
        bodyB.setLinearVelocity(bodyB.getLinearVelocity() + impulse * bodyB.inverseMass);
    }
    
    private bool collide(RigidBody2D bodyA, RigidBody2D bodyB, out Vector2 normal, out float depth) {
        normal = Vector2.Zero;
        depth = 0;

        PhysicsShape physicsShapeA = bodyA.shape;
        PhysicsShape physicsShapeB = bodyB.shape;

        if (physicsShapeA is PhysicsShape.BOX) {
            if (physicsShapeB is PhysicsShape.BOX) {
                return Collisions.checkPolyOverlap(bodyA.getPosition(), bodyA.getTransformedVertices(), bodyB.getPosition(), bodyB.getTransformedVertices(),
                    out normal, out depth);
            }
            
            if (physicsShapeB is PhysicsShape.CIRCLE) {
                bool result = Collisions.checkPolyCircleOverlap(bodyB.getPosition(), bodyB.radius, bodyA.getPosition(),
                    bodyA.getTransformedVertices(), out normal, out depth);

                normal = -normal;
                return result;
            }
        } else if (physicsShapeA is PhysicsShape.CIRCLE) {
            if (physicsShapeB is PhysicsShape.BOX) {
                return Collisions.checkPolyCircleOverlap(bodyA.getPosition(), bodyA.radius, bodyB.getPosition(),
                    bodyB.getTransformedVertices(), out normal, out depth);
            }
            
            if (physicsShapeB is PhysicsShape.CIRCLE) {
                return Collisions.checkCircleOverlap(bodyA.getPosition(), bodyA.radius, bodyB.getPosition(),
                    bodyB.radius, out normal, out depth);
            }
        }

        return false;
    }

}