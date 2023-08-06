package com.aengineeditor.game;

import com.badlogic.gdx.math.Vector3;

public class Transform {
    public Vector3 position;
    public Vector3 scale;

    public Transform(Vector3 position, Vector3 scale) {
        this.position = position;
        this.scale = scale;
    }
}
