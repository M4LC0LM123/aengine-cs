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
        
        for (int i = 0; i < 10; i++) {
            PhysicsShape type = (PhysicsShape) aengine.core.aengine.getRandomInt(0, 2);
            
            if (type == PhysicsShape.CIRCLE) {
                bool success = RigidBody2D.createCircleBody(aengine.core.aengine.getRandomFloat(10, 50),
                    new Vector2(aengine.core.aengine.getRandomFloat(0, GetScreenWidth()),
                        aengine.core.aengine.getRandomFloat(0, GetScreenHeight())), 1, false, 0.5f,
                    out RigidBody2D body, out string errorMsg);

                if (success) {
                    world.addBody(body);
                } else {
                    Console.WriteLine(errorMsg);  
                }
            } else if (type == PhysicsShape.BOX) {
                bool success = RigidBody2D.createBoxBody(40, 40,
                    new Vector2(aengine.core.aengine.getRandomFloat(0, GetScreenWidth()),
                        aengine.core.aengine.getRandomFloat(0, GetScreenHeight())), 1, false, 0.5f,
                    out RigidBody2D body, out string errorMsg);

                if (success) {
                    world.addBody(body);
                } else {
                    Console.WriteLine(errorMsg);  
                }
            }
        }
        
        // Main game loop
        while (!WindowShouldClose()) {
            world.tick(GetFrameTime());
            
            float dx = 0;
            float dy = 0;

            if (!world.getBody(0, out RigidBody2D player)) {
                throw new Exception("Body not found!");
            }
                
            if (IsKeyDown(KeyboardKey.KEY_LEFT) && !IsKeyDown(KeyboardKey.KEY_LEFT_CONTROL))
                dx -= 10 * GetFrameTime();
            if (IsKeyDown(KeyboardKey.KEY_RIGHT) && !IsKeyDown(KeyboardKey.KEY_LEFT_CONTROL))
                dx += 10 * GetFrameTime();
            if (IsKeyDown(KeyboardKey.KEY_UP))
                dy -= 10 * GetFrameTime();
            if (IsKeyDown(KeyboardKey.KEY_DOWN))
                dy += 10 * GetFrameTime();

            if (IsKeyDown(KeyboardKey.KEY_LEFT_CONTROL)) {
                if (IsKeyDown(KeyboardKey.KEY_RIGHT)) 
                    player.rotate(4);
                if (IsKeyDown(KeyboardKey.KEY_LEFT)) 
                    player.rotate(-4);
            }

            if (dx != 0 || dy != 0) {
                Vector2 forceDir = Vector2.Normalize(new Vector2(dx, dy));
                Vector2 force = forceDir * 100;
                player.applyForce(force);
            }

            if (IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT)) {
                bool success = RigidBody2D.createBoxBody(40, 40,
                    GetMousePosition(), 1, false, 0.5f,
                    out RigidBody2D body, out string errorMsg);
                
                if (success) {
                    world.addBody(body);
                } else {
                    Console.WriteLine(errorMsg);  
                }
            }
            
            if (IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_RIGHT)) {
                bool success = RigidBody2D.createCircleBody(aengine.core.aengine.getRandomFloat(10, 50),
                    GetMousePosition(), 1, false, 0.5f,
                    out RigidBody2D body, out string errorMsg);

                if (success) {
                    world.addBody(body);
                } else {
                    Console.WriteLine(errorMsg);  
                }
            }
            
            BeginDrawing();
            ClearBackground(RAYWHITE);

            for (int i = 0; i < world.bodyCount(); i++) {
                if (!world.getBody(i, out RigidBody2D body)) {
                    throw new Exception("Body not found!");
                }
                
                if (body.shape is PhysicsShape.BOX) {
                    DrawRectanglePro(new Rectangle(body.getPosition().X, body.getPosition().Y, body.width, body.height),
                        Vector2.Zero with { X = body.width / 2, Y = body.height / 2 }, body.getRotation(), GREEN);
                }

                if (body.shape is PhysicsShape.CIRCLE)
                    DrawCircleV(body.getPosition(), body.radius, GREEN);
            }
            
            DrawFPS(10, 10);
            EndDrawing();
        }

        CloseWindow();
    }
}