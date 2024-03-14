using System.Numerics;
using Raylib_CsLo;
using static Raylib_CsLo.Raylib;
using static Raylib_CsLo.RlGl;

namespace Editor;

public class ConvexHull {
    public float[] vertices;
    public uint[] indices;
    public Vector3 position;
    public Vector3 rotation;
    public Vector3 scale;
    public List<Vector3> vertexGrabbers;

    public ConvexHull() {
        vertices = Shapes.CUBE_VERTICES;
        indices = Shapes.CUBE_INDICES;
        
        position = Vector3.Zero;
        rotation = Vector3.Zero;
        scale = Vector3.One;

        vertexGrabbers = new List<Vector3>();
        
        for (var i = 0; i < vertices.Length; i += 3) {
            vertexGrabbers.Add(Vector3.Zero with {
                X = vertices[i],
                Y = vertices[i + 1],
                Z = vertices[i + 2]
            });
        }
    }

    public ConvexHull(float[] vertices, uint[] indices) {
        this.vertices = vertices;
        this.indices = indices;
        
        position = Vector3.Zero;
        rotation = Vector3.Zero;
        scale = Vector3.One;

        vertexGrabbers = new List<Vector3>();
        
        for (var i = 0; i < vertices.Length; i += 3) {
            vertexGrabbers.Add(Vector3.Zero with {
                X = vertices[i],
                Y = vertices[i + 1],
                Z = vertices[i + 2]
            });
        }
    }

    public Vector3 getVertexPos(int id) {
        return vertexGrabbers[id];
    }

    public void setVertexPos(int id, Vector3 pos) {
        if (id > vertexGrabbers.Count - 1 || id < 0) return;
        
        vertexGrabbers[id] = pos;
        
        updateVertices(id);
    }

    private void updateVertices(int id) {
        vertices[id * 3] = vertexGrabbers[id].X;
        vertices[id * 3 + 1] = vertexGrabbers[id].Y;
        vertices[id * 3 + 2] = vertexGrabbers[id].Z;
    }

    public void render(Color color) {
        rlPushMatrix();
        
        rlTranslatef(position.X, position.Y, position.Z);
        rlRotatef(rotation.X, 1, 0, 0);
        rlRotatef(rotation.Y, 0, 1, 0);
        rlRotatef(rotation.Z, 0, 0, 1);
        rlScalef(scale.X, scale.Y, scale.Z);
        
        rlBegin(RL_LINES);
        rlColor4ub(color.r, color.g, color.b, color.a);
        
        for (int i = 0; i < indices.Length; i += 2) {
            rlVertex3f(vertices[3 * indices[i]], vertices[3 * indices[i] + 1], vertices[3 * indices[i] + 2]);
            rlVertex3f(vertices[3 * indices[i + 1]], vertices[3 * indices[i + 1] + 1], vertices[3 * indices[i + 1] + 2]);
        }

        rlEnd();
        
        Console.WriteLine(vertexGrabbers.Count);
        
        foreach (Vector3 vertexGrabber in vertexGrabbers) {
            DrawSphereWires(vertexGrabber, 0.25f, 16, 16, RED);
        }
        
        rlPopMatrix();
    }

    public void renderXZ(Color color) {
        rlPushMatrix();
        
        rlTranslatef(position.X, position.Z, 0);
        rlRotatef(rotation.Y, 0, 0, 1);
        rlScalef(scale.X * 25, scale.Z * 25, 0);
        
        rlBegin(RL_LINES);
        rlColor4ub(color.r, color.g, color.b, color.a);
        
        for (int i = 0; i < indices.Length; i += 2) {
            rlVertex2f(vertices[3 * indices[i]], vertices[3 * indices[i] + 2]);
            rlVertex2f(vertices[3 * indices[i + 1]], vertices[3 * indices[i + 1] + 2]);
        }

        rlEnd();
        
        foreach (Vector3 vertexGrabber in vertexGrabbers) {
            DrawCircleLines((int)vertexGrabber.X, (int)vertexGrabber.Z, 0.25f, RED);
        }
        
        rlPopMatrix();
    }

    public ConvexHull copy() {
        ConvexHull res = new ConvexHull(vertices, indices);
        res.position = RayMath.Vector3AddValue(position, 0.5f);
        res.rotation = rotation;
        res.scale = scale;

        return res;
    }
}