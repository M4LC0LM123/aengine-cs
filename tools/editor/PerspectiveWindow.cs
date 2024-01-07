using System.Numerics;
using aengine_cs.aengine.windowing;
using aengine.ecs;
using Raylib_CsLo;
using Sandbox.aengine.Gui;
using static Raylib_CsLo.Raylib;

namespace Editor; 

public class PerspectiveWindow {
    public static GuiWindow window = new GuiWindow("Perspective", 300, 300);
    public static GuiEmbeddedWindow perspective = new GuiEmbeddedWindow(280, 180);
    public static Camera2D camera = new Camera2D();

    private static int gridSpacing = 20;
    private static int renderScalar = 25; 
    private static Vector2 prevMP = GetMousePosition();

    public static void init() {
        window.active = false;

        camera.target = new Vector2(perspective.width * 0.5f, perspective.height * 0.5f);
        camera.offset = new Vector2(perspective.width * 0.5f, perspective.height * 0.5f);
        camera.rotation = 0.0f;
        camera.zoom = 1.0f;
    }

    public static void update() {
        if (window.active) {
            perspective.mousePosition = GetScreenToWorld2D(perspective.mousePosition, camera);

            Vector2 delta = Vector2.Subtract(prevMP, perspective.mousePosition);
            prevMP = perspective.mousePosition;
            
            float newZoom = camera.zoom + GetMouseWheelMoveV().Y * 0.01f;
            if (newZoom <= 0)
                newZoom = 0.01f;

            camera.zoom = newZoom;

            if (IsKeyPressed(KeyboardKey.KEY_SPACE)) {
                camera.target = Vector2.Zero;
            }

            if (IsMouseButtonDown(MouseButton.MOUSE_BUTTON_MIDDLE)) {
                if (delta != Vector2.Zero) {
                    camera.target = GetScreenToWorld2D(Vector2.Add(camera.offset, delta), camera);
                } 
            }
        }
    }
    
    public static void render() {
        if (window.active) {
            perspective.beginRender();
            ClearBackground(BLACK);
            
            // for (int x = -perspective.width; x < perspective.width * 2; x += gridSpacing) {
            //     DrawLine(x - ((int)camera.target.X % gridSpacing), -perspective.height, x - ((int)camera.target.X % gridSpacing),
            //         perspective.height * 2, LIGHTGRAY);
            // }
            //
            // for (int y = -perspective.height; y < perspective.height * 2; y += gridSpacing) {
            //     DrawLine(-perspective.width, y - ((int)camera.target.Y % gridSpacing), perspective.width * 2,
            //         y - ((int)camera.target.Y % gridSpacing), LIGHTGRAY);
            // }
            
            BeginMode2D(camera);
            
            foreach (Entity ent in World.entities.Values) {
                DrawRectangleLines(
                    (int)(ent.transform.position.X - ent.transform.scale.X * 0.5f) * renderScalar,
                    (int)(ent.transform.position.Z - ent.transform.scale.Z * 0.5f) * renderScalar,
                    (int)ent.transform.scale.X * renderScalar,
                    (int)ent.transform.scale.Z * renderScalar, WHITE);
            }
                    
            EndMode2D();
            perspective.endRender();
        }
    }

    public static void reload() {
        // camera.target = new Vector2(perspective.width * 0.5f, perspective.height * 0.5f);
        // camera.offset = new Vector2(perspective.width * 0.5f, perspective.height * 0.5f);
    }
}