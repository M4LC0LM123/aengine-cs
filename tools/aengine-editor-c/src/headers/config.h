#ifndef CONFIG_H
#define CONFIG_H
#pragma once
#include "../../include/raylib.h"
#include "scene_object.h"
#include "mode.h"

class config
{
    public:
        static float moveSpeed;
        static int currentId;
        static int activeId;
        static mode currentMode;
        static Vector3 activePos;
        static Vector3 activeScale;
        static bool isObjActive;

        static void changeId();
};


#endif