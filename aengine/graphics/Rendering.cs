using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Raylib_CsLo;
using static Raylib_CsLo.Raylib;
using static Raylib_CsLo.RlGl;

namespace aengine.graphics
{
    public class Rendering
    {
        private static float linePointSizeDivisor = 3; 
        public static void drawDebugAxies(float length = 4)
        {
            DrawLine3D(Vector3.Zero, new  Vector3(length, 0, 0), BLUE);
            DrawLine3D(Vector3.Zero, new  Vector3(0, length, 0), RED);
            DrawLine3D(Vector3.Zero, new  Vector3(0, 0, length), GREEN);
        }

        public static Color getRandomColor(int a = 255)
        {
            return new Color(GetRandomValue(-1, 256), GetRandomValue(-1, 256), GetRandomValue(-1, 256), a);
        }

        public static void drawArrow(Vector3 startPos, Vector3 endPos, Color color)
        {
            DrawLine3D(startPos, endPos, color);
            float cubeWidth = (endPos.X - startPos.X)/linePointSizeDivisor;
            float cubeHeight = (endPos.Y - startPos.Y)/linePointSizeDivisor;
            float cubeLength = (endPos.Z - startPos.Z)/linePointSizeDivisor;
            Vector3 cubePos = new Vector3(endPos.X + cubeWidth/2, endPos.Y + cubeHeight/2, endPos.Z + cubeLength/2);
            DrawCube(cubePos, cubeWidth, cubeHeight, cubeLength, color);
        }
        
        public static void drawSkyBox(Texture[] textures, Color tint, int scale = 200)
        {
            float fix = 0.5f;
            DrawCubeTexture(textures[0], new Vector3(0, 0, scale/2), scale, -scale, 0, tint);   // front
            DrawCubeTexture(textures[1], new Vector3(0, 0, -scale/2), scale, -scale, 0, tint);   // back
            DrawCubeTexture(textures[2], new Vector3(-scale/2, 0, 0), 0, -scale, scale, tint);   // left
            DrawCubeTexture(textures[3], new Vector3(scale/2, 0, 0), 0, -scale, scale, tint);   // right
            DrawCubeTexture(textures[4], new Vector3(0, scale/2, 0), -scale, 0, -scale, tint);     // top
            DrawCubeTexture(textures[5], new Vector3(0, -scale/2, 0), -scale, 0, scale, tint); // bottom
        }
        
        public static void drawSprite3D(Texture texture, Vector3 position, float width, float height, float rotation, Color color)
        {
            float x = position.X;
            float y = position.Y;
            float z = position.Z;

            rlSetTexture(texture.id);

            rlPushMatrix();
            rlTranslatef(x, y, z);
            rlRotatef(rotation, 0.0f, 1.0f, 0.0f);
            rlTranslatef(-x, -y, -z);

            rlBegin(RL_QUADS);
            rlColor4ub(color.r, color.g, color.b, color.a);
            rlNormal3f(0.0f, 0.0f, 1.0f);       // Normal Pointing Towards Viewer
            rlTexCoord2f(1.0f, 1.0f); rlVertex3f(x - width/2, y - height/2, z);  
            rlTexCoord2f(0.0f, 1.0f); rlVertex3f(x + width/2, y - height/2, z);  
            rlTexCoord2f(0.0f, 0.0f); rlVertex3f(x + width/2, y + height/2, z);  
            rlTexCoord2f(1.0f, 0.0f); rlVertex3f(x - width/2, y + height/2, z);  

            rlNormal3f(0.0f, 0.0f, 1.0f);       // Normal Pointing Towards Viewer
            rlTexCoord2f(1.0f, 1.0f); rlVertex3f(x + width/2, y - height/2, z); 
            rlTexCoord2f(0.0f, 1.0f); rlVertex3f(x - width/2, y - height/2, z); 
            rlTexCoord2f(0.0f, 0.0f); rlVertex3f(x - width/2, y + height/2, z); 
            rlTexCoord2f(1.0f, 0.0f); rlVertex3f(x + width/2, y + height/2, z); 
            rlEnd();
            rlPopMatrix();

            rlSetTexture(0);
        }

        public static void drawCrosshair(Color color, float size = 25, int thickness = 2)
        {
            DrawLineEx(new Vector2(GetScreenWidth()/2 - size/2, GetScreenHeight()/2), new Vector2(GetScreenWidth()/2 + size/2, GetScreenHeight()/2), thickness, color);
            DrawLineEx(new Vector2(GetScreenWidth()/2, GetScreenHeight()/2 - size/2), new Vector2(GetScreenWidth()/2, GetScreenHeight()/2 + size/2), thickness, color);
        }
        
    }   
}