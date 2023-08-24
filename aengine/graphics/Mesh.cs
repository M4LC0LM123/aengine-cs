using System.Numerics;

namespace aengine.graphics;

public class Mesh {
    public List<Vector3> vertices = new List<Vector3>();
    public List<Vector3> normals = new List<Vector3>();
    public List<Face> faces = new List<Face>();
    public List<Vector2> texCoords = new List<Vector2>();
    public Color color = Colors.WHITE;

    public static Mesh genCubeMesh(float width, float height, float length) {
        Mesh cube = new Mesh();

        // Define the eight vertices of the cube
        Vector3[] cubeVertices = {
            // Front face
            new Vector3(-1.0f, -1.0f, 1.0f),
            new Vector3(1.0f, -1.0f, 1.0f),
            new Vector3(1.0f, 1.0f, 1.0f),

            new Vector3(1.0f, 1.0f, 1.0f),
            new Vector3(-1.0f, 1.0f, 1.0f),
            new Vector3(-1.0f, -1.0f, 1.0f),

            // Back face
            new Vector3(-1.0f, -1.0f, -1.0f),
            new Vector3(1.0f, -1.0f, -1.0f),
            new Vector3(1.0f, 1.0f, -1.0f),

            new Vector3(1.0f, 1.0f, -1.0f),
            new Vector3(-1.0f, 1.0f, -1.0f),
            new Vector3(-1.0f, -1.0f, -1.0f),
                
            // Top face
            new Vector3(-1.0f, 1.0f, -1.0f),
            new Vector3(1.0f, 1.0f, -1.0f),
            new Vector3(1.0f, 1.0f, 1.0f),

            new Vector3(1.0f, 1.0f, 1.0f),
            new Vector3(-1.0f, 1.0f, 1.0f),
            new Vector3(-1.0f, 1.0f, -1.0f),

            // Bottom face
            new Vector3(-1.0f, -1.0f, -1.0f),
            new Vector3(1.0f, -1.0f, -1.0f),
            new Vector3(1.0f, -1.0f, 1.0f),

            new Vector3(1.0f, -1.0f, 1.0f),
            new Vector3(-1.0f, -1.0f, 1.0f),
            new Vector3(-1.0f, -1.0f, -1.0f),
            
            // Right face
            new Vector3(1.0f, -1.0f, -1.0f),
            new Vector3(1.0f, 1.0f, -1.0f),
            new Vector3(1.0f, 1.0f, 1.0f),

            new Vector3(1.0f, 1.0f, 1.0f),
            new Vector3(1.0f, -1.0f, 1.0f),
            new Vector3(1.0f, -1.0f, -1.0f),
            
            // Left face
            new Vector3(-1.0f, -1.0f, -1.0f),
            new Vector3(-1.0f, 1.0f, -1.0f),
            new Vector3(-1.0f, 1.0f, 1.0f),

            new Vector3(-1.0f, 1.0f, 1.0f),
            new Vector3(-1.0f, -1.0f, 1.0f),
            new Vector3(-1.0f, -1.0f, -1.0f),
            
        };

        // Define the normals for each face (assuming cube is centered at origin)
        Vector3[] cubeNormals = {
            new Vector3(0, 0, 1), // Front face normal
            new Vector3(0, 0, -1), // Back face normal
            new Vector3(0, 1, 0), // Top face normal
            new Vector3(0, -1, 0), // Bottom face normal
            new Vector3(1, 0, 0), // Right face normal
            new Vector3(-1, 0, 0), // Left face normal
        };

        // Define the faces of the cube using indices
        int[][] cubeFaces = {
            new int[] { 0, 1, 2, 3 }, // Front
            new int[] { 4, 5, 6, 7 }, // Back
            new int[] { 8, 9, 10, 11 }, // Top
            new int[] { 12, 13, 14, 15 }, // Bottom
            new int[] { 16, 17, 18, 19 }, // Right
            new int[] { 20, 21, 22, 23 } // Left
        };

        // Define texture coordinates for each vertex (unique for each vertex)
        Vector2[] texCoords = {
            // front face
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(1, 1),
            new Vector2(0, 1),
            
            // back face
            new Vector2(1, 0),
            new Vector2(1, 1),
            new Vector2(0, 1),
            new Vector2(0, 0),
            
            // top face
            new Vector2(0, 1),
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(1, 1),
            
            // bottom face
            new Vector2(1, 1),
            new Vector2(0, 1),
            new Vector2(0, 0),
            new Vector2(1, 0),
            
            // right face
            new Vector2(1, 0),
            new Vector2(1, 1),
            new Vector2(0, 1),
            new Vector2(0, 0),
            
            // left face
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(1, 1),
            new Vector2(0, 1),
        };

        for (int i = 0; i < 6; i++)
        {
            foreach (int vertexIndex in cubeFaces[i])
            {
                cube.vertices.Add(cubeVertices[vertexIndex]);
                cube.normals.Add(cubeNormals[i]);
                cube.texCoords.Add(texCoords[vertexIndex % 4]); // Use modulo to cycle through texCoords
            }

            // Add a face
            cube.faces.Add(new Face(cubeFaces[i].ToList()));
        }

        return cube;
    }
}