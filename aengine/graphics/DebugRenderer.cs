using System.Numerics;
using Jitter.LinearMath;
using Raylib_CsLo;

namespace aengine.graphics; 

public class DebugRenderer : Jitter.IDebugDrawer {
    public void DrawLine(JVector start, JVector end) {
        Raylib.DrawLine3D(Vector3.Zero with { X = start.X, Y = start.Y, Z = start.Z },
            Vector3.Zero with { X = end.X, Y = end.Y, Z = end.Z }, Raylib.GREEN);
    }

    public void DrawPoint(JVector pos) {
        Raylib.DrawCubeV(Vector3.Zero with { X = pos.X, Y = pos.Y, Z = pos.Z }, Vector3.One * 0.5f, Raylib.GREEN);
    }

    public void DrawTriangle(JVector pos1, JVector pos2, JVector pos3) {
        Raylib.DrawTriangle3D(jitterToNumerics(pos1), jitterToNumerics(pos2), jitterToNumerics(pos3), Raylib.GREEN);
    }

    public Vector3 jitterToNumerics(JVector vector) {
        return Vector3.Zero with { X = vector.X, Y = vector.Y, Z = vector.Z};
    }
    
}