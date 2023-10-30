using System.Numerics;
using Raylib_CsLo;
using static Raylib_CsLo.Raylib;
using static Raylib_CsLo.RlGl;

namespace Editor;

public class Utils
{
    public static Color transBlue = new Color(0, 0, 255, 125);
    public static Color transRed = new Color(255, 0, 0, 125);
    public static Color transGreen = new Color(0, 255, 0, 125);

    public static Color semiWhite = new Color(255, 255, 255, 127);
    
    public static void drawAxisLine(Axis axis, Vector3 start, float length, float thickness, Color color)
    {
        Model model = LoadModelFromMesh(GenMeshCylinder(thickness, length, 15));
        switch (axis)
        {
            case Axis.X:
                model.transform = Matrix4x4.CreateFromYawPitchRoll(0, 0, 90 * RayMath.DEG2RAD);
                break;
            case Axis.Z:
                model.transform = Matrix4x4.CreateFromYawPitchRoll(0, -90 * RayMath.DEG2RAD, 0);
                break;
        }
        
        DrawModel(model, start, 1, color);
        UnloadModel(model);
    }

    public static void drawAxies(Vector3 start, float length)
    {
        drawAxisLine(Axis.X, start, length, 0.05f, BLUE);
        drawAxisLine(Axis.Y, start, length, 0.05f, RED);
        drawAxisLine(Axis.Z, start, length, 0.05f, GREEN);
    }
    
    public static void drawAxiesArrows(Vector3 start, float lengthX, float lengthY, float lengthZ, float radius)
    {
        drawAxisLine(Axis.X, start, lengthX, 0.05f, BLUE);
        DrawSphere(start with { X = start.X + lengthX}, 0.25f, BLUE);
        DrawSphereWires(start with { X = start.X + lengthX}, radius, 15, 15, transBlue);
        
        drawAxisLine(Axis.Y, start, lengthY, 0.05f, RED);
        DrawSphere(start with { Y = start.Y + lengthY}, 0.25f, RED);
        DrawSphereWires(start with { Y = start.Y + lengthY}, radius, 15, 15, transRed);
        
        drawAxisLine(Axis.Z, start, lengthZ, 0.05f, GREEN);
        DrawSphere(start with { Z = start.Z + lengthZ}, 0.25f, GREEN);
        DrawSphereWires(start with { Z = start.Z + lengthZ}, radius, 15, 15, transGreen);
    }
    
    public static unsafe bool IsNaN (float f)
    {
        int binary = *(int*)(&f);
        return ((binary & 0x7F800000) == 0x7F800000) && ((binary & 0x007FFFFF) != 0);
    }
    
    public static void drawCubePro(Vector3 position, Vector3 scale, Vector3 rotation, Color color)
    {
        float x = position.X;
        float y = position.Y;
        float z = position.Z;
        float width = scale.X;
        float height = scale.Y;
        float length = scale.Z;
        
        rlPushMatrix();

            rlTranslatef(x, y, z);
            rlRotatef(rotation.Z, 0, 0, 1.0f);
            rlRotatef(rotation.Y, 0, 1.0f, 0);
            rlRotatef(rotation.X, 1.0f, 0, 0);
            rlTranslatef(-x, -y, -z);

            rlBegin(RL_QUADS);
                rlColor4ub(color.r, color.g, color.b, color.a);
                // Front Face
                rlNormal3f(0.0f, 0.0f, 1.0f);       // Normal Pointing Towards Viewer
                rlTexCoord2f(0.0f, 0.0f); rlVertex3f(x - width/2, y - height/2, z + length/2);  // Bottom Left Of The Texture and Quad
                rlTexCoord2f(1.0f, 0.0f); rlVertex3f(x + width/2, y - height/2, z + length/2);  // Bottom Right Of The Texture and Quad
                rlTexCoord2f(1.0f, 1.0f); rlVertex3f(x + width/2, y + height/2, z + length/2);  // Top Right Of The Texture and Quad
                rlTexCoord2f(0.0f, 1.0f); rlVertex3f(x - width/2, y + height/2, z + length/2);  // Top Left Of The Texture and Quad
                // Back Face
                rlNormal3f(0.0f, 0.0f, - 1.0f);     // Normal Pointing Away From Viewer
                rlTexCoord2f(1.0f, 0.0f); rlVertex3f(x - width/2, y - height/2, z - length/2);  // Bottom Right Of The Texture and Quad
                rlTexCoord2f(1.0f, 1.0f); rlVertex3f(x - width/2, y + height/2, z - length/2);  // Top Right Of The Texture and Quad
                rlTexCoord2f(0.0f, 1.0f); rlVertex3f(x + width/2, y + height/2, z - length/2);  // Top Left Of The Texture and Quad
                rlTexCoord2f(0.0f, 0.0f); rlVertex3f(x + width/2, y - height/2, z - length/2);  // Bottom Left Of The Texture and Quad
                // Top Face
                rlNormal3f(0.0f, 1.0f, 0.0f);       // Normal Pointing Up
                rlTexCoord2f(0.0f, 1.0f); rlVertex3f(x - width/2, y + height/2, z - length/2);  // Top Left Of The Texture and Quad
                rlTexCoord2f(0.0f, 0.0f); rlVertex3f(x - width/2, y + height/2, z + length/2);  // Bottom Left Of The Texture and Quad
                rlTexCoord2f(1.0f, 0.0f); rlVertex3f(x + width/2, y + height/2, z + length/2);  // Bottom Right Of The Texture and Quad
                rlTexCoord2f(1.0f, 1.0f); rlVertex3f(x + width/2, y + height/2, z - length/2);  // Top Right Of The Texture and Quad
                // Bottom Face
                rlNormal3f(0.0f, - 1.0f, 0.0f);     // Normal Pointing Down
                rlTexCoord2f(1.0f, 1.0f); rlVertex3f(x - width/2, y - height/2, z - length/2);  // Top Right Of The Texture and Quad
                rlTexCoord2f(0.0f, 1.0f); rlVertex3f(x + width/2, y - height/2, z - length/2);  // Top Left Of The Texture and Quad
                rlTexCoord2f(0.0f, 0.0f); rlVertex3f(x + width/2, y - height/2, z + length/2);  // Bottom Left Of The Texture and Quad
                rlTexCoord2f(1.0f, 0.0f); rlVertex3f(x - width/2, y - height/2, z + length/2);  // Bottom Right Of The Texture and Quad
                // Right face
                rlNormal3f(1.0f, 0.0f, 0.0f);       // Normal Pointing Right
                rlTexCoord2f(1.0f, 0.0f); rlVertex3f(x + width/2, y - height/2, z - length/2);  // Bottom Right Of The Texture and Quad
                rlTexCoord2f(1.0f, 1.0f); rlVertex3f(x + width/2, y + height/2, z - length/2);  // Top Right Of The Texture and Quad
                rlTexCoord2f(0.0f, 1.0f); rlVertex3f(x + width/2, y + height/2, z + length/2);  // Top Left Of The Texture and Quad
                rlTexCoord2f(0.0f, 0.0f); rlVertex3f(x + width/2, y - height/2, z + length/2);  // Bottom Left Of The Texture and Quad
                // Left Face
                rlNormal3f( - 1.0f, 0.0f, 0.0f);    // Normal Pointing Left
                rlTexCoord2f(0.0f, 0.0f); rlVertex3f(x - width/2, y - height/2, z - length/2);  // Bottom Left Of The Texture and Quad
                rlTexCoord2f(1.0f, 0.0f); rlVertex3f(x - width/2, y - height/2, z + length/2);  // Bottom Right Of The Texture and Quad
                rlTexCoord2f(1.0f, 1.0f); rlVertex3f(x - width/2, y + height/2, z + length/2);  // Top Right Of The Texture and Quad
                rlTexCoord2f(0.0f, 1.0f); rlVertex3f(x - width/2, y + height/2, z - length/2);  // Top Left Of The Texture and Quad
            rlEnd();
        rlPopMatrix();

        rlSetTexture(0);
    }
    
    public static void drawCubeTextured(Texture texture, Vector3 position, Vector3 scale, Vector3 rotation, Color color)
    {
        float x = position.X;
        float y = position.Y;
        float z = position.Z;
        float width = scale.X;
        float height = scale.Y;
        float length = scale.Z;
        
        rlSetTexture(texture.id);
        
        rlPushMatrix();

            rlTranslatef(x, y, z);
            rlRotatef(rotation.Z, 0, 0, 1.0f);
            rlRotatef(rotation.Y, 0, 1.0f, 0);
            rlRotatef(rotation.X, 1.0f, 0, 0);
            rlTranslatef(-x, -y, -z);

            rlBegin(RL_QUADS);
                rlColor4ub(color.r, color.g, color.b, color.a);
                // Front Face
                rlNormal3f(0.0f, 0.0f, 1.0f);       // Normal Pointing Towards Viewer
                rlTexCoord2f(0.0f, 0.0f); rlVertex3f(x - width/2, y - height/2, z + length/2);  // Bottom Left Of The Texture and Quad
                rlTexCoord2f(1.0f, 0.0f); rlVertex3f(x + width/2, y - height/2, z + length/2);  // Bottom Right Of The Texture and Quad
                rlTexCoord2f(1.0f, 1.0f); rlVertex3f(x + width/2, y + height/2, z + length/2);  // Top Right Of The Texture and Quad
                rlTexCoord2f(0.0f, 1.0f); rlVertex3f(x - width/2, y + height/2, z + length/2);  // Top Left Of The Texture and Quad
                // Back Face
                rlNormal3f(0.0f, 0.0f, - 1.0f);     // Normal Pointing Away From Viewer
                rlTexCoord2f(1.0f, 0.0f); rlVertex3f(x - width/2, y - height/2, z - length/2);  // Bottom Right Of The Texture and Quad
                rlTexCoord2f(1.0f, 1.0f); rlVertex3f(x - width/2, y + height/2, z - length/2);  // Top Right Of The Texture and Quad
                rlTexCoord2f(0.0f, 1.0f); rlVertex3f(x + width/2, y + height/2, z - length/2);  // Top Left Of The Texture and Quad
                rlTexCoord2f(0.0f, 0.0f); rlVertex3f(x + width/2, y - height/2, z - length/2);  // Bottom Left Of The Texture and Quad
                // Top Face
                rlNormal3f(0.0f, 1.0f, 0.0f);       // Normal Pointing Up
                rlTexCoord2f(0.0f, 1.0f); rlVertex3f(x - width/2, y + height/2, z - length/2);  // Top Left Of The Texture and Quad
                rlTexCoord2f(0.0f, 0.0f); rlVertex3f(x - width/2, y + height/2, z + length/2);  // Bottom Left Of The Texture and Quad
                rlTexCoord2f(1.0f, 0.0f); rlVertex3f(x + width/2, y + height/2, z + length/2);  // Bottom Right Of The Texture and Quad
                rlTexCoord2f(1.0f, 1.0f); rlVertex3f(x + width/2, y + height/2, z - length/2);  // Top Right Of The Texture and Quad
                // Bottom Face
                rlNormal3f(0.0f, - 1.0f, 0.0f);     // Normal Pointing Down
                rlTexCoord2f(1.0f, 1.0f); rlVertex3f(x - width/2, y - height/2, z - length/2);  // Top Right Of The Texture and Quad
                rlTexCoord2f(0.0f, 1.0f); rlVertex3f(x + width/2, y - height/2, z - length/2);  // Top Left Of The Texture and Quad
                rlTexCoord2f(0.0f, 0.0f); rlVertex3f(x + width/2, y - height/2, z + length/2);  // Bottom Left Of The Texture and Quad
                rlTexCoord2f(1.0f, 0.0f); rlVertex3f(x - width/2, y - height/2, z + length/2);  // Bottom Right Of The Texture and Quad
                // Right face
                rlNormal3f(1.0f, 0.0f, 0.0f);       // Normal Pointing Right
                rlTexCoord2f(1.0f, 0.0f); rlVertex3f(x + width/2, y - height/2, z - length/2);  // Bottom Right Of The Texture and Quad
                rlTexCoord2f(1.0f, 1.0f); rlVertex3f(x + width/2, y + height/2, z - length/2);  // Top Right Of The Texture and Quad
                rlTexCoord2f(0.0f, 1.0f); rlVertex3f(x + width/2, y + height/2, z + length/2);  // Top Left Of The Texture and Quad
                rlTexCoord2f(0.0f, 0.0f); rlVertex3f(x + width/2, y - height/2, z + length/2);  // Bottom Left Of The Texture and Quad
                // Left Face
                rlNormal3f( - 1.0f, 0.0f, 0.0f);    // Normal Pointing Left
                rlTexCoord2f(0.0f, 0.0f); rlVertex3f(x - width/2, y - height/2, z - length/2);  // Bottom Left Of The Texture and Quad
                rlTexCoord2f(1.0f, 0.0f); rlVertex3f(x - width/2, y - height/2, z + length/2);  // Bottom Right Of The Texture and Quad
                rlTexCoord2f(1.0f, 1.0f); rlVertex3f(x - width/2, y + height/2, z + length/2);  // Top Right Of The Texture and Quad
                rlTexCoord2f(0.0f, 1.0f); rlVertex3f(x - width/2, y + height/2, z - length/2);  // Top Left Of The Texture and Quad
            rlEnd();
        rlPopMatrix();

        rlSetTexture(0);
    }
    
}