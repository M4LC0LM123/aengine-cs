using System.Collections.Generic;
using System.Numerics;
using aengine.loader;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace aengine.graphics
{
    class Mesh
    {
        private int vao;
        private int vbo;
        private int ebo;
        private Texture texture;

        public List<Vector3> Vertices { get; }
        public List<Vector2> TexCoords { get; }
        public List<Vector3> Normals { get; }
        public int[] Indices { get; }
        public Material Material { get; set; }

        public Mesh(float[] vertices, float[] texCoords, float[] normals, int[] indices, Texture texture)
        {
            this.texture = texture;

            vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);

            vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            ebo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(int), indices, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));
            GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 5 * sizeof(float));
            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            GL.EnableVertexAttribArray(2);

            GL.BindVertexArray(0);

            Vertices = new List<Vector3>();
            TexCoords = new List<Vector2>();
            Normals = new List<Vector3>();
            Material = new Material();

            Vertices = ConvertToVector3List(vertices);
            TexCoords = ConvertToVector2List(texCoords);
            Normals = ConvertToVector3List(normals);
            Indices = indices;
        }

        private static List<Vector3> ConvertToVector3List(float[] array)
        {
            List<Vector3> result = new List<Vector3>();

            for (int i = 0; i < array.Length; i += 3)
            {
                result.Add(new Vector3(array[i], array[i + 1], array[i + 2]));
            }

            return result;
        }

        private static List<Vector2> ConvertToVector2List(float[] array)
        {
            List<Vector2> result = new List<Vector2>();

            for (int i = 0; i < array.Length; i += 2)
            {
                result.Add(new Vector2(array[i], array[i + 1]));
            }

            return result;
        }

        public void render(Vector3 position, Vector3 scale, Vector3 rotation, Color tint)
        {
            GL.Enable(EnableCap.Texture2D);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            if (texture != null) GL.BindTexture(TextureTarget.Texture2D, texture.id);

            // Enable blending for transparency
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            GL.PushMatrix();

            GL.Translate(position.X, position.Y, position.Z);
            GL.Rotate(rotation.X, OpenTK.Mathematics.Vector3.UnitX);
            GL.Rotate(rotation.Y, OpenTK.Mathematics.Vector3.UnitY);
            GL.Rotate(rotation.Z, OpenTK.Mathematics.Vector3.UnitZ);
            GL.Scale(scale.X, scale.Y, scale.Z);
            GL.Color4(tint.r, tint.g, tint.b, tint.a);

            GL.Begin(PrimitiveType.Triangles);

            for (int i = 0; i < Indices.Length; i += 3)
            {
                int index1 = Indices[i];
                int index2 = Indices[i + 1];
                int index3 = Indices[i + 2];

                Vector3 vertex1 = Vertices[index1];
                Vector3 vertex2 = Vertices[index2];
                Vector3 vertex3 = Vertices[index3];

                Vector2 texCoord1 = TexCoords[index1];
                Vector2 texCoord2 = TexCoords[index2];
                Vector2 texCoord3 = TexCoords[index3];

                GL.TexCoord2(texCoord1.X, texCoord1.Y);
                GL.Vertex3(vertex1.X, vertex1.Y, vertex1.Z);

                GL.TexCoord2(texCoord2.X, texCoord2.Y);
                GL.Vertex3(vertex2.X, vertex2.Y, vertex2.Z);

                GL.TexCoord2(texCoord3.X, texCoord3.Y);
                GL.Vertex3(vertex3.X, vertex3.Y, vertex3.Z);
            }

            GL.End();

            GL.PopMatrix();

            // GL.Disable(EnableCap.Blend);
            GL.Disable(EnableCap.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }
    }
}