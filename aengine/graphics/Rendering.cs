using System.Numerics;
using aengine.window;
using OpenTK.Graphics.OpenGL;
using SharpFNT;

namespace aengine.graphics;

public class Rendering {
    public static void clearBackground(Color color) {
        GL.ClearColor(color.r, color.g, color.b, color.a);
    }

    public static void drawLine(Vector3 start, Vector3 end, Color color, float thickness = 3) {
        GL.LineWidth(thickness);
        GL.Begin(PrimitiveType.Lines);
        GL.Color4(color.r, color.g, color.b, color.a);
        GL.Vertex3(start.X, start.Y, start.Z);
        GL.Vertex3(end.X, end.Y, end.Z);
        GL.End();
        GL.LineWidth(1);
    }

    public static void drawDebugAxes(float length = 3) {
        drawLine(Vector3.Zero, Vector3.Zero with { X = length }, Colors.BLUE);
        drawLine(Vector3.Zero, Vector3.Zero with { Y = length }, Colors.MAROON);
        drawLine(Vector3.Zero, Vector3.Zero with { Z = length }, Colors.GREEN);
    }

    public static void drawGrid(int slices, int spacing = 1) {
        for (int i = -slices / 2; i < slices / 2; i++) {
            drawLine(Vector3.Zero with { X = -slices / 2, Z = i * spacing },
                Vector3.Zero with { X = slices / 2, Z = i * spacing }, Colors.WHITE, 1);
            drawLine(Vector3.Zero with { Z = -slices / 2, X = i * spacing },
                Vector3.Zero with { Z = slices / 2, X = i * spacing }, Colors.WHITE, 1);
        }
    }

    public static void drawRectangle(float x, float y, float width, float height, Color color) {
        GL.MatrixMode(MatrixMode.Projection);
        GL.PushMatrix();
        GL.LoadIdentity();
        GL.Ortho(0, Window.getScreenSize().X, Window.getScreenSize().Y, 0, 0,1); // Use an orthographic projection

        GL.MatrixMode(MatrixMode.Modelview);
        GL.PushMatrix();
        GL.LoadIdentity();

        GL.PushMatrix();
        GL.Translate(x, y, 0);
        GL.Scale(width, height, 0);
        
        GL.Begin(PrimitiveType.Quads);
        
        GL.Color4(color.r, color.g, color.b, color.a);
        GL.Vertex2(0, 0);
        GL.Vertex2(0, 1);
        GL.Vertex2(1, 1);
        GL.Vertex2(1, 0);
        
        GL.End();

        GL.PopMatrix();

        GL.MatrixMode(MatrixMode.Projection);
        GL.PopMatrix();
        GL.MatrixMode(MatrixMode.Modelview);
        GL.PopMatrix();
    }
    
    public static void drawRectangleTexture(Texture texture, float x, float y, float width, float height, Color color) {
        GL.MatrixMode(MatrixMode.Projection);
        GL.PushMatrix();
        GL.LoadIdentity();
        GL.Ortho(0, Window.getScreenSize().X, Window.getScreenSize().Y, 0, 0,1); // Use an orthographic projection

        GL.MatrixMode(MatrixMode.Modelview);
        GL.PushMatrix();
        GL.LoadIdentity();

        GL.PushMatrix();
        GL.Translate(x, y, 0);
        GL.Scale(width, height, 0);
        
        GL.Enable(EnableCap.Texture2D);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
            (int)TextureMinFilter.LinearMipmapLinear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        GL.BindTexture(TextureTarget.Texture2D, texture.id);
        
        GL.Begin(PrimitiveType.Quads);
        
        GL.Color4(color.r, color.g, color.b, color.a);
        GL.TexCoord2(0, 0); GL.Vertex2(0, 0);
        GL.TexCoord2(0, 1); GL.Vertex2(0, 1);
        GL.TexCoord2(1, 1); GL.Vertex2(1, 1);
        GL.TexCoord2(1, 0); GL.Vertex2(1, 0);
        
        GL.End();

        GL.BindTexture(TextureTarget.Texture2D, 0);
        
        GL.PopMatrix();

        GL.MatrixMode(MatrixMode.Projection);
        GL.PopMatrix();
        GL.MatrixMode(MatrixMode.Modelview);
        GL.PopMatrix();
    }

    public static void drawText(Font font, string text, float px, float py, float fontSize, Color color) {
        float rx = px;
        float ry = py;

        float cursorX = 0.0f;
        float cursorY = 0.0f;

        GL.MatrixMode(MatrixMode.Projection);
        GL.PushMatrix();
        GL.LoadIdentity();
        GL.Ortho(0, Window.getScreenSize().X, Window.getScreenSize().Y, 0, 0,1); // Use an orthographic projection

        GL.MatrixMode(MatrixMode.Modelview);
        GL.PushMatrix();
        GL.LoadIdentity();

        GL.Enable(EnableCap.Texture2D);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
            (int)TextureMinFilter.LinearMipmapLinear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        if (font.atlas != null) GL.BindTexture(TextureTarget.Texture2D, font.atlas.id);

        GL.PushMatrix();
        GL.Translate(rx, ry, 0);
        GL.Scale(fontSize, fontSize, fontSize);

        foreach (char character in text) {
            Character glyph = font.bitmap.GetCharacter(character);

            // Calculate vertex positions
            if (glyph != null) {
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
                GL.Vertex2(x, y + glyph.YOffset);

                GL.TexCoord2(u1, v2);
                GL.Vertex2(x, y + height + glyph.YOffset);

                GL.TexCoord2(u2, v2);
                GL.Vertex2(x + width, y + height + glyph.YOffset);

                GL.TexCoord2(u2, v1);
                GL.Vertex2(x + width, y + glyph.YOffset);

                GL.End();

                // Update cursor position for the next character
                cursorX += glyph.XAdvance;
            }
        }

        GL.PopMatrix();
        
        GL.Disable(EnableCap.Texture2D);
        GL.BindTexture(TextureTarget.Texture2D, 0);

        GL.MatrixMode(MatrixMode.Projection);
        GL.PopMatrix();
        GL.MatrixMode(MatrixMode.Modelview);
        GL.PopMatrix();
    }
}