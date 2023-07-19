using System.Numerics;

namespace aengine.loader
{
    class Material
    {
        public string Name { get; set; }
        public Vector3 AmbientColor { get; set; }
        public Vector3 DiffuseColor { get; set; }
        public Vector3 SpecularColor { get; set; }
        public float Shininess { get; set; }
        // Additional material properties can be added

        public Material()
        {
            // Set default material properties
            Name = String.Empty;
            AmbientColor = Vector3.One;
            DiffuseColor = Vector3.One;
            SpecularColor = Vector3.One;
            Shininess = 32.0f;
        }
    }
}