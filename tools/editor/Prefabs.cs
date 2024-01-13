using static aengine.core.aengine;

namespace Editor;

public static class Prefabs {
    public static readonly string[] PREFABS_CONTENT = new string[] {
        "#mod prefab",
        "object block {",
        $"   str tag = {QUOTE}block{QUOTE};",
        "   f32 x = 0;",
        "   f32 y = 0;",
        "   f32 z = 0;",
        "   f32 width = 1;",
        "   f32 height = 1;",
        "   f32 depth = 1;",
        "   f32 rx = 0;",
        "   f32 ry = 0;",
        "   f32 rz = 0;",
        $"   str components = {QUOTE}block_mesh, block_rb{QUOTE};",
        "}",
        "object block_mesh {",
        $"   str type = {QUOTE}MeshComponent{QUOTE};",
        "   i32 shape = 0;",
        "   i32 r = 255;",
        "   i32 g = 255;",
        "   i32 b = 255;",
        "   i32 a = 255;",
        "   bool isModel = true;",
        $"   str texture = {QUOTE}{QUOTE};",
        $"   str model = {QUOTE}{QUOTE};",
        $"   str heightmap = {QUOTE}{QUOTE};",
        "   i32 scale = 1;",
        "}",
        "object block_rb {",
        $"   str type = {QUOTE}RigidBodyComponent{QUOTE};",
        "   f32 mass = 1;",
        "   i32 shape = 0;",
        "   i32 body_type = 0;",
        $"   str model = {QUOTE}{QUOTE};",
        $"   str heightmap = {QUOTE}{QUOTE};",
        "}"
    };
}