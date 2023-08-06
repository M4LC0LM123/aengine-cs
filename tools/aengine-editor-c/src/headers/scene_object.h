#ifndef SCENEOBJ_H
#define SCENEOBJ_H
#pragma once
#include "stdio.h"
#include "../../include/raylib.h"
#include "../../include/raymath.h"

class scene_object {
    public:
        Vector3 position;
        Vector3 rotation;
        Vector3 scale;
        int id;
        bool selected;

        scene_object(int id); 
        scene_object(int id, Vector3 position);
        void update();
        void render();
};

#endif