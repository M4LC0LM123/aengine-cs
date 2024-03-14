using Raylib_CsLo;
using Sandbox.aengine.Gui;
using static Raylib_CsLo.Raylib;

namespace Editor;

public class ConvexHullManager {
    public List<ConvexHull> convexHulls = new List<ConvexHull>();

    public void update(TransformGizmo gizmo) {
        if (IsKeyPressed(KeyboardKey.KEY_ENTER)) {
            convexHulls.Add(new ConvexHull());
        }
    }

    public void render() {
        foreach (ConvexHull convexHull in convexHulls) {
            convexHull.render(GREEN);
        }
    }
}