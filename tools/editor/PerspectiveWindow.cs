using System.Numerics;
using aengine_cs.aengine.windowing;
using aengine.ecs;
using aengine.graphics;
using Raylib_CsLo;
using Sandbox.aengine.Gui;
using static Raylib_CsLo.Raylib;
using static Raylib_CsLo.RlGl;

namespace Editor;

public class PerspectiveWindow {
    public static GuiWindow window = new GuiWindow("Perspective", 300, 300);
    public static GuiEmbeddedWindow perspective = new GuiEmbeddedWindow(280, 180);
    public static Camera2D camera = new Camera2D();
    public static int renderScalar = 25;

    public static int gridSpacing = 25;
    private static Color gridColor = new Color(255, 255, 255, 125);
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
    
            prevMP = perspective.mousePosition;

            float newZoom = camera.zoom + GetMouseWheelMoveV().Y * 0.01f;
            if (newZoom <= 0)
                newZoom = 0.01f;
            
            float zoomFactor = newZoom / camera.zoom;

            camera.offset = RayMath.Vector2Subtract(camera.offset,
                RayMath.Vector2Scale(RayMath.Vector2Subtract(perspective.mousePosition, camera.target),
                    zoomFactor - 1.0f));
            
            camera.zoom = newZoom;

            if (IsKeyPressed(KeyboardKey.KEY_SPACE)) {
                camera.target = Vector2.Zero;
            }

            if (IsMouseButtonDown(MouseButton.MOUSE_BUTTON_MIDDLE)) {
                Vector2 delta = GetMouseDelta();
                delta = RayMath.Vector2Scale(delta, -1.0f / camera.zoom);

                camera.target = RayMath.Vector2Add(camera.target, delta);
            }
        }
    }

    public static void render(ConvexHullManager manager) {
        if (!window.active) {
            return;
        }

        perspective.beginRender();
        ClearBackground(BLACK);

        BeginMode2D(camera);
        
        Rendering.drawGrid2D(100, gridSpacing, gridColor);
        DrawCircleV(Vector2.Zero, 5, WHITE);
        
        if (TransformGizmo.pMoving) {
            if (IsMouseButtonReleased(MouseButton.MOUSE_BUTTON_LEFT)) {
                TransformGizmo.pMoving = false;
                TransformGizmo.currHull = null;
                TransformGizmo.currVertex = (int)TransformGizmo.vID.EMPTY;
            }

            if (TransformGizmo.currHull != null && TransformGizmo.currVertex != (int)TransformGizmo.vID.EMPTY &&
                TransformGizmo.currVertex != (int)TransformGizmo.vID.POSITION) {
                float snappedX = (float)Math.Round(perspective.mousePosition.X / gridSpacing) * gridSpacing;
                float snappedY = (float)Math.Round(perspective.mousePosition.Y / gridSpacing) * gridSpacing;
                
                Vector3 res = Vector3.Zero with {
                    X = snappedX / renderScalar - TransformGizmo.currHull.position.X,
                    Y = TransformGizmo.currHull.getVertexPos(TransformGizmo.currVertex).Y,
                    Z = snappedY / renderScalar - TransformGizmo.currHull.position.Z
                };
                TransformGizmo.currHull.setVertexPos(TransformGizmo.currVertex, res);
            } else if (TransformGizmo.currHull != null &&
                       TransformGizmo.currVertex == (int)TransformGizmo.vID.POSITION) {
                float snappedX = (float)Math.Round(perspective.mousePosition.X / gridSpacing) * gridSpacing - gridSpacing * 0.5f;
                float snappedY = (float)Math.Round(perspective.mousePosition.Y / gridSpacing) * gridSpacing - gridSpacing * 0.5f;
                
                TransformGizmo.currHull.position = Vector3.Zero with {
                    X = snappedX / renderScalar,
                    Y = TransformGizmo.currHull.position.Y,
                    Z = snappedY / renderScalar
                };
            }
        }

        foreach (ConvexHull ch in manager.convexHulls) {
            Vector2 col = Vector2.Zero with {
                X = ch.position.X * renderScalar,
                Y = ch.position.Z * renderScalar
            };
            
            DrawCircleLines((int)col.X, (int)col.Y, 0.25f * renderScalar, BLUE);
            
            if (CheckCollisionPointCircle(perspective.mousePosition, col, 0.25f * renderScalar)) {
                DrawCircleV(col, 0.25f * renderScalar, GREEN);
                DrawPixelV(perspective.mousePosition, WHITE);
                if (IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT) && !TransformGizmo.pMoving) {
                    TransformGizmo.pMoving = true;
                    TransformGizmo.currHull = ch;
                    TransformGizmo.currVertex = (int)TransformGizmo.vID.POSITION;
                }
            }
            
            for (var i = 0; i < ch.vertices.Length; i++) {
                Vector3 chVertexA = ch.vertices[i];

                Vector2 posA = Vector2.Zero with {
                    X = (chVertexA.X + ch.position.X) * renderScalar,
                    Y = (chVertexA.Z + ch.position.Z) * renderScalar
                };

                Vector2 pos = posA;
                int id = i;
                
                for (var j = i + 1; j < ch.vertices.Length; j++) {
                    Vector3 chVertexB = ch.vertices[j];
                
                    Vector2 posB = Vector2.Zero with {
                        X = (chVertexB.X + ch.position.X) * renderScalar,
                        Y = (chVertexB.Z + ch.position.Z) * renderScalar
                    };
                
                    if (CheckCollisionPointCircle(perspective.mousePosition, posA, 0.25f * renderScalar) &&
                        CheckCollisionPointCircle(perspective.mousePosition, posB, 0.25f * renderScalar)) {
                        if (chVertexB.Y > chVertexA.Y) {
                            pos = posB;
                            id = j;
                        }
                    }
                }

                DrawCircleLines((int)pos.X, (int)pos.Y, 0.25f * renderScalar, RED);
                
                if (CheckCollisionPointCircle(perspective.mousePosition, pos, 0.25f * renderScalar)) {
                    DrawCircleV(pos, 0.25f * renderScalar, GREEN);
                    DrawPixelV(perspective.mousePosition, WHITE);
                    if (IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT) && !TransformGizmo.pMoving) {
                        TransformGizmo.pMoving = true;
                        TransformGizmo.currHull = ch;
                        TransformGizmo.currVertex = id;
                    }
                }
            }
            
            ch.renderXZ(YELLOW);
        }

        EndMode2D();
        perspective.endRender();
    }

    public static void reload() {
        // camera.target = new Vector2(perspective.width * 0.5f, perspective.height * 0.5f);
        // camera.offset = new Vector2(perspective.width * 0.5f, perspective.height * 0.5f);
    }
}