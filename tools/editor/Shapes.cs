using System.Numerics;

namespace Editor;

// shapes in float[] vertices
public static class Shapes {
    public static Vector3[] CUBE_VERTICES = new Vector3[] {
        Vector3.Zero with { X = -0.5f, Y = -0.5f, Z = -0.5f },
        Vector3.Zero with { X = 0.5f, Y = -0.5f, Z = -0.5f },
        Vector3.Zero with { X = 0.5f, Y = 0.5f, Z = -0.5f },
        Vector3.Zero with { X = -0.5f, Y = 0.5f, Z = -0.5f },
        Vector3.Zero with { X = -0.5f, Y = -0.5f, Z = 0.5f },
        Vector3.Zero with { X = 0.5f, Y = -0.5f, Z = 0.5f },
        Vector3.Zero with { X = 0.5f, Y = 0.5f, Z = 0.5f },
        Vector3.Zero with { X = -0.5f, Y = 0.5f, Z = 0.5f }
    };
    
    public static Vector3[] cubeVertices() {
        Vector3[] res = new Vector3[CUBE_VERTICES.Length];
        Array.Copy(CUBE_VERTICES, res, CUBE_VERTICES.Length);

        return res;
    }
    
    public static uint[][] CUBE_INDICES = new uint[][] {
        new uint[] { 0, 1, 3, 3, 1, 2 }, // Front face
        new uint[] { 1, 5, 2, 2, 5, 6 }, // Right face
        new uint[] { 5, 4, 6, 6, 4, 7 }, // Back face
        new uint[] { 4, 0, 7, 7, 0, 3 }, // Left face
        new uint[] { 3, 2, 7, 7, 2, 6 }, // Top face
        new uint[] { 4, 5, 0, 0, 5, 1 } // Bottom face
    };

}