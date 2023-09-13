using System.Numerics;

namespace aengine.physics2D; 

public sealed class PhysicsWorld {
    public static readonly float minBodySize = 0.01f * 0.01f;
    public static readonly float maxBodySize = 1_000_000;

    // scales the mass because screen and physics world coordinates arent the same which means the mass of a rigidbody will be high
    internal static readonly float massScalar = 10_000;

    public static readonly float minDensity = 0.25f; // g/cm^3
    public static readonly float maxDensity = 21.4f;

    public static readonly int minIterations = 1;
    public static readonly int maxIterations = 128;

    private List<PhysicsBody> m_bodies;
    private List<PhysicsManifold> m_manifolds;
    private Vector2 m_gravity;

    public List<Vector2> contactPointList;

    public PhysicsWorld() {
        m_gravity = new Vector2(0, 981f);
        m_bodies = new List<PhysicsBody>();
        m_manifolds = new List<PhysicsManifold>();
        contactPointList = new List<Vector2>();
    }

    public PhysicsWorld(Vector2 gravity) {
        m_gravity = gravity;
        m_bodies = new List<PhysicsBody>();
        m_manifolds = new List<PhysicsManifold>();
        contactPointList = new List<Vector2>();
    }

    public int bodyCount() {
        return m_bodies.Count;
    }
    
    public void addBody(PhysicsBody body) {
        m_bodies.Add(body);
    }

    public bool removeBody(PhysicsBody body) {
        return m_bodies.Remove(body);
    }

    public bool getBody(int index, out PhysicsBody body) {
        body = null;
        
        if (index < 0 || index >= m_bodies.Count) {
            return false;
        }

        body = m_bodies[index];
        
        return true;
    }

    public void tick(float dt, int iterations = 15) {
        iterations = Math.Clamp(iterations, minIterations, maxIterations);
        
        contactPointList.Clear();

        for (int k = 0; k < iterations; k++) {
            // move tick
            for (int i = 0; i < m_bodies.Count; i++) {
                m_bodies[i].tick(dt, m_gravity, iterations);
            }
            
            m_manifolds.Clear();

            // collision tick
            for (int i = 0; i < m_bodies.Count - 1; i++) {
                PhysicsBody bodyA = m_bodies[i];
                PhysicsAABB bodyA_aabb = bodyA.getAABB();
                
                for (int j = i + 1; j < m_bodies.Count; j++) {
                    PhysicsBody bodyB = m_bodies[j];
                    PhysicsAABB bodyB_aabb = bodyB.getAABB();
                    
                    if (bodyA.isStatic && bodyB.isStatic) {
                        continue;
                    }

                    if (Collisions.checkCollisionAABB(bodyA_aabb, bodyB_aabb)) {
                        if (Collisions.collide(bodyA, bodyB, out Vector2 normal, out float depth) && bodyA.collisionEnabled && bodyB.collisionEnabled) {

                            if (bodyA.isStatic) {
                                bodyB.move(normal * depth);
                            } else if (bodyB.isStatic) {
                                bodyA.move(-normal * depth);
                            } else {
                                bodyA.move(-normal * depth / 2);
                                bodyB.move(normal * depth / 2);
                            }

                            Collisions.findContactPoints(bodyA, bodyB, out Vector2 contactOne, out Vector2 contactTwo, out int contactCount);
                            PhysicsManifold contact = new PhysicsManifold(bodyA, bodyB, normal, depth, contactOne, contactTwo, contactCount);
                            m_manifolds.Add(contact);
                        }   
                    }
                }
            }
        }
        
        for (int i = 0; i < m_manifolds.Count; i++) {
            PhysicsManifold manifold = m_manifolds[i];
            resolveCollision(in manifold);

            if (manifold.contactCount > 0) {
                contactPointList.Add(manifold.contactOne);
                
                if (manifold.contactCount > 1) {
                    contactPointList.Add(manifold.contactTwo);
                }
            }
        }
        
    }

    private void resolveCollision(in PhysicsManifold manifold) {
        Vector2 relativeVel = manifold.bodyB.getLinearVelocity() - manifold.bodyA.getLinearVelocity();

        if (Vector2.Dot(relativeVel, manifold.normal) > 0) {
            return;
        }
        
        float e = MathF.Min(manifold.bodyA.restitution, manifold.bodyB.restitution);
        
        float j = -(1 + e) * Vector2.Dot(relativeVel, manifold.normal);
        j /= manifold.bodyA.inverseMass + manifold.bodyB.inverseMass;

        Vector2 impulse = j * manifold.normal;
        
        manifold.bodyA.setLinearVelocity(manifold.bodyA.getLinearVelocity() - impulse * manifold.bodyA.inverseMass);
        manifold.bodyB.setLinearVelocity(manifold.bodyB.getLinearVelocity() + impulse * manifold.bodyB.inverseMass);
    }
    
    

}