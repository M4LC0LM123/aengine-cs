#include "../include/raylib.h"
#include "math.h"
#include "so_manager.cpp"
#include "scene_object.cpp"
#include "iostream"
#include "../include/portable-file-dialogs.h"

//------------------------------------------------------------------------------------
// Program main entry point
//------------------------------------------------------------------------------------
int main(void)
{
    // Initialization
    //--------------------------------------------------------------------------------------
    const int screenWidth = 800;
    const int screenHeight = 450;

    SetConfigFlags(FLAG_WINDOW_RESIZABLE);
    InitWindow(screenWidth, screenHeight, "aengine-editor");
    SetWindowIcon(LoadImage("logo.png"));
    SetExitKey(KEY_NULL);

    // Define the camera to look into our 3d world
    Camera camera = { 0 };
    camera.position = (Vector3){ 10.0f, 10.0f, 10.0f }; // Camera position
    camera.target = (Vector3){ 0.0f, 0.0f, 0.0f };      // Camera looking at point
    camera.up = (Vector3){ 0.0f, 1.0f, 0.0f };          // Camera up vector (rotation towards target)
    camera.fovy = 45.0f;                                // Camera field-of-view Y
    camera.projection = CAMERA_PERSPECTIVE;             // Camera projection type

    Ray ray = { 0 };                    // Picking line ray
    RayCollision collision = { 0 };     // Ray collision hit info

    SetTargetFPS(60);                   // Set our game to run at 60 frames-per-second
    //--------------------------------------------------------------------------------------

    so_manager manager;

    // Main game loop
    while (!WindowShouldClose())        // Detect window close button or ESC key
    {
        // Update
        //----------------------------------------------------------------------------------
        if (IsCursorHidden()) UpdateCamera(&camera);

        manager.update(ray, camera, collision);

        config::changeId();

        if (IsKeyPressed(KEY_SPACE)) {
            manager.add_object(config::currentId);
        }

        if (IsKeyDown(KEY_LEFT_CONTROL) && IsKeyPressed(KEY_SPACE)) {
            manager.add_object(config::currentId, config::activePos);
        }

        if (IsKeyPressed(KEY_F1)) {
            config::currentMode = ROAM;
        }
        if (IsKeyPressed(KEY_F2)) {
            config::currentMode = MOVE;
        }
        if (IsKeyPressed(KEY_F3)) {
            config::currentMode = SCALE;
        }

        // Toggle camera controls
        if (IsMouseButtonPressed(MOUSE_BUTTON_RIGHT)) {
            if (IsCursorHidden()) EnableCursor();
            else DisableCursor();
        }

        if (IsKeyDown(KEY_LEFT_CONTROL) && IsKeyPressed(KEY_S)) {
            auto f = pfd::save_file("Choose file to save",
                            pfd::path::home() + pfd::path::separator(),
                            { "json Files (.json .json)", "*.json *.json" },
                            pfd::opt::force_overwrite);
            manager.save_to_json(f.result());
        }

        if (IsKeyDown(KEY_LEFT_CONTROL) && IsKeyPressed(KEY_L)) {
            auto f = pfd::open_file("Choose files to read", pfd::path::home(),
                            { "json Files (.json .json)", "*.json *.json",
                              "All Files", "*" },
                            pfd::opt::multiselect);
            manager.load_from_json(f.result()[0]);
        }

        //----------------------------------------------------------------------------------

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();

            ClearBackground(BLACK);

            BeginMode3D(camera);
            manager.render();

                DrawRay(ray, MAROON);
                DrawGrid(100, 1.0f);

            EndMode3D();

            DrawText(TextFormat("fps: %d", GetFPS()), 10, 5, 20, WHITE);
            DrawText(TextFormat("mode: %s", enum_to_string[config::currentMode]), 10, 30, 20, WHITE);
            DrawText(TextFormat("id: %d", config::currentId), 10, 55, 20, WHITE);
            if (config::isObjActive) DrawText(TextFormat("object id: %d", config::activeId), 10, 80, 20, WHITE);
            if (config::isObjActive) DrawText(TextFormat("object position: %.2f, %.2f, %.2f", config::activePos.x, config::activePos.y, config::activePos.z), 10, 105, 20, WHITE);
            if (config::isObjActive) DrawText(TextFormat("object scale: %.2f, %.2f, %.2f", config::activeScale.x, config::activeScale.y, config::activeScale.z), 10, 130, 20, WHITE);

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    // De-Initialization
    //--------------------------------------------------------------------------------------
    CloseWindow();        // Close window and OpenGL context
    //--------------------------------------------------------------------------------------

    return 0;
}