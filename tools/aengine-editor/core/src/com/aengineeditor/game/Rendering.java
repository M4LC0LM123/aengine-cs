package com.aengineeditor.game;

import com.badlogic.gdx.graphics.Color;
import com.badlogic.gdx.graphics.VertexAttributes;
import com.badlogic.gdx.graphics.g3d.*;
import com.badlogic.gdx.graphics.g3d.attributes.ColorAttribute;
import com.badlogic.gdx.graphics.g3d.utils.MeshPartBuilder;
import com.badlogic.gdx.graphics.g3d.utils.ModelBuilder;
import com.badlogic.gdx.math.Vector3;

public class Rendering {
    public static void drawGrid(int xDiv, int zDiv, int xSize, int zSize, Color color, ModelBatch batch, Environment environment) {
        ModelBuilder modelBuilder = new ModelBuilder();
        Model model = modelBuilder.createLineGrid(xDiv, zDiv, xSize, zSize, new Material(ColorAttribute.createDiffuse(color)), VertexAttributes.Usage.Position | VertexAttributes.Usage.Normal);
        ModelInstance instance = new ModelInstance(model);

        batch.render(instance, environment);
    }

    public static void drawDebugAxies(float length, ModelBatch batch, Environment environment) {
        ModelBuilder modelBuilder = new ModelBuilder();
        Model xLine = modelBuilder.createArrow(new Vector3(0, 0, 0), new Vector3(length, 0, 0), new Material(ColorAttribute.createDiffuse(Color.BLUE)), VertexAttributes.Usage.Position | VertexAttributes.Usage.Normal);
        Model yLine = modelBuilder.createArrow(new Vector3(0, 0, 0), new Vector3(0, length, 0), new Material(ColorAttribute.createDiffuse(Color.RED)), VertexAttributes.Usage.Position | VertexAttributes.Usage.Normal);
        Model zLine = modelBuilder.createArrow(new Vector3(0, 0, 0), new Vector3(0, 0, length), new Material(ColorAttribute.createDiffuse(Color.GREEN)), VertexAttributes.Usage.Position | VertexAttributes.Usage.Normal);
        ModelInstance xinstance = new ModelInstance(xLine);
        ModelInstance yinstance = new ModelInstance(yLine);
        ModelInstance zinstance = new ModelInstance(zLine);

        batch.render(xinstance, environment);
        batch.render(yinstance, environment);
        batch.render(zinstance, environment);
    }

    public static void drawLine(Vector3 start, Vector3 end, Color color, ModelBatch batch, Environment environment) {
        ModelBuilder modelBuilder = new ModelBuilder();
        Model xLine = modelBuilder.createArrow(start, end, new Material(ColorAttribute.createDiffuse(color)), VertexAttributes.Usage.Position | VertexAttributes.Usage.Normal);
        ModelInstance xinstance = new ModelInstance(xLine);

        batch.render(xinstance, environment);
    }

}
