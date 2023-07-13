using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace aengine.loader
{
    class OBJLoader
    {
        public List<Vector3> vertices;
        public List<Vector2> texCoords;
        public List<Vector3> normals;
        public List<Face> faces;

        public OBJLoader()
        {
            vertices = new List<Vector3>();
            texCoords = new List<Vector2>();
            normals = new List<Vector3>();
            faces = new List<Face>();
        }

        public void LoadModel(string objPath)
        {
            string[] lines = File.ReadAllLines(objPath);

            foreach (string line in lines)
            {
                string[] tokens = line.Split(' ');

                switch (tokens[0])
                {
                    case "v":
                        float x = float.Parse(tokens[1]);
                        float y = float.Parse(tokens[2]);
                        float z = float.Parse(tokens[3]);
                        vertices.Add(new Vector3(x, y, z));
                        break;

                    case "vt":
                        float u = float.Parse(tokens[1]);
                        float v = float.Parse(tokens[2]);
                        texCoords.Add(new Vector2(u, v));
                        break;

                    case "vn":
                        float nx = float.Parse(tokens[1]);
                        float ny = float.Parse(tokens[2]);
                        float nz = float.Parse(tokens[3]);
                        normals.Add(new Vector3(nx, ny, nz));
                        break;

                    case "f":
                        Face face = new Face();

                        for (int i = 1; i < tokens.Length; i++)
                        {
                            string[] faceTokens = tokens[i].Split('/');
                            int vertexIndex = int.Parse(faceTokens[0]) - 1;
                            int texCoordIndex = int.Parse(faceTokens[1]) - 1;
                            int normalIndex = int.Parse(faceTokens[2]) - 1;
                            face.Vertices.Add(vertices[vertexIndex]);
                            face.TexCoords.Add(texCoords[texCoordIndex]);
                            face.Normals.Add(normals[normalIndex]);
                        }
                        face.MaterialName = "DefaultMaterial";
                        faces.Add(face);
                        break;
                }
            }
        }
    }
}