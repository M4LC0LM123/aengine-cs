package com.aengineeditor.game;

import com.badlogic.gdx.Gdx;
import com.badlogic.gdx.Input;
import com.badlogic.gdx.graphics.Color;
import com.badlogic.gdx.graphics.g3d.Environment;
import com.badlogic.gdx.graphics.g3d.ModelBatch;
import com.badlogic.gdx.math.collision.Ray;
import com.badlogic.gdx.utils.Array;
import com.badlogic.gdx.utils.Json;
import com.badlogic.gdx.utils.JsonWriter;

import java.io.File;
import java.io.FileWriter;
import java.io.IOException;
import java.io.Writer;
import java.nio.file.Files;
import java.nio.file.Paths;
import java.util.ArrayList;
import java.util.List;

public class World {
    public static ArrayList<SceneObject> objects = new ArrayList<SceneObject>();

    public static void addObject(int id, Color color) {
        objects.add(new SceneObject(id, color));
    }

    public static void removeObject(SceneObject object) {
        objects.remove(object);
    }

    public static void update(Ray mouseRay) {
        for (SceneObject object : objects) {
            Transform transform = new Transform(object.position, object.scale);
            if (Collision.isRayColliding(mouseRay.origin, mouseRay.direction, transform) && Gdx.input.isButtonJustPressed(Input.Buttons.LEFT)) {
                object.selected = true;
                Config.activeId = object.id;
                Config.selectedObject = object;
            }
            if (!Collision.isRayColliding(mouseRay.origin, mouseRay.direction, transform) && Gdx.input.isButtonJustPressed(Input.Buttons.LEFT)) {
                object.selected = false;
                Config.selectedObject = null;
            }
            object.update();
        }
    }

    public static void render(ModelBatch batch, Environment environment) {
        for (SceneObject object : objects) {
            object.render(batch, environment);
        }
    }

    public static void dispose() {
        for (SceneObject object : objects) {
            object.dispose();
        }
    }

    private static final Json json = new Json();

    static {
        json.setOutputType(JsonWriter.OutputType.json);
        json.setUsePrototypes(false);
        json.addClassTag("SceneObjectData", SceneObjectData.class);
    }

    public static void saveObjectsToJson(String fileName) {
        Array<SceneObjectData> objectDataList = new Array<>();
        for (SceneObject object : objects) {
            SceneObjectData data = new SceneObjectData();
            data.x = object.position.x;
            data.y = object.position.y;
            data.z = object.position.z;
            data.w = object.scale.x;
            data.h = object.scale.y;
            data.d = object.scale.z;
            data.rx = object.rotation.x;
            data.ry = object.rotation.y;
            data.rz = object.rotation.z;
            data.id = object.id;
            objectDataList.add(data);
        }
        String jsonData = json.prettyPrint(objectDataList);
        try {
            Files.writeString(Paths.get(fileName), jsonData);
        } catch (IOException e) {
            e.printStackTrace();
        }
    }

    public static void loadObjectsFromJson(String fileName) {
        try {
            String jsonData = Files.readString(Paths.get(fileName));
            Array<SceneObjectData> objectDataList = json.fromJson(Array.class, jsonData);
            objects.clear();
            for (SceneObjectData data : objectDataList) {
                SceneObject object = new SceneObject(data.id, Color.GREEN);
                object.position.set(data.x, data.y, data.z);
                object.scale.set(data.w, data.h, data.d);
                object.rotation.set(data.rx, data.ry, data.rz);
                objects.add(object);
            }
        } catch (IOException e) {
            e.printStackTrace();
        }
    }

}
