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
        private int vertexCount;

        public List<Vector3> Vertices { get; }
        public List<Vector2> TexCoords { get; }
        public List<Vector3> Normals { get; }
        public int[] Indices { get; }
        public Material Material { get; set; }

        public Mesh(float[] vertices, float[] texCoords, float[] normals, int[] indices)
        {
            vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);

            vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            ebo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(int), indices, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.BindVertexArray(0);

            vertexCount = indices.Length;

            Vertices = ConvertToVector3List(vertices);
            TexCoords = ConvertToVector2List(texCoords);
            Normals = ConvertToVector3List(normals);
            Indices = indices;
        }

        private List<Vector3> ConvertToVector3List(float[] array)
        {
            List<Vector3> result = new List<Vector3>();

            for (int i = 0; i < array.Length; i += 3)
            {
                result.Add(new Vector3(array[i], array[i + 1], array[i + 2]));
            }

            return result;
        }

        private List<Vector2> ConvertToVector2List(float[] array)
        {
            List<Vector2> result = new List<Vector2>();

            for (int i = 0; i < array.Length; i += 2)
            {
                result.Add(new Vector2(array[i], array[i + 1]));
            }

            return result;
        }

        public void Render()
        {
            GL.BindVertexArray(vao);
            GL.DrawElements(PrimitiveType.Triangles, vertexCount, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);
        }
    }
}