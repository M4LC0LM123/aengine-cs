using System.Numerics;
using aengine_cs.aengine.parser;
using aengine.graphics;
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
        
        float rx = obj.getValue<float>("rx"); // pitch
        float ry = obj.getValue<float>("ry"); // yaw
        float rz = obj.getValue<float>("rz"); // roll

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

        if (type == "LightComponent" || type == "Light") {
            return loadLightComponent(entity, obj);
        }
        
        if (type == "SpatialAudioComponent" || type == "SpatialAudio" || type == "Sound") {
            return loadSpatialAudioComponent(entity, obj);
        }

        if (type == "FluidComponent" || type == "Fluid") {
            return loadFluidComponent(entity, obj);
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

    public static Component loadLightComponent(Entity entity, ParsedObject obj) {
        float intensity = obj.getValue<float>("intensity");
        
        int r = obj.getValue<int>("r");
        int g = obj.getValue<int>("g");
        int b = obj.getValue<int>("b");
        int a = obj.getValue<int>("a");
        Color color = new Color(r, g, b, a);

        string vertShaderPath = obj.getValue<string>("shader_vert");
        string fragShaderPath = obj.getValue<string>("shader_frag");

        if (vertShaderPath == "")
            vertShaderPath = null;
        
        if (fragShaderPath == "")
            fragShaderPath = null;

        bool enabled = obj.getValue<bool>("enabled");

        LightType type = obj.getValue<LightType>("light_type");

        LightComponent result = new LightComponent(entity,
            new aShader(vertShaderPath, fragShaderPath),
            color,
            type
        );

        result.intensity = intensity;
        result.enabled = enabled;

        return result;
    }

    public static Component loadSpatialAudioComponent(Entity entity, ParsedObject obj) {
        string soundFile = obj.getValue<string>("sound");
        float strength = obj.getValue<float>("strength");
        bool canPlay = obj.getValue<bool>("can_play");

        Sound sound = new Sound();

        if (soundFile != "") {
            sound = Raylib.LoadSound(soundFile);
        }

        SpatialAudioComponent result = new SpatialAudioComponent(entity, sound);

        result.strength = strength;
        result.canPlay = canPlay;
        
        return result;
    }

    public static Component loadFluidComponent(Entity entity, ParsedObject obj) {
        string vertShaderPath = obj.getValue<string>("shader_vert");
        string fragShaderPath = obj.getValue<string>("shader_frag");
        
        if (vertShaderPath == "")
            vertShaderPath = null;
        
        if (fragShaderPath == "")
            fragShaderPath = null;

        string texturePath = obj.getValue<string>("texture");

        Texture texture = new Texture();

        if (texturePath != "")
            texture = Raylib.LoadTexture(texturePath);
        
        int r = obj.getValue<int>("r");
        int g = obj.getValue<int>("g");
        int b = obj.getValue<int>("b");
        int a = obj.getValue<int>("a");
        Color color = new Color(r, g, b, a);

        FluidComponent result = new FluidComponent(entity, 
            new aShader(vertShaderPath, fragShaderPath), texture, color);

        result.freqX = obj.getValue<float>("freqX");
        result.freqY = obj.getValue<float>("freqY");
        result.ampX = obj.getValue<float>("ampX");
        result.ampY = obj.getValue<float>("ampY");
        result.speedX = obj.getValue<float>("speedX");
        result.speedY = obj.getValue<float>("speedY");

        result.resetValues();
        
        return result;
    }
    
}