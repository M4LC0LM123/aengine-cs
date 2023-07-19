using System;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common; 
using OpenTK.Windowing.Desktop;
using StbImageSharp;

namespace aengine.graphics
{
    public class Texture 
    {
        public int id;
        public int width;
        public int height;
        public byte[] data;

        public Texture(string path)
        {
            id = 0;
            width = 0;
            height = 0;
            data = new byte[]{};
            using (var imageStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var imageResult = ImageResult.FromStream(imageStream, ColorComponents.RedGreenBlueAlpha);
                if (imageResult == null)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Failed to load texture from path: " + path);
                    Console.ForegroundColor = ConsoleColor.White;
                    return;
                }
                id = GL.GenTexture();
                GL.BindTexture(TextureTarget.Texture2D, id);
                width = imageResult.Width;
                height = imageResult.Height;
                data = imageResult.Data;
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Rgba, PixelType.UnsignedByte, data);
                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            }
        }

        public Texture()
        {
            id = 0;
            width = 0;
            height = 0;
            data = new byte[]{};
        }

        public void load(string path)
        {
            using (var imageStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var imageResult = ImageResult.FromStream(imageStream, ColorComponents.RedGreenBlueAlpha);
                if (imageResult == null)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Failed to load texture from path: " + path);
                    Console.ForegroundColor = ConsoleColor.White;
                    return;
                }
                id = GL.GenTexture();
                GL.BindTexture(TextureTarget.Texture2D, id);
                width = imageResult.Width;
                height = imageResult.Height;
                data = imageResult.Data;
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Rgba, PixelType.UnsignedByte, data);
                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            }
        }

    }
}