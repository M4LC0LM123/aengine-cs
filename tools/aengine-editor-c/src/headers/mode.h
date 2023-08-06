#ifndef MODE_H
#define MODE_H
#pragma once

enum mode {
    ROAM,
    MOVE,
    SCALE
};

static const char *enum_to_string[] ={ 
    "ROAM", 
    "MOVE", 
    "SCALE"
};

#endif