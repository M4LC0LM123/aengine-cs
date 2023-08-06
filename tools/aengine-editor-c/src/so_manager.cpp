#include "headers/so_manager.h"
#include "../include/raylib.h"
#include "config.cpp"
#include <iostream>
#include <fstream>
#include <vector>
#include "headers/json.hpp"

using json = nlohmann::json;

json scene_obj_to_json(scene_object* obj) {
    json j;
    j["x"] = obj->position.x;
    j["y"] = obj->position.y;
    j["z"] = obj->position.z;
    j["w"] = obj->scale.x;
    j["h"] = obj->scale.y;
    j["d"] = obj->scale.z;
    j["rx"] = obj->rotation.x;
    j["ry"] = obj->rotation.y;
    j["rz"] = obj->rotation.z;
    j["id"] = obj->id;
    return j;
}


scene_object* json_to_scene_obj(const json& j) {
    scene_object* obj = new scene_object(j["id"]);
    obj->position = {j["x"], j["y"], j["z"]};
    obj->scale = {j["w"], j["h"], j["d"]};
    obj->rotation = {j["rx"], j["ry"], j["rz"]};
    return obj;
}

std::vector<scene_object*> load_scene_from_json(std::string filename) {
    std::vector<scene_object*> objects;
    
    std::ifstream file(filename);
    if (!file.is_open()) {
        std::cerr << "Error opening file: " << filename << std::endl;
        return objects;
    }

    json j;
    file >> j;
    file.close();

    if (j.is_array()) {
        for (const auto& entry : j) {
            objects.push_back(json_to_scene_obj(entry));
        }
    } else {
        std::cerr << "JSON file does not contain an array!" << std::endl;
    }

    return objects;
}

void so_manager::add_object(int id) {
    objects.push_back(new scene_object(id));
}

void so_manager::add_object(int id, Vector3 position) {
    objects.push_back(new scene_object(id, position));
}

void so_manager::remove_object(scene_object* obj) {
    for (auto it = objects.begin(); it != objects.end(); ++it)
    {
        if (*it == obj)
        {
            objects.erase(it);
            break;
        }
    }
}

void so_manager::update(Ray ray, Camera3D camera, RayCollision collision) {
    for (scene_object* obj : objects) {
        if (IsMouseButtonPressed(MOUSE_BUTTON_LEFT)) {
            ray = GetMouseRay(GetMousePosition(), camera);
                // Check collision between ray and box
                collision = GetRayCollisionBox(ray,
                            (BoundingBox){(Vector3){ obj->position.x - obj->scale.x/2, obj->position.y - obj->scale.y/2, obj->position.z - obj->scale.z/2 },
                                        (Vector3){ obj->position.x + obj->scale.x/2, obj->position.y + obj->scale.y/2, obj->position.z + obj->scale.z/2 }});

            if (collision.hit) {
                obj->selected = true;
                config::activeId = obj->id;
                config::activePos = obj->position;
                config::activeScale = obj->scale;
            }

            if (!collision.hit) {
                obj->selected = false;
                config::isObjActive = false;
            }
        }

        if (obj->selected) {
            config::isObjActive = true; 
            config::activeId = obj->id;
            config::activePos = obj->position;
            config::activeScale = obj->scale;
            if (IsKeyPressed(KEY_BACKSPACE)) {
                obj->selected = false;
                config::isObjActive = false;
                remove_object(obj);
            }
        }
        obj->update();
    }
}

void so_manager::render() {
    for (scene_object* obj : objects) {
        obj->render();
    }
}

void so_manager::save_to_json(std::string filename) {
    json j;
    for (scene_object* obj : objects) {
        j.push_back(scene_obj_to_json(obj));
    }

    std::ofstream file(filename);
    if (!file.is_open()) {
        std::cerr << "Error opening file: " << filename << std::endl;
        return;
    }

    file << j.dump(4);
    file.close();
}

void so_manager::load_from_json(std::string filename) {
    objects.clear();

    std::ifstream file(filename);
    if (!file.is_open()) {
        std::cerr << "Error opening file: " << filename << std::endl;
    }

    json j;
    file >> j;
    file.close();

    if (j.is_array()) {
        for (const auto& entry : j) {
            objects.push_back(json_to_scene_obj(entry));
        }
    } else {
        std::cerr << "JSON file does not contain an array!" << std::endl;
    }
}