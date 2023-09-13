using System.Numerics;
using Raylib_CsLo;

namespace aengine.physics2D;

public sealed class PhysicsBody {
    private Vector2 m_position;
    private Vector2 m_linearVelocity;
    private float m_rotation;
    private float m_rotationalVelocity;

    private Vector2 m_force;

    public readonly float mass;
    public readonly float inverseMass;
    public readonly float density;
    public readonly float restitution;
    public readonly float area;

    public readonly bool isStatic;

    public readonly float radius;
    public readonly float width;
    public readonly float height;

    private readonly Vector2[] m_vertices;
    public readonly int[] triangles; // indices
    private readonly Vector2[] m_transformedVertices;
    private PhysicsAABB aabb;
    
    private bool m_transformUpdateRequired;
    private bool m_aabbUpdateRequired;

    public readonly PhysicsShape shape;

    public bool collisionEnabled = true;

    private PhysicsBody(Vector2 position, float mass,
        float density, float restitution, float area, bool isStatic, float radius, float width, float height,
        PhysicsShape shape) {
        m_position = position;
        m_linearVelocity = Vector2.Zero;
        m_rotation = 0;
        m_rotationalVelocity = 0;

        m_force = Vector2.Zero;

        this.mass = mass;
        this.density = density;
        this.restitution = restitution;
        this.area = area;

        this.isStatic = isStatic;

        this.radius = radius;
        this.width = width;
        this.height = height;

        this.shape = shape;

        if (!isStatic) {
            inverseMass = 1.0f / mass;
        } else {
            inverseMass = 0.0f;
        }

        if (this.shape is PhysicsShape.BOX) {
            m_vertices = createBoxVertices(width, height);
            triangles = triangulateBox();
            m_transformedVertices = new Vector2[m_vertices.Length];
        }
        else {
            m_vertices = null;
            m_transformedVertices = null;
        }
        
        m_transformUpdateRequired = true;
        m_aabbUpdateRequired = true;
    }

    internal void tick(float dt, Vector2 gravity, int iterations) {
        // Vector2 acceleration = m_force / mass;
        //
        // m_linearVelocity += acceleration * dt;

        if (isStatic) {
            return;
        }

        dt /= iterations;
        
        m_linearVelocity += gravity * dt;
        m_position += m_linearVelocity * dt;
        m_rotation += m_rotationalVelocity * dt;
        
        m_force = Vector2.Zero;
        m_transformUpdateRequired = true;
        m_aabbUpdateRequired = true;
    }

    public Vector2 getLinearVelocity() {
        return m_linearVelocity;
    }

    public void setLinearVelocity(Vector2 velocity) {
        m_linearVelocity = velocity;
    }
    
    public void applyForce(Vector2 amount) {
        m_force = amount;
    }

    public void move(Vector2 amount) {
        m_position += amount;
        m_transformUpdateRequired = true;
        m_aabbUpdateRequired = true;
    }

    public void rotate(float amount) {
        m_rotation += amount;
        m_transformUpdateRequired = true;
        m_aabbUpdateRequired = true;
    }

    public void setPosition(Vector2 position) {
        m_position = position;
        m_transformUpdateRequired = true;
        m_aabbUpdateRequired = true;
    }

    public Vector2 getPosition() {
        return m_position;
    }

    public float getRotation() {
        return m_rotation;
    }

    public PhysicsAABB getAABB() {
        if (m_aabbUpdateRequired) {
            float minX = 0;
            float minY = 0;
            float maxX = 0;
            float maxY = 0;
        
            if (shape is PhysicsShape.BOX) {
                for (int i = 0; i < getTransformedVertices().Length; i++) {
                    Vector2 v = getTransformedVertices()[i];

                    if (v.X < minX) minX = v.X;
                    if (v.X > maxX) maxX = v.X;
                    if (v.Y < minY) minY = v.Y;
                    if (v.Y > maxY) maxY = v.Y;
                }
            } else if (shape is PhysicsShape.CIRCLE) {
                minX = m_position.X - radius;
                minY = m_position.Y - radius;
                maxX = m_position.X + radius;
                maxY = m_position.Y + radius;
            } else {
                throw new Exception("Unknown shape!");
            }

            aabb = new PhysicsAABB(minX, minY, maxX, maxY);
        }

        m_aabbUpdateRequired = true;
        return aabb;
    }

    public static bool createCircleBody(float radius, Vector2 position, float density, bool isStatic,
        float restitution, out PhysicsBody body, out string errorMsg) {
        body = null;
        errorMsg = string.Empty;

        float area = radius * radius * MathF.PI;

        if (area < PhysicsWorld.minBodySize) {
            errorMsg = $"Circle radius is too small, minimum circle area is {PhysicsWorld.minBodySize}!";
            return false;
        }

        if (area > PhysicsWorld.maxBodySize) {
            errorMsg = $"Circle radius is too large, maximum circle area is {PhysicsWorld.maxBodySize}!";
            return false;
        }

        if (density < PhysicsWorld.minDensity) {
            errorMsg = $"Body density is too small, minimum body density is {PhysicsWorld.minBodySize}!";
            return false;
        }

        if (density > PhysicsWorld.maxDensity) {
            errorMsg = $"Body density is too large, maximum body density is {PhysicsWorld.maxBodySize}!";
            return false;
        }

        restitution = Math.Clamp(restitution, 0, 1);

        // mass = area * depth * density
        float mass = area * density / PhysicsWorld.massScalar;

        body = new PhysicsBody(position, mass, density, restitution, area, isStatic, radius, 0, 0, PhysicsShape.CIRCLE);

        return true;
    }

    public static bool createBoxBody(float width, float height, Vector2 position, float density, bool isStatic,
        float restitution, out PhysicsBody body, out string errorMsg) {
        body = null;
        errorMsg = string.Empty;

        float area = width * height;

        if (area < PhysicsWorld.minBodySize) {
            errorMsg = $"Box radius is too small, minimum box area is {PhysicsWorld.minBodySize}!";
            return false;
        }

        if (area > PhysicsWorld.maxBodySize) {
            errorMsg = $"Box radius is too large, maximum box area is {PhysicsWorld.maxBodySize}!";
            return false;
        }

        if (density < PhysicsWorld.minDensity) {
            errorMsg = $"Body density is too small, minimum body density is {PhysicsWorld.minBodySize}!";
            return false;
        }

        if (density > PhysicsWorld.maxDensity) {
            errorMsg = $"Body density is too large, maximum body density is {PhysicsWorld.maxBodySize}!";
            return false;
        }

        restitution = Math.Clamp(restitution, 0, 1);

        // mass = area * depth * density
        float mass = area * density / PhysicsWorld.massScalar;

        body = new PhysicsBody(position, mass, density, restitution, area, isStatic, 0, width, height,
            PhysicsShape.BOX);

        return true;
    }
    
    private static Vector2[] createBoxVertices(float width, float height) {
        float left = -width / 2; 
        float right = left + width;
        float bottom = -height / 2;
        float top = bottom + height;

        Vector2[] vertices = new Vector2[4];
        vertices[0] = new Vector2(left, top);
        vertices[1] = new Vector2(right, top);
        vertices[2] = new Vector2(right, bottom);
        vertices[3] = new Vector2(left, bottom);

        return vertices;
    }

    private static int[] triangulateBox() {
        int[] triangles = new int[6];
        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;
        triangles[3] = 0;
        triangles[4] = 2;
        triangles[5] = 3;

        return triangles;
    }

    public Vector2[] getTransformedVertices() {
        if (m_transformUpdateRequired) {
            PhysicsTransform physicsTransform = new PhysicsTransform(m_position, m_rotation);

            for (int i = 0; i < m_vertices.Length; i++) {
                Vector2 v = m_vertices[i];
                m_transformedVertices[i] = PhysicsUtils.transform(v, physicsTransform);
            }
        }

        m_transformUpdateRequired = false;
        return m_transformedVertices;
    }
    
}