using System.Numerics;
using aengine_cs.aengine.parser;
using Raylib_CsLo;

namespace aengine.ecs; 

public class Prefab {
    public static Entity loadPrefab(string path, string name) {
        ParsedData data = Parser.parse(Parser.read(path));
        ParsedObject obj = data.getObject(name);

        string tag = obj.getValue<string>("tag");
        
        Entity result = new Entity(tag);

        float x = obj.getValue<float>("x");
        float y = obj.getValue<float>("y");
        float z = obj.getValue<float>("z");
        
        float width = obj.getValue<float>("width");
        float height = obj.getValue<float>("height");
        float depth = obj.getValue<float>("depth");
        
        float rx = obj.getValue<float>("rx");
        float ry = obj.getValue<float>("ry");
        float rz = obj.getValue<float>("rz");

        result.transform.position = new Vector3(x, y, z);
        result.transform.scale = new Vector3(width, height, depth);
        result.transform.rotation = new Vector3(rx, ry, rz);

        string components = obj.getValue<string>("components");
        char separator = ',';

        string[] componentNameArray = components.Split(separator, StringSplitOptions.TrimEntries);

        foreach (string s in componentNameArray) {
            result.addComponent(loadComponent(result, path, s));
        }
        
        return result;
    }
    
    public static Entity loadEntity(string path, string name) {
        ParsedData data = Parser.parse(Parser.read(path));
        ParsedObject obj = data.getObject(name);

        string tag = obj.getValue<string>("tag");
        
        Entity result = new Entity(tag);

        float x = obj.getValue<float>("x");
        float y = obj.getValue<float>("y");
        float z = obj.getValue<float>("z");
        
        float width = obj.getValue<float>("width");
        float height = obj.getValue<float>("height");
        float depth = obj.getValue<float>("depth");
        
        float rx = obj.getValue<float>("rx");
        float ry = obj.getValue<float>("ry");
        float rz = obj.getValue<float>("rz");

        result.transform.position = new Vector3(x, y, z);
        result.transform.scale = new Vector3(width, height, depth);
        result.transform.rotation = new Vector3(rx, ry, rz);

        return result;
    }
    
    public static Component loadComponent(Entity entity, string path, string name) {
        ParsedData data = Parser.parse(Parser.read(path));
        ParsedObject obj = data.getObject(name);

        string type = obj.getValue<string>("type");
        
        if (type == "MeshComponent" || type == "Mesh") {
            return loadMeshComponent(entity, obj);
        }
        
        if (type == "RigidBodyComponent" || type == "RigidBody") {
            return loadRigidBodyComponent(entity, obj);
        }

        return null;
    }

    public static Component loadMeshComponent(Entity entity, ParsedObject obj) {
        int r = obj.getValue<int>("r");
        int g = obj.getValue<int>("g");
        int b = obj.getValue<int>("b");
        int a = obj.getValue<int>("a");
        Color color = new Color(r, g, b, a);

        ShapeType shape = (ShapeType)obj.getValue<int>("shape");

        string texturePath = obj.getValue<string>("texture");
        string terrainPath = obj.getValue<string>("terrain");

        Texture texture = new Texture();
        Texture terrain = new Texture();

        if (texturePath != "") {
            texture = Raylib.LoadTexture(texturePath);
        }

        if (terrainPath != "") {
            terrain = Raylib.LoadTexture(terrainPath);
        }

        Mesh mesh = new Mesh();
        Model model = new Model();

        if (shape is ShapeType.BOX) {
            mesh = Raylib.GenMeshCube(1, 1, 1);
        }
        else if (shape is ShapeType.SPHERE) {
            mesh = Raylib.GenMeshSphere(1, 15, 15);
        }
        else if (shape is ShapeType.CYLINDER) {
            mesh = Raylib.GenMeshCylinder(1, 1, 15);
        }
        else if (shape is ShapeType.CONE) {
            mesh = Raylib.GenMeshCone(1, 1, 15);
        }
        else if (shape is ShapeType.TERRAIN) {
            mesh = Raylib.GenMeshHeightmap(Raylib.LoadImageFromTexture(terrain), Vector3.One);
        }

        if (shape is ShapeType.MODEL) {
            model = Raylib.LoadModel(obj.getValue<string>("model"));
        }
        else {
            model = Raylib.LoadModelFromMesh(mesh);
        }

        MeshComponent result = new MeshComponent(entity,
            mesh,
            color
        );

        result.scale = obj.getValue<int>("scale");

        result.model = model;
        result.setTexture(texture);

        return result;
    }

    public static Component loadRigidBodyComponent(Entity entity, ParsedObject obj) {
        float mass = obj.getValue<float>("mass");
        ShapeType shape = (ShapeType)obj.getValue<int>("shape");
        BodyType bodyType = (BodyType) obj.getValue<int>("body_type");

        string modelPath = obj.getValue<string>("model");
        string heightmapPath = obj.getValue<string>("heightmap");

        if (modelPath != "") {
            Model model = Raylib.LoadModel(modelPath);
 
            return new RigidBodyComponent(entity,
                model, mass, bodyType
            );
        }
            
        if (heightmapPath != "") {
            Texture heightmap = Raylib.LoadTexture(heightmapPath);

            return new RigidBodyComponent(entity,
                heightmap, mass, bodyType
            );
        }

        RigidBodyComponent result = new RigidBodyComponent(entity,
            mass, bodyType, shape
        );

        return result;
    }
    
}