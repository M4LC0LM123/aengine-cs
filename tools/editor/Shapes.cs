using System.Numerics;

namespace Editor;

// shapes in float[] vertices
public static class Shapes {
    public static Vector3[] CUBE_VERTICES = new Vector3[] {
        Vector3.Zero with { X = 0.5f, Y = -0.5f, Z = -0.5f },
        Vector3.Zero with { X = 0.5f, Y = 0.5f, Z = -0.5f },
        Vector3.Zero with { X = -0.5f, Y = 0.5f, Z = -0.5f },
        Vector3.Zero with { X = -0.5f, Y = -0.5f, Z = -0.5f },
        Vector3.Zero with { X = 0.5f, Y = -0.5f, Z = 0.5f },
        Vector3.Zero with { X = 0.5f, Y = 0.5f, Z = 0.5f },
        Vector3.Zero with { X = -0.5f, Y = -0.5f, Z = 0.5f },
        Vector3.Zero with { X = -0.5f, Y = 0.5f, Z = 0.5f }
    };
    
    public static Vector3[] cubeVertices() {
        Vector3[] res = new Vector3[CUBE_VERTICES.Length];
        Array.Copy(CUBE_VERTICES, res, CUBE_VERTICES.Length);

        return res;
    }

}