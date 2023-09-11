using System.Numerics;
using aengine.graphics;
using aengine.physics2D;
using Raylib_CsLo;
using static Raylib_CsLo.Raylib;
using PhysicsShape = aengine.physics2D.PhysicsShape;

namespace Sandbox2D;

public class Sandbox2D {
    public static void main() {
        Directory.SetCurrentDirectory("../../../");
        
        SetConfigFlags(ConfigFlags.FLAG_WINDOW_RESIZABLE);
        InitWindow(800, 600, "Sandbox2D");
        SetWindowIcon(LoadImage("assets/logo.png"));
        SetTargetFPS(60);

        PhysicsWorld world = new PhysicsWorld();

        if (RigidBody2D.createBoxBody(GetScreenWidth() / 8 * 6, 50,
                new Vector2(GetScreenWidth() / 2, GetScreenHeight() - 25),
                1.0f, true, 0.5f, out RigidBody2D floor, out string error)) {
            world.addBody(floor);
        } else {
            throw new Exception(error);
        }
        
        // Main game loop
        while (!WindowShouldClose()) {
            world.tick(GetFrameTime());

            if (IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT)) {
                bool success = RigidBody2D.createBoxBody(aengine.core.aengine.getRandomFloat(10, 50), aengine.core.aengine.getRandomFloat(10, 50),
                    GetMousePosition(), 1, false, 0.5f,
                    out RigidBody2D body, out string errorMsg);
                
                if (success) {
                    world.addBody(body);
                } else {
                    throw new Exception(errorMsg); 
                }
            }
            
            if (IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_RIGHT)) {
                bool success = RigidBody2D.createCircleBody(aengine.core.aengine.getRandomFloat(10, 50),
                    GetMousePosition(), 1, false, 0.5f,
                    out RigidBody2D body, out string errorMsg);

                if (success) {
                    world.addBody(body);
                } else {
                    throw new Exception(errorMsg);
                }
            }
            
            BeginDrawing();
            ClearBackground(RAYWHITE);

            for (int i = 0; i < world.bodyCount(); i++) {
                if (!world.getBody(i, out RigidBody2D body)) {
                    throw new Exception("Body not found!");
                }
                
                if (body.shape is PhysicsShape.BOX) {

                    if (body.isStatic) {
                        DrawRectanglePro(new Rectangle(body.getPosition().X, body.getPosition().Y, body.width, body.height),
                            Vector2.Zero with { X = body.width / 2, Y = body.height / 2 }, body.getRotation(), BLUE);
                    }
                    else {
                        DrawRectanglePro(new Rectangle(body.getPosition().X, body.getPosition().Y, body.width, body.height),
                            Vector2.Zero with { X = body.width / 2, Y = body.height / 2 }, body.getRotation(), GREEN);   
                    }
                }

                if (body.shape is PhysicsShape.CIRCLE) {
                    if (body.isStatic) {
                        DrawCircleV(body.getPosition(), body.radius, BLUE);
                    }
                    else {
                        DrawCircleV(body.getPosition(), body.radius, GREEN);   
                    }
                }
            }
            
            DrawFPS(10, 10);
            EndDrawing();
        }

        CloseWindow();
    }
}