using Raylib_CsLo;
using Sandbox.aengine.Gui;
using static Raylib_CsLo.Raylib;

namespace Editor;

public class ConvexHullManager {
    public List<ConvexHull> convexHulls = new List<ConvexHull>();

    public void update(TransformGizmo gizmo) {
        if (IsKeyDown(KeyboardKey.KEY_ENTER)) {
            convexHulls.Add(new ConvexHull());
        }

        foreach (ConvexHull ch in convexHulls) {
            for (var i = 0; i < ch.vertexGrabbers.Count; i++) {
                RayCollision collision = GetRayCollisionSphere(TransformGizmo.MOUSE_RAY, ch.getVertexPos(i), 0.25f);
                if (IsMouseButtonDown(MouseButton.MOUSE_BUTTON_LEFT) && !Gui.isMouseOver()) {
                    if (collision.hit) {
                        if (!float.IsNaN(collision.point.X) && !float.IsNaN(collision.point.Y) &&
                            !float.IsNaN(collision.point.Z)) {
                            ch.setVertexPos(i, collision.point);
                        }
                    }
                }
            }
        }
    }

    public void render() {
        foreach (ConvexHull convexHull in convexHulls) {
            convexHull.render(GREEN);
        }
    }
}