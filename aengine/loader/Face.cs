using System.Numerics;

namespace aengine.loader
{
    class Face
    {
        public List<Vector3> Vertices { get; }
        public List<Vector2> TexCoords { get; }
        public List<Vector3> Normals { get; }
        public string MaterialName { get; set; }

        public Face()
        {
            Vertices = new List<Vector3>();
            TexCoords = new List<Vector2>();
            Normals = new List<Vector3>();
            MaterialName = String.Empty;
        }
    }

}