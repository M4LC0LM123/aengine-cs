using System.Numerics;
using aengine_cs.aengine.parser;
using aengine.graphics;
using Raylib_CsLo;

namespace aengine.ecs;

public class Prefab {
    private static string open = "{";
    private static string close = "}";

    public static void loadScene(string path, string name) {
        string prevDir = Directory.GetCurrentDirectory();
        // Console.WriteLine("prev dir: " + prevDir);

        ParsedData data = Parser.parse(Parser.read(path));
        ParsedObject obj = data.getObject(name);

        string newDir = prevDir + "\\" + Path.GetDirectoryName(path);
        string newPath = prevDir + "\\" + path.Replace("/", "\\");
        // Console.WriteLine("new dir " + newDir);
        // Console.WriteLine("new path " + newPath);
        Directory.SetCurrentDirectory(newDir);

        foreach (string attribute in data.dataKeys(name)) {
            string entityPath = obj.getValue<string>(attribute);
            World.entities.Add(loadPrefab(entityPath, attribute));
        }

        Directory.SetCurrentDirectory(prevDir);
    }

    public static void savePrefab(string path, string name, Entity entity) {
        saveEntity(path, name, entity);

        foreach (var component in entity.components) {
            if (component.GetType() == typeof(MeshComponent)) {
                using (StreamWriter sw = File.AppendText(path)) {
                    sw.WriteLine(m_saveMeshComponent(component.fileName(), entity.getComponent<MeshComponent>()));   
                }
                
                // File.AppendAllText(path,
                //     m_saveMeshComponent(component.fileName(), entity.getComponent<MeshComponent>()) +
                //     Environment.NewLine);
            }
            else if (component.GetType() == typeof(RigidBodyComponent)) {
                using (StreamWriter sw = File.AppendText(path)) {
                    sw.WriteLine(m_saveRigidBody(component.fileName(), entity.getComponent<RigidBodyComponent>()));   
                }
                
                // File.AppendAllText(path,
                //     m_saveRigidBody(component.fileName(), entity.getComponent<RigidBodyComponent>()) +
                //     Environment.NewLine);
            }
        }
    }

    public static void saveEntity(string path, string name, Entity entity) {
        string components = String.Empty;
        foreach (Component component in entity.components) {
            components += component.fileName() + ", ";
        }

        string text = $@"object {name} {open}
    str tag = {entity.tag};
    
    f32 x = {entity.transform.position.X};
    f32 y = {entity.transform.position.Y};
    f32 z = {entity.transform.position.Z};

    f32 width  = {entity.transform.scale.X};
    f32 height = {entity.transform.scale.Y};
    f32 depth  = {entity.transform.scale.Z};

    f32 rx = {entity.transform.rotation.X}; // rotation x (pitch)
    f32 ry = {entity.transform.rotation.Y}; // rotation y (yaw)
    f32 rz = {entity.transform.rotation.Z}; // rotation z (roll)

    str components = {core.aengine.QUOTE}{components}{core.aengine.QUOTE};
{close}";

        File.WriteAllText(path, text);
    }

    private static string m_saveMeshComponent(string name, MeshComponent component) {
        string text = $@"object {name} {open}
    str type = {core.aengine.QUOTE}MeshComponent{core.aengine.QUOTE};
    
    i32 r = {component.color.r};
    i32 g = {component.color.g};
    i32 b = {component.color.b};
    i32 a = {component.color.a};

    bool isModel = {component.isModel};

    str texture = {core.aengine.QUOTE}{core.aengine.QUOTE};
    str model = {core.aengine.QUOTE}{core.aengine.QUOTE};
    
    str terrain = {core.aengine.QUOTE}{core.aengine.QUOTE};

    i32 scale = {component.scale};    
{close}";

        return text;
    }

    private static string m_saveRigidBody(string name, RigidBodyComponent component) {
        string text = $@"object {name} {open}
    str type = {core.aengine.QUOTE}RigidBodyComponent{core.aengine.QUOTE};
    
    f32 mass = {component.body.Mass};

    i32 shape = {component.shapeType};
    i32 body_type = {Convert.ToInt32(component.body.IsStatic)};

    str model = {core.aengine.QUOTE}{core.aengine.QUOTE};

    str heightmap = {core.aengine.QUOTE}{core.aengine.QUOTE};
{close}";

        return text;
    }

    public static Entity loadPrefab(string path, string name) {
        string prevDir = Directory.GetCurrentDirectory();
        // Console.WriteLine("prev dir: " + prevDir);

        ParsedData data = Parser.parse(Parser.read(path));
        ParsedObject obj = data.getObject(name);

        string newDir = prevDir + "\\" + Path.GetDirectoryName(path);
        string newPath = prevDir + "\\" + path.Replace("/", "\\");
        // Console.WriteLine("new dir " + newDir);
        // Console.WriteLine("new path " + newPath);
        Directory.SetCurrentDirectory(newDir);

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
            // Console.WriteLine("comp path load: " + newPath);
            result.addComponent(m_loadComponent(result, newPath, s));
        }

        Directory.SetCurrentDirectory(prevDir);
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

    private static Component m_loadComponent(Entity entity, string path, string name) {
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
            mesh = Raylib.GenMeshCube(entity.transform.scale.X, entity.transform.scale.Y, entity.transform.scale.Z);
        }
        else if (shape is ShapeType.SPHERE) {
            mesh = Raylib.GenMeshSphere(entity.transform.scale.X * 0.5f, 15, 15);
        }
        else if (shape is ShapeType.CYLINDER) {
            mesh = Raylib.GenMeshCylinder(entity.transform.scale.X * 0.5f, entity.transform.scale.Y, 15);
        }
        else if (shape is ShapeType.CONE) {
            mesh = Raylib.GenMeshCone(entity.transform.scale.X * 0.5f, entity.transform.scale.Y, 15);
        }
        else if (shape is ShapeType.TERRAIN) {
            mesh = Raylib.GenMeshHeightmap(Raylib.LoadImageFromTexture(terrain), entity.transform.scale);
        }

        if (shape is ShapeType.MODEL) {
            // Console.WriteLine(obj.getValue<string>("model"));
            // Console.WriteLine(Directory.GetCurrentDirectory());
            //
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
        BodyType bodyType = (BodyType)obj.getValue<int>("body_type");

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