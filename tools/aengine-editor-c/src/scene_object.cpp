#include "headers/scene_object.h"
#include "../include/raylib.h"
#include "iostream"

#define R_MULTIPLIER 40
#define G_MULTIPLIER 50
#define B_MULTIPLIER 60

scene_object::scene_object(int id) {
    position = {0, 0, 0};
    scale = {2, 2, 2};
    rotation = {0, 0, 0};
    this->id = id;
    selected = false;
    color = Color {(unsigned char)(R_MULTIPLIER * id), (unsigned char)(G_MULTIPLIER * id), (unsigned char)(B_MULTIPLIER * id), 255};
}

scene_object::scene_object(int id, Vector3 position) {
    this->position = position;
    scale = {2, 2, 2};
    rotation = {0, 0, 0};
    this->id = id;
    selected = false;
    color = Color {(unsigned char)(R_MULTIPLIER * id), (unsigned char)(G_MULTIPLIER * id), (unsigned char)(B_MULTIPLIER * id), 255};
}

void scene_object::update() {
    if (selected) {
        if (config::currentMode == MOVE)
        {
            if (IsKeyDown(KEY_W))
            {
                position.x += config::moveSpeed * GetFrameTime();
            }
            if (IsKeyDown(KEY_S))
            {
                position.x -= config::moveSpeed * GetFrameTime();
            }
            if (IsKeyDown(KEY_A))
            {
                position.z -= config::moveSpeed * GetFrameTime();
            }
            if (IsKeyDown(KEY_D))
            {
                position.z += config::moveSpeed * GetFrameTime();
            }
            if (IsKeyDown(KEY_PAGE_UP))
            {
                position.y += config::moveSpeed * GetFrameTime();
            }
            if (IsKeyDown(KEY_PAGE_DOWN))
            {
                position.y -= config::moveSpeed * GetFrameTime();
            }
        }
        if (config::currentMode == SCALE)
        {
            if (IsKeyDown(KEY_W))
            {
                scale.x += config::moveSpeed * GetFrameTime();
            }
            if (IsKeyDown(KEY_S))
            {
                scale.x -= config::moveSpeed * GetFrameTime();
            }
            if (IsKeyDown(KEY_A))
            {
                scale.z -= config::moveSpeed * GetFrameTime();
            }
            if (IsKeyDown(KEY_D))
            {
                scale.z += config::moveSpeed * GetFrameTime();
            }
            if (IsKeyDown(KEY_PAGE_UP))
            {
                scale.y += config::moveSpeed * GetFrameTime();
            }
            if (IsKeyDown(KEY_PAGE_DOWN))
            {
                scale.y -= config::moveSpeed * GetFrameTime();
            }
        }
    }
}

void scene_object::render() {
    DrawCubeV(position, scale, color);

    if (selected) {
        DrawCubeWiresV(position, scale, YELLOW);
    } else {
        DrawCubeWiresV(position, scale, WHITE);
    }
}