package com.aengineeditor.game;

import com.badlogic.gdx.Gdx;
import com.badlogic.gdx.Input;
import com.badlogic.gdx.graphics.Color;

public class Config {
    public static float moveSpeed = 25;
    public static int currentId = 0;
    public static int activeId = 0;
    public static Mode currentMode = Mode.ROAM;
    public static SceneObject selectedObject = null;

    public static void changeId() {
        if (Gdx.input.isKeyJustPressed(Input.Keys.NUM_1)) {
            currentId = 1;
        }
        if (Gdx.input.isKeyJustPressed(Input.Keys.NUM_2)) {
            currentId = 2;
        }
        if (Gdx.input.isKeyJustPressed(Input.Keys.NUM_3)) {
            currentId = 3;
        }
        if (Gdx.input.isKeyJustPressed(Input.Keys.NUM_4)) {
            currentId = 4;
        }
        if (Gdx.input.isKeyJustPressed(Input.Keys.NUM_5)) {
            currentId = 5;
        }
        if (Gdx.input.isKeyJustPressed(Input.Keys.NUM_6)) {
            currentId = 6;
        }
        if (Gdx.input.isKeyJustPressed(Input.Keys.NUM_7)) {
            currentId = 7;
        }
        if (Gdx.input.isKeyJustPressed(Input.Keys.NUM_8)) {
            currentId = 8;
        }
        if (Gdx.input.isKeyJustPressed(Input.Keys.NUM_9)) {
            currentId = 9;
        }
    }
}
