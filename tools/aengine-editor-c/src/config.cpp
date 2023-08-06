#include "headers/config.h"

float config::moveSpeed = 25;
int config::currentId = 0;
int config::activeId = 0;
mode config::currentMode = ROAM;
Vector3 config::activePos;
Vector3 config::activeScale;
bool config::isObjActive = false;

void config::changeId() {
    if (IsKeyPressed(KEY_ZERO))
        currentId = 0;
    if (IsKeyPressed(KEY_ONE))
        currentId = 1;
    if (IsKeyPressed(KEY_TWO))
        currentId = 2;
    if (IsKeyPressed(KEY_THREE))
        currentId = 3;
    if (IsKeyPressed(KEY_FOUR))
        currentId = 4;
    if (IsKeyPressed(KEY_FIVE))
        currentId = 5;
    if (IsKeyPressed(KEY_SIX))
        currentId = 6;
    if (IsKeyPressed(KEY_SEVEN))
        currentId = 7;
    if (IsKeyPressed(KEY_EIGHT))
        currentId = 8;
    if (IsKeyPressed(KEY_NINE))
        currentId = 9;
}