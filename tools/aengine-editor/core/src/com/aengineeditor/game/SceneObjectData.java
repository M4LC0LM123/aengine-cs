package com.aengineeditor.game;

import com.badlogic.gdx.utils.Json;
import com.badlogic.gdx.utils.JsonValue;

public class SceneObjectData implements Json.Serializable {
    public float x;
    public float y;
    public float z;
    public float w;
    public float h;
    public float d;
    public float rx;
    public float ry;
    public float rz;
    public int id;

    @Override
    public void write(Json json) {
        json.writeValue("x", x);
        json.writeValue("y", y);
        json.writeValue("z", z);
        json.writeValue("w", w);
        json.writeValue("h", h);
        json.writeValue("d", d);
        json.writeValue("rx", rx);
        json.writeValue("ry", ry);
        json.writeValue("rz", rz);
        json.writeValue("id", id);
    }

    @Override
    public void read(Json json, JsonValue jsonData) {
        x = jsonData.getFloat("x");
        y = jsonData.getFloat("y");
        z = jsonData.getFloat("z");
        w = jsonData.getFloat("w");
        h = jsonData.getFloat("h");
        d = jsonData.getFloat("d");
        rx = jsonData.getFloat("rx");
        ry = jsonData.getFloat("ry");
        rz = jsonData.getFloat("rz");
        id = jsonData.getInt("id");
    }
}
