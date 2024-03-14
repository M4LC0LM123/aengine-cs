using System.Numerics;

namespace Editor; 

public class Vertex {
    public float x;
    public float y;
    public float z;

    public Vertex(float x, float y, float z) {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public Vertex() {
        x = 0;
        y = 0;
        z = 0;
    }

    public static Vertex Zero() {
        return new Vertex(0, 0, 0);
    }

    public Vector3 toVector3() {
        return Vector3.Zero with {
            X = x,
            Y = y,
            Z = z
        };
    }

    public Vertex fromVector3(Vector3 v) {
        x = v.X;
        y = v.Y;
        z = v.Z;

        return this;
    }
}