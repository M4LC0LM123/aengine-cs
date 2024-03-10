namespace Editor;

// shapes in float[] vertices
public static class Shapes {
    public static float[] CUBE_VERTICES = new float[] {
        1, -1, -1,
        1, 1, -1,
        -1, 1, -1,
        -1, -1, -1,
        1, -1, 1,
        1, 1, 1,
        -1, -1, 1,
        -1, 1, 1
    };

    public static uint[] CUBE_INDICES = new uint[] {
        0, 1,
        0, 3,
        0, 4,
        2, 1,
        2, 3,
        2, 7,
        6, 3,
        6, 4,
        6, 7,
        5, 1,
        5, 4,
        5, 7
    };

}