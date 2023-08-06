#ifndef SOMANGER_H
#define SOMANAGER_H
#pragma once
#include "stdio.h"
#include "../../include/raylib.h"
#include "scene_object.h"
#include "iostream"
#include "vector"

class so_manager {
    public:
        std::vector<scene_object*> objects;

        void add_object(int id);
        void add_object(int id, Vector3 position);
        void remove_object(scene_object* object);
        void update(Ray ray, Camera3D camera, RayCollision collision);
        void render();
        void save_to_json(std::string filename);
        void load_from_json(std::string filename);

};


#endif