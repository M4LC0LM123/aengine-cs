using System.Collections.Generic;
using System.Numerics;
using aengine.loader;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace aengine.graphics
{
    class Model
    {
        private List<Mesh> meshes = new List<Mesh>();

        public Model(string path, string mtlpath, Texture texture)
        {
            load(path, mtlpath, texture);
        }

        public void render(System.Numerics.Vector3 position, System.Numerics.Vector3 scale, System.Numerics.Vector3 rotation, Color tint)
        {
            foreach (var mesh in meshes)
                mesh.render(position, scale, rotation, tint);
        }

        private void load(string path, string mtlpath, Texture texture)
        {
            OBJLoader loader = new OBJLoader();
            loader.LoadModel(path);

            // Load MTL file
            MTLLoader mtlLoader = new MTLLoader();
            mtlLoader.LoadMaterials(mtlpath);

            // Create meshes based on the loaded OBJ data
            foreach (Face face in loader.faces)
            {
                List<Vector3> faceVertices = face.Vertices;
                List<Vector2> faceTexCoords = face.TexCoords;
                List<Vector3> faceNormals = face.Normals;

                // Create arrays for vertices, texture coordinates, and normals
                float[] meshVertices = new float[faceVertices.Count * 3];
                float[] meshTexCoords = new float[faceTexCoords.Count * 2];
                float[] meshNormals = new float[faceNormals.Count * 3];
                int[] meshIndices = new int[faceVertices.Count];

                // Copy vertex data into the mesh arrays
                for (int i = 0; i < faceVertices.Count; i++)
                {
                    meshVertices[i * 3] = faceVertices[i].X;
                    meshVertices[i * 3 + 1] = faceVertices[i].Y;
                    meshVertices[i * 3 + 2] = faceVertices[i].Z;

                    meshIndices[i] = i;
                }

                // Copy texture coordinate data into the mesh arrays
                for (int i = 0; i < faceTexCoords.Count; i++)
                {
                    meshTexCoords[i * 2] = faceTexCoords[i].X;
                    meshTexCoords[i * 2 + 1] = faceTexCoords[i].Y;
                }

                // Copy normal data into the mesh arrays
                for (int i = 0; i < faceNormals.Count; i++)
                {
                    meshNormals[i * 3] = faceNormals[i].X;
                    meshNormals[i * 3 + 1] = faceNormals[i].Y;
                    meshNormals[i * 3 + 2] = faceNormals[i].Z;
                }

                // Create a mesh using the loaded data
                Mesh mesh = new Mesh(meshVertices, meshTexCoords, meshNormals, meshIndices, texture);
                mesh.Material = mtlLoader.GetMaterial(face.MaterialName);

                // Add the mesh to the list of meshes
                meshes.Add(mesh);
            }
        }
    }  
}