using System;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common; 
using OpenTK.Windowing.Desktop;
using StbImageSharp;

namespace aengine.graphics
{
    public class Rendering 
    {

        private static Quaternion EulerToQuaternion(double roll, double pitch, double yaw)
        {
            double cr = Math.Cos(roll * 0.5);
            double sr = Math.Sin(roll * 0.5);
            double cp = Math.Cos(pitch * 0.5);
            double sp = Math.Sin(pitch * 0.5);
            double cy = Math.Cos(yaw * 0.5);
            double sy = Math.Sin(yaw * 0.5);

            Quaternion q = new Quaternion();
            q.W = (float)(cr * cp * cy + sr * sp * sy);
            q.X = (float)(sr * cp * cy - cr * sp * sy);
            q.Y = (float)(cr * sp * cy + sr * cp * sy);
            q.Z = (float)(cr * cp * sy - sr * sp * cy);
            return q;
        }

        public static void clearBackground(Color color)
        {
            GL.ClearColor(color.r, color.g, color.b, color.a);
        }

        public static void drawLine(System.Numerics.Vector3 start, System.Numerics.Vector3 end, Color color)
        {
            GL.Begin(PrimitiveType.Lines);
            GL.Color4(color.r, color.g, color.b, color.a);
            GL.Vertex3(start.X, start.Y, start.Z);
            GL.Vertex3(end.X, end.Y, end.Z);
            GL.End();
        }

        public static void drawDebugAxies(float length = 3)
        {
            drawLine(new System.Numerics.Vector3(0, 0, 0), new System.Numerics.Vector3(length, 0, 0), Colors.BLUE);
            drawLine(new System.Numerics.Vector3(0, 0, 0), new System.Numerics.Vector3(0, length, 0), Colors.MAROON);
            drawLine(new System.Numerics.Vector3(0, 0, 0), new System.Numerics.Vector3(0, 0, length), Colors.GREEN);
        }

        public static void drawTexturedCube(Texture texture, System.Numerics.Vector3 position, System.Numerics.Vector3 scale, System.Numerics.Vector3 rotation, Color tint)
        {
            GL.Enable(EnableCap.Texture2D);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.BindTexture(TextureTarget.Texture2D, texture.id);

            // Render the cube
            GL.PushMatrix();

            GL.Translate(position.X, position.Y, position.Z);
            GL.Rotate(rotation.X, Vector3.UnitX);
            GL.Rotate(rotation.Y, Vector3.UnitY);
            GL.Rotate(rotation.Z, Vector3.UnitZ);
            GL.Scale(scale.X, scale.Y, scale.Z);

            GL.Begin(PrimitiveType.Quads);

            // Front face
            GL.Color4(tint.r, tint.g, tint.b, tint.a);
            GL.TexCoord2(0, 0); GL.Vertex3(-0.5f, -0.5f, 0.5f);
            GL.TexCoord2(1, 0); GL.Vertex3(0.5f, -0.5f, 0.5f);
            GL.TexCoord2(1, 1); GL.Vertex3(0.5f, 0.5f, 0.5f);
            GL.TexCoord2(0, 1); GL.Vertex3(-0.5f, 0.5f, 0.5f);

            // Back face
            GL.TexCoord2(0, 0); GL.Vertex3(-0.5f, -0.5f, -0.5f);
            GL.TexCoord2(1, 0); GL.Vertex3(0.5f, -0.5f, -0.5f);
            GL.TexCoord2(1, 1); GL.Vertex3(0.5f, 0.5f, -0.5f);
            GL.TexCoord2(0, 1); GL.Vertex3(-0.5f, 0.5f, -0.5f);

            // Left face
            GL.TexCoord2(0, 0); GL.Vertex3(-0.5f, -0.5f, -0.5f);
            GL.TexCoord2(1, 0); GL.Vertex3(-0.5f, 0.5f, -0.5f);
            GL.TexCoord2(1, 1); GL.Vertex3(-0.5f, 0.5f, 0.5f);
            GL.TexCoord2(0, 1); GL.Vertex3(-0.5f, -0.5f, 0.5f);

            // Right face
            GL.TexCoord2(0, 0); GL.Vertex3(0.5f, -0.5f, -0.5f);
            GL.TexCoord2(1, 0); GL.Vertex3(0.5f, 0.5f, -0.5f);
            GL.TexCoord2(1, 1); GL.Vertex3(0.5f, 0.5f, 0.5f);
            GL.TexCoord2(0, 1); GL.Vertex3(0.5f, -0.5f, 0.5f);

            // Top face
            GL.TexCoord2(0, 0); GL.Vertex3(-0.5f, 0.5f, -0.5f);
            GL.TexCoord2(1, 0); GL.Vertex3(0.5f, 0.5f, -0.5f);
            GL.TexCoord2(1, 1); GL.Vertex3(0.5f, 0.5f, 0.5f);
            GL.TexCoord2(0, 1); GL.Vertex3(-0.5f, 0.5f, 0.5f);

            // Bottom face
            GL.TexCoord2(0, 0); GL.Vertex3(-0.5f, -0.5f, -0.5f);
            GL.TexCoord2(1, 0); GL.Vertex3(0.5f, -0.5f, -0.5f);
            GL.TexCoord2(1, 1); GL.Vertex3(0.5f, -0.5f, 0.5f);
            GL.TexCoord2(0, 1); GL.Vertex3(-0.5f, -0.5f, 0.5f);

            GL.End();

            GL.PopMatrix();

            GL.Disable(EnableCap.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public static void drawTexturedSphere(Texture texture, System.Numerics.Vector3 position, System.Numerics.Vector3 scale, System.Numerics.Vector3 rotation, Color tint)
        {
            int lats = 50;
            int longs = 50;
            GL.Enable(EnableCap.Texture2D);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.BindTexture(TextureTarget.Texture2D, texture.id); 

            GL.PushMatrix();
            GL.Translate(position.X, position.Y, position.Z);
            GL.Rotate(rotation.X, Vector3.UnitX);
            GL.Rotate(rotation.Y, Vector3.UnitY);
            GL.Rotate(rotation.Z, Vector3.UnitZ);
            GL.Scale(scale.X, scale.Y, scale.Z);

            int i, j;
            double r = 1;
            for (i = 0; i <= lats; i++)
            {
                double lat0 = Math.PI * (-0.5 + (double)(i - 1) / lats);
                double z0 = Math.Sin(lat0);
                double zr0 = Math.Cos(lat0);

                double lat1 = Math.PI * (-0.5 + (double)i / lats);
                double z1 = Math.Sin(lat1);
                double zr1 = Math.Cos(lat1);

                GL.Begin(PrimitiveType.QuadStrip);
                for (j = 0; j <= longs; j++)
                {
                    double lng = 2 * Math.PI * (double)(j - 1) / longs;
                    double x = Math.Cos(lng);
                    double y = Math.Sin(lng);

                    // Calculate texture coordinates
                    double s = (double)j / longs;
                    double t0 = (double)(i - 1) / lats;
                    double t1 = (double)i / lats;

                    GL.Color4(tint.r, tint.g, tint.b, tint.a);
                    GL.TexCoord2(s, t0); GL.Normal3(x * zr0, y * zr0, z0);
                    GL.Vertex3(r * x * zr0, r * y * zr0, r * z0);

                    GL.TexCoord2(s, t1);
                    GL.Normal3(x * zr1, y * zr1, z1);
                    GL.Vertex3(r * x * zr1, r * y * zr1, r * z1);
                }
                GL.End();
            }

            GL.PopMatrix();

            GL.Disable(EnableCap.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

    }

}