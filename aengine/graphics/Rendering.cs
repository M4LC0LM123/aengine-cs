using System.Drawing;
using System.Drawing.Imaging;
using aengine.gui;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SharpFNT;
using StbImageSharp;
using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;

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
            GL.Enable(EnableCap.DepthTest); // Enable depth testing
            GL.DepthFunc(DepthFunction.Lequal); // Set depth function

            GL.Enable(EnableCap.Texture2D);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            if (texture != null) GL.BindTexture(TextureTarget.Texture2D, texture.id);

            // Enable blending for transparency
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

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
            GL.TexCoord2(0,  0); GL.Vertex3(-0.5f, 0.5f, -0.5f);
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

            GL.Disable(EnableCap.Blend);
            GL.Disable(EnableCap.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public static void drawSkyBox(Texture[] textures, Color tint, int scale = 200)
        {
            float fix = 0.5f;
            drawSprite3D(textures[1], new System.Numerics.Vector3(scale/2-fix, 0, 0), scale, scale, -90, tint);
            drawSprite3D(textures[0], new System.Numerics.Vector3(-scale/2+fix, 0, 0), scale, scale, 90, tint);
            drawSprite3D(textures[2], new System.Numerics.Vector3(0, 0, scale/2-fix), scale, scale, 180, tint);
            drawSprite3D(textures[3], new System.Numerics.Vector3(0, 0, -scale/2+fix), scale, scale, 0, tint);
            drawPlane(textures[4], new System.Numerics.Vector3(0, scale/2-fix, 0), scale, scale, -90, tint);
            drawPlane(textures[5], new System.Numerics.Vector3(0, -scale/2+fix, 0), scale, scale, -90, tint);
        }

        public static void drawPlane(Texture texture, System.Numerics.Vector3 position, float width, float height, float rotation, Color tint)
        {
            GL.Enable(EnableCap.DepthTest); // Enable depth testing
            GL.DepthFunc(DepthFunction.Lequal); // Set depth function

            GL.Enable(EnableCap.Texture2D);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            if (texture != null) GL.BindTexture(TextureTarget.Texture2D, texture.id);

            // Enable blending for transparency
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            // Render the cube
            GL.PushMatrix();

            GL.Translate(position.X, position.Y, position.Z);
            GL.Rotate(90, Vector3.UnitX);
            GL.Rotate(rotation, Vector3.UnitZ);
            GL.Scale(width, height, 0);

            GL.Begin(PrimitiveType.Quads);

            // Front face
            GL.Color4(tint.r, tint.g, tint.b, tint.a);
            GL.TexCoord2(0, 1); GL.Vertex3(-0.5f, -0.5f, 0.5f);
            GL.TexCoord2(1, 1); GL.Vertex3(0.5f, -0.5f, 0.5f);
            GL.TexCoord2(1, 0); GL.Vertex3(0.5f, 0.5f, 0.5f);
            GL.TexCoord2(0, 0); GL.Vertex3(-0.5f, 0.5f, 0.5f);

            GL.End();

            GL.PopMatrix();

            GL.Disable(EnableCap.Blend);
            GL.Disable(EnableCap.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public static void drawSprite3D(Texture texture, System.Numerics.Vector3 position, float width, float height, float rotation, Color tint)
        {
            GL.Enable(EnableCap.DepthTest); // Enable depth testing
            GL.DepthFunc(DepthFunction.Lequal); // Set depth function

            GL.Enable(EnableCap.Texture2D);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            if (texture != null) GL.BindTexture(TextureTarget.Texture2D, texture.id);

            // Enable blending for transparency
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            // Render the cube
            GL.PushMatrix();

            GL.Translate(position.X, position.Y, position.Z);
            GL.Rotate(rotation, Vector3.UnitY);
            GL.Scale(width, height, 0);

            GL.Begin(PrimitiveType.Quads);

            // Front face
            GL.Color4(tint.r, tint.g, tint.b, tint.a);
            GL.TexCoord2(0, 1); GL.Vertex3(-0.5f, -0.5f, 0.5f);
            GL.TexCoord2(1, 1); GL.Vertex3(0.5f, -0.5f, 0.5f);
            GL.TexCoord2(1, 0); GL.Vertex3(0.5f, 0.5f, 0.5f);
            GL.TexCoord2(0, 0); GL.Vertex3(-0.5f, 0.5f, 0.5f);

            GL.End();

            GL.PopMatrix();

            GL.Disable(EnableCap.Blend);
            GL.Disable(EnableCap.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public static void drawTexturedSphere(Texture texture, System.Numerics.Vector3 position, System.Numerics.Vector3 scale, System.Numerics.Vector3 rotation, Color tint)
        {
            GL.Enable(EnableCap.DepthTest); // Enable depth testing
            GL.DepthFunc(DepthFunction.Lequal); // Set depth function

            int lats = 15;
            int longs = 15;
            GL.Enable(EnableCap.Texture2D);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            if (texture != null) GL.BindTexture(TextureTarget.Texture2D, texture.id);

            // Enable blending for transparency
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

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

            GL.Disable(EnableCap.Blend);
            GL.Disable(EnableCap.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public static void drawTexturedCylinder(Texture texture, System.Numerics.Vector3 position, System.Numerics.Vector3 scale, System.Numerics.Vector3 rotation, Color tint)
        {
            GL.Enable(EnableCap.DepthTest); // Enable depth testing
            GL.DepthFunc(DepthFunction.Lequal); // Set depth function

            int sides = 15;
            int stacks = 15;
            float radius = 2;
            float height = 2;
            GL.Enable(EnableCap.Texture2D);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            if (texture != null) GL.BindTexture(TextureTarget.Texture2D, texture.id);

            // Enable blending for transparency
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            GL.PushMatrix();
            GL.Translate(position.X, position.Y, position.Z);
            GL.Rotate(rotation.X, Vector3.UnitX);
            GL.Rotate(rotation.Y, Vector3.UnitY);
            GL.Rotate(rotation.Z, Vector3.UnitZ);
            GL.Scale(scale.X, scale.Y, scale.Z);

            float stackHeight = height / stacks;
            float stackAngle = 2 * MathHelper.Pi / sides;

            for (int i = 0; i < stacks; i++)
            {
                float stackTop = i * stackHeight;
                float stackBottom = stackTop + stackHeight;

                for (int j = 0; j < sides; j++)
                {
                    float theta = j * stackAngle;
                    float nextTheta = (j + 1) * stackAngle;

                    // Calculate the coordinates of the vertices
                    float x1 = radius * (float)Math.Cos(theta);
                    float y1 = radius * (float)Math.Sin(theta);
                    float x2 = radius * (float)Math.Cos(nextTheta);
                    float y2 = radius * (float)Math.Sin(nextTheta);

                    // Calculate the texture coordinates
                    float s1 = (float)j / sides;
                    float t1 = (float)i / stacks;
                    float s2 = (float)(j + 1) / sides;
                    float t2 = (float)(i + 1) / stacks;

                    // Render the quad
                    GL.Begin(PrimitiveType.Quads);

                    GL.Color4(tint.r, tint.g, tint.b, tint.a); 
                    GL.TexCoord2(s1, t1);
                    GL.Vertex3(x1, y1, stackTop);

                    GL.TexCoord2(s1, t2);
                    GL.Vertex3(x1, y1, stackBottom);

                    GL.TexCoord2(s2, t2);
                    GL.Vertex3(x2, y2, stackBottom);

                    GL.TexCoord2(s2, t1);
                    GL.Vertex3(x2, y2, stackTop);

                    GL.End();
                }
            }

            // Render the top cap
            GL.Begin(PrimitiveType.TriangleFan);
            GL.Normal3(0, 0, 1); // Normal vector pointing up

            GL.Color4(tint.r, tint.g, tint.b, tint.a); 
            GL.TexCoord2(0.5f, 0.5f);
            GL.Vertex3(0, 0, height); // Center vertex

            for (int i = 0; i <= sides; i++)
            {
                float theta = i * stackAngle;
                float x = radius * (float)Math.Cos(theta);
                float y = radius * (float)Math.Sin(theta);

                GL.TexCoord2(x / radius + 0.5f, y / radius + 0.5f);
                GL.Vertex3(x, y, height);
            }

            GL.End();

            // Render the bottom cap
            GL.Begin(PrimitiveType.TriangleFan);
            GL.Normal3(0, 0, -1); // Normal vector pointing down

            GL.Color4(tint.r, tint.g, tint.b, tint.a); 
            GL.TexCoord2(0.5f, 0.5f);
            GL.Vertex3(0, 0, 0); // Center vertex

            for (int i = 0; i <= sides; i++)
            {
                float theta = i * stackAngle;
                float x = radius * (float)Math.Cos(theta);
                float y = radius * (float)Math.Sin(theta);

                GL.TexCoord2(x / radius + 0.5f, y / radius + 0.5f);
                GL.Vertex3(x, y, 0);
            }

            GL.End();

            GL.PopMatrix();

            GL.Disable(EnableCap.Blend);
            GL.Disable(EnableCap.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public static void drawTexturedCapsule(Texture texture, System.Numerics.Vector3 position, System.Numerics.Vector3 scale, System.Numerics.Vector3 rotation, Color tint)
        {
            GL.Enable(EnableCap.DepthTest); // Enable depth testing
            GL.DepthFunc(DepthFunction.Lequal); // Set depth function

            int slices = 15;
            int stacks = 15;
            float radius = 2;
            float height = 2;
            GL.Enable(EnableCap.Texture2D);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            if (texture != null) GL.BindTexture(TextureTarget.Texture2D, texture.id);

            // Enable blending for transparency
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            GL.PushMatrix();
            GL.Translate(position.X, position.Y, position.Z);
            GL.Rotate(rotation.X, Vector3.UnitX);
            GL.Rotate(rotation.Y, Vector3.UnitY);
            GL.Rotate(rotation.Z, Vector3.UnitZ);
            GL.Scale(scale.X, scale.Y, scale.Z);

            float phiStep = (float)Math.PI / stacks;
            float thetaStep = 2.0f * (float)Math.PI / slices;

            for (int stack = 0; stack < stacks; stack++)
            {
                float phi = (float)Math.PI / 2 - stack * phiStep;
                float nextPhi = (float)Math.PI / 2 - (stack + 1) * phiStep;

                GL.Begin(PrimitiveType.Quads);

                for (int slice = 0; slice < slices; slice++)
                {
                    float theta = slice * thetaStep;
                    float nextTheta = (slice + 1) * thetaStep;

                    // Vertices and texture coordinates for the current quad
                    float x1 = radius * (float)Math.Cos(theta) * (float)Math.Cos(phi);
                    float y1 = height * (stack / (float)stacks) - height / 2;
                    float z1 = radius * (float)Math.Sin(theta) * (float)Math.Cos(phi);
                    float u1 = 1.0f - (slice / (float)slices);
                    float v1 = stack / (float)stacks;

                    float x2 = radius * (float)Math.Cos(nextTheta) * (float)Math.Cos(phi);
                    float y2 = height * (stack / (float)stacks) - height / 2;
                    float z2 = radius * (float)Math.Sin(nextTheta) * (float)Math.Cos(phi);
                    float u2 = 1.0f - ((slice + 1) / (float)slices);
                    float v2 = stack / (float)stacks;

                    float x3 = radius * (float)Math.Cos(nextTheta) * (float)Math.Cos(nextPhi);
                    float y3 = height * ((stack + 1) / (float)stacks) - height / 2;
                    float z3 = radius * (float)Math.Sin(nextTheta) * (float)Math.Cos(nextPhi);
                    float u3 = 1.0f - ((slice + 1) / (float)slices);
                    float v3 = (stack + 1) / (float)stacks;

                    float x4 = radius * (float)Math.Cos(theta) * (float)Math.Cos(nextPhi);
                    float y4 = height * ((stack + 1) / (float)stacks) - height / 2;
                    float z4 = radius * (float)Math.Sin(theta) * (float)Math.Cos(nextPhi);
                    float u4 = 1.0f - (slice / (float)slices);
                    float v4 = (stack + 1) / (float)stacks;

                    // Draw the quad
                    GL.Color4(tint.r, tint.g, tint.b, tint.a);
                    GL.TexCoord2(u1, v1);
                    GL.Vertex3(x1, y1, z1);

                    GL.TexCoord2(u2, v2);
                    GL.Vertex3(x2, y2, z2);

                    GL.TexCoord2(u3, v3);
                    GL.Vertex3(x3, y3, z3);

                    GL.TexCoord2(u4, v4);
                    GL.Vertex3(x4, y4, z4);
                }

                GL.End();
            }
            
            GL.PopMatrix();

            GL.Disable(EnableCap.Blend);
            GL.Disable(EnableCap.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public static void drawText(Font font, string text, float px, float py, float fontSize, Color color, GuiWindow window = null)
        {
            float rx = px;
            float ry = py;

            if (window != null)
            {
                rx = window.rect.x + px;
                ry = window.rect.y + py;
            }

            GL.Enable(EnableCap.DepthTest); // Enable depth testing
            GL.DepthFunc(DepthFunction.Lequal); // Set depth function

            float cursorX = 0.0f;
            float cursorY = 0.0f;

            GL.MatrixMode(MatrixMode.Projection);
            GL.PushMatrix();
            GL.LoadIdentity();
            GL.Ortho(0, graphics.Graphics.getScreenSize().X, graphics.Graphics.getScreenSize().Y, 0, 0, 1); // Use an orthographic projection

            GL.MatrixMode(MatrixMode.Modelview);
            GL.PushMatrix();
            GL.LoadIdentity();

            GL.Enable(EnableCap.Texture2D);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            if (font.atlas != null) GL.BindTexture(TextureTarget.Texture2D, font.atlas.id); 

            // Enable blending for transparency
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            GL.PushMatrix();
            GL.Translate(rx, ry, 0);
            GL.Scale(fontSize, fontSize, fontSize);

            foreach (char character in text)
            {
                Character glyph = font.bitmap.GetCharacter(character);

                // Calculate vertex positions
                if (glyph != null)
                {
                    float x = cursorX + glyph.XOffset;
                    float y = cursorY;
                    float width = glyph.Width;
                    float height = glyph.Height;

                    // Calculate texture coordinates
                    float u1 = glyph.X / (float)font.bitmap.Common.ScaleWidth;
                    float v1 = glyph.Y / (float)font.bitmap.Common.ScaleHeight;
                    float u2 = (glyph.X + glyph.Width) / (float)font.bitmap.Common.ScaleWidth;
                    float v2 = (glyph.Y + glyph.Height) / (float)font.bitmap.Common.ScaleHeight;

                    // Render the character using immediate mode
                    GL.Begin(PrimitiveType.Quads);

                    GL.Color4(color.r, color.g, color.b, color.a);
                    GL.TexCoord2(u1, v1);
                    GL.Vertex2(x, y);

                    GL.TexCoord2(u1, v2);
                    GL.Vertex2(x, y + height);

                    GL.TexCoord2(u2, v2);
                    GL.Vertex2(x + width, y + height);

                    GL.TexCoord2(u2, v1);
                    GL.Vertex2(x + width, y);

                    GL.End();

                    // Update cursor position for the next character
                    cursorX += glyph.XAdvance;
                }
            }

            GL.PopMatrix();

            GL.Disable(EnableCap.Blend);
            GL.Disable(EnableCap.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            GL.MatrixMode(MatrixMode.Projection);
            GL.PopMatrix();
            GL.MatrixMode(MatrixMode.Modelview);
            GL.PopMatrix();
        }

        public static void drawTexturedRectangle(Texture texture, float x, float y, float width, float height, Color color)
        {
            GL.Enable(EnableCap.DepthTest); // Enable depth testing
            GL.DepthFunc(DepthFunction.Lequal); // Set depth function

            GL.MatrixMode(MatrixMode.Projection);
            GL.PushMatrix();
            GL.LoadIdentity();
            GL.Ortho(0, graphics.Graphics.getScreenSize().X, graphics.Graphics.getScreenSize().Y, 0, 0, 1); // Use an orthographic projection

            GL.MatrixMode(MatrixMode.Modelview);
            GL.PushMatrix();
            GL.LoadIdentity();

            GL.Enable(EnableCap.Texture2D);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            if (texture != null) GL.BindTexture(TextureTarget.Texture2D, texture.id);

            // Enable blending for transparency
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            GL.PushMatrix();
            GL.Translate(x, y, 0);
            GL.Scale(width, height, 0);

            GL.Begin(PrimitiveType.Quads);

            GL.Color4(color.r, color.g, color.b, color.a);
            GL.TexCoord2(0, 0); GL.Vertex2(0, 0);
            GL.TexCoord2(0, 1); GL.Vertex2(0, 0.5f);
            GL.TexCoord2(1, 1); GL.Vertex2(0.5f, 0.5f);
            GL.TexCoord2(1, 0); GL.Vertex2(0.5f, 0);

            GL.End();

            GL.PopMatrix();

            GL.Disable(EnableCap.Blend);
            GL.Disable(EnableCap.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            GL.MatrixMode(MatrixMode.Projection);
            GL.PopMatrix();
            GL.MatrixMode(MatrixMode.Modelview);
            GL.PopMatrix();
        }

    }

}