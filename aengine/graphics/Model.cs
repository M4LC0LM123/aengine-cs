using System.Numerics;
using Assimp;
using OpenTK.Graphics.OpenGL;
using PrimitiveType = OpenTK.Graphics.OpenGL.PrimitiveType;

namespace aengine.graphics; 

public class Model {
    private static AssimpContext loader = new AssimpContext();
    
    public Material material;
    public Texture texture;
    public Color color;

    public Vector3 position;
    public Vector3 rotation;
    public Vector3 scale;

    public List<Mesh> meshes;

    public Model(Mesh mesh) {
        material = new Material();
        texture = new Texture();
        color = Colors.WHITE;
        
        position = Vector3.Zero;
        rotation = Vector3.Zero;
        scale = Vector3.One;

        meshes = new List<Mesh>();
        meshes.Add(mesh);
    }
    
    public Model(string path) {
        material = new Material();
        texture = new Texture();
        color = Colors.WHITE;
        
        position = Vector3.Zero;
        rotation = Vector3.Zero;
        scale = Vector3.One;

        meshes = new List<Mesh>();

        Scene scene = loader.ImportFile(path,
            PostProcessSteps.Triangulate | PostProcessSteps.FlipUVs);

        for (int i = 0; i < scene.MeshCount; i++) {
            Mesh temp = new Mesh();
            
            if (scene.Meshes[i].HasVertices) {
                temp.vertices = scene.Meshes[i].Vertices.Select(
                    v => Vector3.Zero with { X = v.X, Y = v.Y, Z = v.Z}
                ).ToList();
            }

            if (scene.Meshes[i].HasNormals) {
                temp.normals = scene.Meshes[i].Normals.Select(
                    n => Vector3.Zero with { X = n.X, Y = n.Y, Z = n.Z}
                ).ToList();
            }

            if (scene.Meshes[i].HasTextureCoords(0)) {
                temp.texCoords = scene.Meshes[i].TextureCoordinateChannels[0].Select(
                    tc => Vector2.Zero with { X = tc.X, Y = tc.Y }
                ).ToList();
            }

            if (scene.Meshes[i].HasFaces) {
                temp.faces = scene.Meshes[i].Faces.Select(
                    f => new Face(f.Indices)
                ).ToList();
            }
            
            if (scene.Meshes[i].HasVertexColors(0)) {
                temp.color.r = scene.Meshes[i].VertexColorChannels[0][0].R;
                temp.color.g = scene.Meshes[i].VertexColorChannels[0][0].G;
                temp.color.b = scene.Meshes[i].VertexColorChannels[0][0].B;
                temp.color.a = scene.Meshes[i].VertexColorChannels[0][0].A;
            }
            
            meshes.Add(temp);
        }
    }

    public void render() {
        GL.PushMatrix();

        material.enable();

        GL.Translate(position.X, position.Y, position.Z);
        GL.Rotate(rotation.Z, 0, 0, 1);
        GL.Rotate(rotation.Y, 0, 1, 0);
        GL.Rotate(rotation.X, 1, 0, 0);
        GL.Scale(scale.X, scale.Y, scale.Z);

        GL.Enable(EnableCap.DepthTest);
        GL.DepthFunc(DepthFunction.Lequal);

        GL.Enable(EnableCap.Texture2D);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
            (int)TextureMinFilter.LinearMipmapLinear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        GL.BindTexture(TextureTarget.Texture2D, texture.id);

        GL.Begin(PrimitiveType.Triangles);

        Vector3 uvw = Vector3.Zero;
        Vector3 normal = Vector3.Zero;

        foreach (var mesh in meshes) {
            for (int i = 0; i < mesh.faces.Count; i++) {
                Face face = mesh.faces[i];
                if (face != null && face.indexCount != 0) {
                    for (int j = 0; j < face.indexCount; j++) {
                        int vertexIndex = face.indices[j];

                        normal.X = mesh.normals[vertexIndex].X;
                        normal.Y = mesh.normals[vertexIndex].Y;
                        normal.Z = mesh.normals[vertexIndex].Z;

                        uvw.X = mesh.texCoords[vertexIndex].X;
                        uvw.Y = mesh.texCoords[vertexIndex].Y;

                        GL.Normal3(normal.X, normal.Y, normal.Z);
                        GL.TexCoord2(uvw.X, 1 - uvw.Y);
                        GL.Color4(mesh.color.r, mesh.color.g, mesh.color.b, mesh.color.a);
                        GL.Vertex3(mesh.vertices[vertexIndex].X, mesh.vertices[vertexIndex].Y,
                            mesh.vertices[vertexIndex].Z);
                    }
                }
            }
        }

        GL.End();

        GL.BindTexture(TextureTarget.Texture2D, 0);

        GL.PopMatrix();
    }

}