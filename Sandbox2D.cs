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
        SetTargetFPS(60);

        List<RigidBody2D> bodies = new List<RigidBody2D>();

        for (int i = 0; i < 10; i++) {
            // PhysicsShape type = (PhysicsShape) aengine.core.aengine.getRandomInt(0, 2);
            PhysicsShape type = PhysicsShape.CIRCLE;
            
            if (type == PhysicsShape.CIRCLE) {
                bool success = RigidBody2D.createCircleBody(aengine.core.aengine.getRandomFloat(10, 50),
                    new Vector2(aengine.core.aengine.getRandomFloat(0, GetScreenWidth()),
                        aengine.core.aengine.getRandomFloat(0, GetScreenHeight())), 1, false, 0.5f,
                    out RigidBody2D body, out string errorMsg);

                if (success) {
                    bodies.Add(body);
                } else {
                    Console.WriteLine(errorMsg);  
                }
            } else if (type == PhysicsShape.BOX) {
                bool success = RigidBody2D.createBoxBody(aengine.core.aengine.getRandomFloat(10, 50), aengine.core.aengine.getRandomFloat(10, 50),
                    new Vector2(aengine.core.aengine.getRandomFloat(0, GetScreenWidth()),
                        aengine.core.aengine.getRandomFloat(0, GetScreenHeight())), 1, false, 0.5f,
                    out RigidBody2D body, out string errorMsg);

                if (success) {
                    bodies.Add(body);
                } else {
                    Console.WriteLine(errorMsg);  
                }
            }
        }
        
        // Main game loop
        while (!WindowShouldClose()) {

            float dx = 0;
            float dy = 0;

            if (IsKeyDown(KeyboardKey.KEY_LEFT))
                dx -= 10 * GetFrameTime();
            if (IsKeyDown(KeyboardKey.KEY_RIGHT))
                dx += 10 * GetFrameTime();
            if (IsKeyDown(KeyboardKey.KEY_UP))
                dy -= 10 * GetFrameTime();
            if (IsKeyDown(KeyboardKey.KEY_DOWN))
                dy += 10 * GetFrameTime();

            if (dx != 0 || dy != 0) {
                Vector2 dir = Vector2.Normalize(new Vector2(dx, dy));
                Vector2 vel = dir * 5;
                bodies[0].move(vel);
            }

            for (int i = 0; i < bodies.Count - 1; i++) {
                RigidBody2D bodyA = bodies[i];
                
                for (int j = i + 1; j < bodies.Count; j++) {
                    RigidBody2D bodyB = bodies[j];

                    if (Collisions.CheckCircleOverlap(bodyA.getPosition(), bodyA.radius, bodyB.getPosition(),
                            bodyB.radius, out Vector2 normal, out float depth)) {
                        bodyA.move(-normal * depth / 2);
                        bodyB.move(normal * depth / 2);
                    }
                }
            }
            
            BeginDrawing();
            ClearBackground(RAYWHITE);

            foreach (var body in bodies) {
                if (body.shape is PhysicsShape.BOX)
                    DrawRectanglePro(new Rectangle(body.getPosition().X, body.getPosition().Y, body.width, body.height), Vector2.Zero, body.getRotation(), GREEN);
                if (body.shape is PhysicsShape.CIRCLE)
                    DrawCircleV(body.getPosition(), body.radius, GREEN);
            }
            
            DrawFPS(10, 10);
            EndDrawing();
        }

        CloseWindow();
    }
}