using System.Numerics;
using Raylib_CsLo;
using static Raylib_CsLo.Raylib;
using static Raylib_CsLo.RlGl;

namespace Editor;

public class ConvexHull {
    public Vector3[] vertices;
    public Vector3 position;
    public Vector3 rotation;
    public Vector3 scale;

    public ConvexHull() {
        vertices = Shapes.cubeVertices();

        position = Vector3.Zero;
        rotation = Vector3.Zero;
        scale = Vector3.One;
    }

    public ConvexHull(Vector3[] vertices) {
        this.vertices = vertices;

        position = Vector3.Zero;
        rotation = Vector3.Zero;
        scale = Vector3.One;
    }

    public Vector3 getVertexPos(int id) {
        return vertices[id];
    }

    public void setVertexPos(int id, Vector3 pos) {
        if (id > vertices.Length - 1 || id < 0) return;

        vertices[id] = pos;
    }

    public void render(Color color) {
        rlPushMatrix();

        rlTranslatef(position.X, position.Y, position.Z);
        rlRotatef(rotation.X, 1, 0, 0);
        rlRotatef(rotation.Y, 0, 1, 0);
        rlRotatef(rotation.Z, 0, 0, 1);
        rlScalef(scale.X, scale.Y, scale.Z);
        
        rlDisableBackfaceCulling();
        rlColor4ub(color.r, color.g, color.b, color.a);
        
        // Draw lines between vertices

        for (int i = 0; i < vertices.Length; i++) {
            for (int j = i + 1; j < vertices.Length; j++) {
                rlBegin(RL_LINES);
                rlVertex3f(vertices[i].X, vertices[i].Y, vertices[i].Z);
                rlVertex3f(vertices[j].X, vertices[j].Y, vertices[j].Z);
                rlEnd();
                
                // render full (non performant)
                // for (int k = j + 1; k < vertices.Length; k++) {
                //     rlBegin(RL_TRIANGLES);
                //     rlVertex3f(vertices[i].X, vertices[i].Y, vertices[i].Z);
                //     rlVertex3f(vertices[j].X, vertices[j].Y, vertices[j].Z);
                //     rlVertex3f(vertices[k].X, vertices[k].Y, vertices[k].Z);
                //     rlEnd();
                // }
            }
        }

        rlEnd();

        foreach (Vector3 vertex in vertices) {
            DrawSphereWires(vertex, 0.1f, 16, 16, RED);
        }

        rlPopMatrix();

        DrawSphereWires(position, 0.1f, 16, 16, YELLOW);
    }

    public void renderXZ(Color color) {
        rlPushMatrix();

        rlTranslatef(position.X * PerspectiveWindow.renderScalar, position.Z * PerspectiveWindow.renderScalar, 0);
        rlRotatef(rotation.Y, 0, 0, 1);
        rlScalef(scale.X * PerspectiveWindow.renderScalar, scale.Z * PerspectiveWindow.renderScalar, 0);

        rlBegin(RL_LINES);
        rlColor4ub(color.r, color.g, color.b, color.a);

        // Draw lines between vertices
        for (int i = 0; i < vertices.Length; i++) {
            for (int j = i + 1; j < vertices.Length; j++) {
                rlVertex2f(vertices[i].X, vertices[i].Z);
                rlVertex2f(vertices[j].X, vertices[j].Z);
            }
        }

        rlEnd();

        rlPopMatrix();
    }

    public void renderXY(Color color) {
        rlPushMatrix();

        rlTranslatef(position.X * PerspectiveWindow.renderScalar, position.Y * PerspectiveWindow.renderScalar, 0);
        rlRotatef(rotation.Z, 0, 0, 1);
        rlScalef(scale.X * PerspectiveWindow.renderScalar, scale.Y * PerspectiveWindow.renderScalar, 0);

        rlBegin(RL_LINES);
        rlColor4ub(color.r, color.g, color.b, color.a);

        // Draw lines between vertices
        for (int i = 0; i < vertices.Length; i++) {
            for (int j = i + 1; j < vertices.Length; j++) {
                rlVertex2f(vertices[i].X, vertices[i].Y);
                rlVertex2f(vertices[j].X, vertices[j].Y);
            }
        }

        rlEnd();

        rlPopMatrix();
    }


    public ConvexHull copy() {
        ConvexHull res = new ConvexHull(vertices);
        res.position = RayMath.Vector3AddValue(position, 0.5f);
        res.rotation = rotation;
        res.scale = scale;

        return res;
    }
}