using System.Diagnostics;
using System.Numerics;
using aengine.core;
using aengine.graphics;
using aengine.physics2D;
using Raylib_CsLo;
using Sandbox.aengine.Gui;
using static Raylib_CsLo.Raylib;
using PhysicsShape = aengine.physics2D.PhysicsShape;

using A_Console = aengine.core.Console;
using Console = System.Console;

namespace Sandbox2D;

public class Sandbox2D {
    public static void main() {
        Directory.SetCurrentDirectory("../../../");
        
        SetConfigFlags(ConfigFlags.FLAG_WINDOW_RESIZABLE);
        InitWindow(800, 600, "Sandbox2D");
        SetWindowIcon(LoadImage("assets/logo.png"));
        SetTargetFPS(60);
        
        Gui.font = LoadFont("assets/fonts/font.ttf");
        A_Console console = new A_Console();
        
        PhysicsWorld world = new PhysicsWorld();

        if (PhysicsBody.createBoxBody(GetScreenWidth() / 8 * 6, 50,
                new Vector2(GetScreenWidth() / 2, GetScreenHeight() - 25),
                1.0f, true, 0.5f, out PhysicsBody floor, out string error)) {
            world.addBody(floor);
        } else {
            throw new Exception(error);
        }
        
        if (PhysicsBody.createBoxBody(250, 20,
                new Vector2(200, 400),
                1.0f, true, 0.5f, out PhysicsBody ledge, out error)) {

            ledge.setRotation(45 * RayMath.DEG2RAD);
            world.addBody(ledge);
        } else {
            throw new Exception(error);
        }

        Stopwatch stopwatch = new Stopwatch();

        Sprite sprite = new Sprite(LoadTexture("assets/animation.png"));
        sprite.position = new Vector2(10, 400);
        sprite.scale = Vector2.One * 40;
        sprite.setFrame(0);

        Animation animation = new Animation() {
            texture = sprite.texture,
            frame = 0,
            frameCounter = 0,
            frameCount = 5,
            frameSize = sprite.frameScale,
            speed = 5
        };
        
        // Main game loop
        while (!WindowShouldClose()) {
            stopwatch.Restart();
            world.tick(GetFrameTime());
            stopwatch.Stop();
            
            for (int i = 0; i < world.bodyCount(); i++) {
                if (!world.getBody(i, out PhysicsBody body)) {
                    throw new ArgumentOutOfRangeException();
                }

                PhysicsAABB box = body.getAABB();

                if (box.max.Y > GetScreenHeight()) {
                    world.removeBody(body);
                }
            }

            if (!console.active) {
                if (IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT)) {
                    bool success = PhysicsBody.createBoxBody(aengine.core.aengine.getRandomFloat(10, 50), aengine.core.aengine.getRandomFloat(10, 50),
                        GetMousePosition(), 1, false, 0.5f,
                        out PhysicsBody body, out string errorMsg);
                
                    if (success) {
                        world.addBody(body);
                    } else {
                        throw new Exception(errorMsg); 
                    }
                }
            
                if (IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_RIGHT)) {
                    bool success = PhysicsBody.createCircleBody(aengine.core.aengine.getRandomFloat(10, 50),
                        GetMousePosition(), 1, false, 0.5f,
                        out PhysicsBody body, out string errorMsg);

                    if (success) {
                        world.addBody(body);
                    } else {
                        throw new Exception(errorMsg);
                    }
                }
            }

            if (IsKeyPressed(KeyboardKey.KEY_GRAVE)) {
                console.window.setPosition(10, 10);
                console.active = !console.active;
            }
            
            BeginDrawing();
            ClearBackground(BLACK);

            sprite.render();

            if (IsKeyDown(KeyboardKey.KEY_TAB)) {
                sprite.animate(animation);
            }
            
            for (int i = 0; i < world.bodyCount(); i++) {
                if (!world.getBody(i, out PhysicsBody body)) {
                    throw new Exception("Body not found!");
                }
                
                if (body.shape is PhysicsShape.BOX) {

                    if (body.isStatic) {
                        DrawRectanglePro(new Rectangle(body.getPosition().X, body.getPosition().Y, body.width, body.height),
                            Vector2.Zero with { X = body.width / 2, Y = body.height / 2 }, body.getRotation() * RayMath.RAD2DEG, BLUE);
                    }
                    else {
                        DrawRectanglePro(new Rectangle(body.getPosition().X, body.getPosition().Y, body.width, body.height),
                            Vector2.Zero with { X = body.width / 2, Y = body.height / 2 }, body.getRotation() * RayMath.RAD2DEG, GREEN);   
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
            
            foreach (Vector2 transformedVertex in ledge.getTransformedVertices()) {
                DrawCircleV(transformedVertex, 5, WHITE);
            }
            
            DrawRectangleV(floor.getAABB().min, floor.getAABB().max - floor.getAABB().min, WHITE with { a = 128});
            
            foreach (Vector2 contactPoint in world.contactPointList) {
                DrawCircleLines((int)contactPoint.X, (int)contactPoint.Y, 5, WHITE);
            }
            
            DrawFPS(10, 10);
            DrawText("bodies: " + world.bodyCount(), 10, 40, 24, GREEN);
            DrawText("step time: " + stopwatch.Elapsed.TotalMilliseconds, 10, 70, 24, GREEN);
            DrawText("contact points: " + world.contactPointList.Count, 10, 100, 24, GREEN);
            console.render();
            EndDrawing();
        }

        sprite.dispose();
        CloseWindow();
    }
}