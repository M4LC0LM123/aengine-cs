using System.Numerics;

namespace aengine.loader
{
    class MTLLoader
    {
        private Dictionary<string, Material> materials = new Dictionary<string, Material>();

        public void LoadMaterials(string mtlpath)
        {
            string[] lines = File.ReadAllLines(mtlpath);
            Material currentMaterial = new Material();

            foreach (string line in lines)
            {
                string[] tokens = line.Split(' ');

                switch (tokens[0])
                {
                    case "newmtl":
                        string materialName = tokens[1];
                        currentMaterial = new Material();
                        materials.Add(materialName, currentMaterial);
                        break;
                    case "Ka":
                        float kaR = float.Parse(tokens[1]);
                        float kaG = float.Parse(tokens[2]);
                        float kaB = float.Parse(tokens[3]);
                        currentMaterial.AmbientColor = new Vector3(kaR, kaG, kaB);
                        break;

                    case "Kd":
                        float kdR = float.Parse(tokens[1]);
                        float kdG = float.Parse(tokens[2]);
                        float kdB = float.Parse(tokens[3]);
                        currentMaterial.DiffuseColor = new Vector3(kdR, kdG, kdB);
                        break;

                    case "Ks":
                        float ksR = float.Parse(tokens[1]);
                        float ksG = float.Parse(tokens[2]);
                        float ksB = float.Parse(tokens[3]);
                        currentMaterial.SpecularColor = new Vector3(ksR, ksG, ksB);
                        break;

                    case "Ns":
                        float shininess = float.Parse(tokens[1]);
                        currentMaterial.Shininess = shininess;
                        break;

                    // Process other material properties like color, texture, etc.
                }
            }
        }

        public Material GetMaterial(string name)
        {
            return materials.TryGetValue(name, out Material material) ? material : null;
        }
    }
}