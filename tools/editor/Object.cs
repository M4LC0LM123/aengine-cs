using System;
using System.IO;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Numerics;
using Raylib_CsLo;
using static Raylib_CsLo.Raylib;

namespace Editor;

public class Object {
    public static int R_MULTIPLIER = 50;
    public static int G_MULTIPLIER = 60;
    public static int B_MULTIPLIER = 70;

    public Vector3 position;
    public Vector3 rotation;
    public Vector3 scale;
    public int id;
    public bool selected;
    public Color color;

    public Model model;

    public static Texture texture = LoadTextureFromImage(GenImageChecked(15, 15, 4, 4, WHITE, BLACK));

    public Object(int id) {
        position = Vector3.Zero;
        rotation = Vector3.Zero;
        scale = Vector3.One * 2;
        this.id = id;
        selected = false;
        color = new Color(R_MULTIPLIER * id, G_MULTIPLIER * id, B_MULTIPLIER * id, 255);

        model = LoadModelFromMesh(GenMeshCube(scale.X, scale.Y, scale.Z));
    }

    public Object(int id, Vector3 position) {
        this.position = position;
        rotation = Vector3.Zero;
        scale = Vector3.One * 2;
        this.id = id;
        selected = false;
        color = new Color(R_MULTIPLIER * id, G_MULTIPLIER * id, B_MULTIPLIER * id, 255);

        model = LoadModelFromMesh(GenMeshCube(scale.X, scale.Y, scale.Z));
    }

    public void update() {
        // if (AxieMover.ACTIVE_ENT != this)
        //     selected = false;

        model.transform = Matrix4x4.CreateFromYawPitchRoll(rotation.Y * RayMath.DEG2RAD, rotation.X * RayMath.DEG2RAD,
            rotation.Z * RayMath.DEG2RAD);
    }

    public void render(bool outlined) {
        if (!outlined) Utils.drawCubeTextured(texture, position, scale, rotation, color);

        if (selected)
            DrawCubeWiresV(position, scale, YELLOW);
        else
            DrawCubeWiresV(position, scale, WHITE);
    }

    public Dictionary<string, object> ToDictionary() {
        return new Dictionary<string, object> {
            { "x", position.X },
            { "y", position.Y },
            { "z", position.Z },
            { "w", scale.X },
            { "h", scale.Y },
            { "d", scale.Z },
            { "rx", rotation.X },
            { "ry", rotation.Y },
            { "rz", rotation.Z },
            { "id", id }
        };
    }
}