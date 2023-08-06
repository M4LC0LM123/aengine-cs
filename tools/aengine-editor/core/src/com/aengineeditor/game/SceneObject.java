package com.aengineeditor.game;

import com.badlogic.gdx.Gdx;
import com.badlogic.gdx.Input;
import com.badlogic.gdx.graphics.Color;
import com.badlogic.gdx.graphics.Texture;
import com.badlogic.gdx.graphics.VertexAttributes;
import com.badlogic.gdx.graphics.g3d.*;
import com.badlogic.gdx.graphics.g3d.attributes.ColorAttribute;
import com.badlogic.gdx.graphics.g3d.attributes.TextureAttribute;
import com.badlogic.gdx.graphics.g3d.utils.ModelBuilder;
import com.badlogic.gdx.math.Quaternion;
import com.badlogic.gdx.math.Vector3;
import com.badlogic.gdx.math.collision.BoundingBox;
import com.badlogic.gdx.math.collision.Ray;

public class SceneObject {
    public Vector3 position;
    public Vector3 scale;
    public Vector3 rotation;
    public int id;

    public boolean selected;

    Model model;
    ModelInstance instance;
    BoundingBox boundingBox;

    Model outline;
    ModelInstance outlineInstance;

    public SceneObject(int id, Color color) {
        position = new Vector3();
        scale = new Vector3(2, 2, 2);
        rotation = new Vector3();
        this.id = id;
        selected = false;

        ModelBuilder modelBuilder = new ModelBuilder();
        model = modelBuilder.createBox(scale.x, scale.y, scale.z, new Material(ColorAttribute.createDiffuse(color)), VertexAttributes.Usage.Position | VertexAttributes.Usage.Normal);
        instance = new ModelInstance(model);

        outline = modelBuilder.createBox(scale.x, scale.y, scale.z, new Material(TextureAttribute.createDiffuse(new Texture(Gdx.files.internal("logo.png")))), VertexAttributes.Usage.Position | VertexAttributes.Usage.Normal | VertexAttributes.Usage.TextureCoordinates);
        outlineInstance = new ModelInstance(outline);

        boundingBox = new BoundingBox();
    }

    public void update() {
        if (selected) {
            if (Config.currentMode == Mode.MOVE) {
                if (Gdx.input.isKeyPressed(Input.Keys.UP)) {
                    position.x += Config.moveSpeed * Gdx.graphics.getDeltaTime();
                }
                if (Gdx.input.isKeyPressed(Input.Keys.DOWN)) {
                    position.x -= Config.moveSpeed * Gdx.graphics.getDeltaTime();
                }
                if (Gdx.input.isKeyPressed(Input.Keys.LEFT)) {
                    position.z -= Config.moveSpeed * Gdx.graphics.getDeltaTime();
                }
                if (Gdx.input.isKeyPressed(Input.Keys.RIGHT)) {
                    position.z += Config.moveSpeed * Gdx.graphics.getDeltaTime();
                }
                if (Gdx.input.isKeyPressed(Input.Keys.PAGE_UP)) {
                    position.y += Config.moveSpeed * Gdx.graphics.getDeltaTime();
                }
                if (Gdx.input.isKeyPressed(Input.Keys.PAGE_DOWN)) {
                    position.y -= Config.moveSpeed * Gdx.graphics.getDeltaTime();
                }
            }
            if (Config.currentMode == Mode.SCALE) {
                if (Gdx.input.isKeyPressed(Input.Keys.UP)) {
                    scale.x += Config.moveSpeed * Gdx.graphics.getDeltaTime();
                }
                if (Gdx.input.isKeyPressed(Input.Keys.DOWN)) {
                    scale.x -= Config.moveSpeed * Gdx.graphics.getDeltaTime();
                }
                if (Gdx.input.isKeyPressed(Input.Keys.LEFT)) {
                    scale.z -= Config.moveSpeed * Gdx.graphics.getDeltaTime();
                }
                if (Gdx.input.isKeyPressed(Input.Keys.RIGHT)) {
                    scale.z += Config.moveSpeed * Gdx.graphics.getDeltaTime();
                }
                if (Gdx.input.isKeyPressed(Input.Keys.PAGE_UP)) {
                    scale.y += Config.moveSpeed * Gdx.graphics.getDeltaTime();
                }
                if (Gdx.input.isKeyPressed(Input.Keys.PAGE_DOWN)) {
                    scale.y -= Config.moveSpeed * Gdx.graphics.getDeltaTime();
                }
            }
        }
        boundingBox.set(position, scale);
        instance.transform.set(position, new Quaternion(), scale);
        outlineInstance.transform.set(position, new Quaternion(), new Vector3(scale.x + 0.1f, scale.y + 0.1f, scale.z + 0.1f));

        System.out.println(boundingBox.toString());
    }

    public void render(ModelBatch batch, Environment environment) {
        batch.render(instance, environment);
        if (selected) batch.render(outlineInstance, environment);
    }

    public void dispose() {
        model.dispose();
    }

}
