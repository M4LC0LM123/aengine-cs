using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Raylib_CsLo;
using static Raylib_CsLo.Raylib;

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

    }   
}