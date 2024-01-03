using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using aengine_cs.aengine.parser;
using aengine.graphics;
using Raylib_CsLo;

namespace aengine.ecs;

public class Prefab {
    private static string open = "{";
    private static string close = "}";

    public static void loadScene(string path, string name, bool changeDir = true) {
        string prevDir = String.Empty;
        if (changeDir) prevDir = Directory.GetCurrentDirectory();
        // Console.WriteLine("prev dir: " + prevDir);
        
        ParsedData data = Parser.parse(Parser.read(path));
        ParsedObject obj = data.getObject(name);

        if (obj.modifier != "scene") {
            Console.WriteLine($"Object {name} isn't a scene");
            return;
        }

        if (changeDir) {
            string newDir = String.Empty;
        
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
                newDir = prevDir + "/" + Path.GetDirectoryName(path);
            } else {
                newDir = prevDir + "\\" + Path.GetDirectoryName(path);  
            }
        
            // string newPath = prevDir + "\\" + path.Replace("/", "\\");
            // Console.WriteLine("new dir " + newDir);
            // Console.WriteLine("new path " + newPath);
            Directory.SetCurrentDirectory(newDir);
        }
        
        foreach (string attribute in data.dataKeys(name)) {
            string entityPath = obj.getValue<string>(attribute);
            loadPrefab(entityPath, attribute);
        }

        if (changeDir) Directory.SetCurrentDirectory(prevDir);
    }

    public static void saveScene(string path, string name) {
        StringBuilder sceneContent = new StringBuilder();
        
        sceneContent.AppendLine("#mod scene");
        sceneContent.AppendLine("object " + name + " {");
        
        for (var i = 0; i < World.entities.Count; i++) {
            sceneContent.AppendLine($"    str {World.entities[i].tag} = \"{Path.GetFileName(path)}\";");
        }
        sceneContent.AppendLine("}");
        
        sceneContent.AppendLine();

        File.WriteAllText(path, sceneContent.ToString());
        
        for (var i = 0; i < World.entities.Count; i++) {
            savePrefab(path, World.entities[i].tag, World.entities[i], false);
        }
    }

    public static void savePrefab(string path, string name, Entity entity, bool clean = true) {
        using (StreamWriter sw = File.AppendText(path)) {
            sw.WriteLine("#mod prefab");   
        }
        saveEntity(path, name, entity, clean);

        if (entity.hasComponent<MeshComponent>()) {
            using (StreamWriter sw = File.AppendText(path)) {
                MeshComponent mesh = entity.getComponent<MeshComponent>(); 
                sw.WriteLine(m_saveMeshComponent(entity.tag + "_" + mesh.fileName(), mesh));   
            }
        }
        
        if (entity.hasComponent<RigidBodyComponent>()) {
            using (StreamWriter sw = File.AppendText(path)) {
                RigidBodyComponent rb = entity.getComponent<RigidBodyComponent>();
                sw.WriteLine(m_saveRigidBody(entity.tag + "_" + rb.fileName(), rb));   
            }
        }
        
        if (entity.hasComponent<SpatialAudioComponent>()) {
            using (StreamWriter sw = File.AppendText(path)) {
                SpatialAudioComponent sa = entity.getComponent<SpatialAudioComponent>();
                sw.WriteLine(m_saveSoundComponent(entity.tag + "_" + sa.fileName(), sa));   
            }
        }
        
        if (entity.hasComponent<LightComponent>()) {
            using (StreamWriter sw = File.AppendText(path)) {
                LightComponent l = entity.getComponent<LightComponent>();
                sw.WriteLine(m_saveLightComponent(entity.tag + "_" + l.fileName(), l));   
            }
        }
        
        if (entity.hasComponent<FluidComponent>()) {
            using (StreamWriter sw = File.AppendText(path)) {
                FluidComponent f = entity.getComponent<FluidComponent>();
                sw.WriteLine(m_saveFluidComponent(entity.tag + "_" + f.fileName(), f));   
            }
        }
    }

    public static void saveEntity(string path, string name, Entity entity, bool clean = true) {
        string components = String.Empty;
        
        for (var i = 0; i < entity.components.Count; i++) {
            components += entity.tag + "_" + entity.components[i].fileName();
            if (i != entity.components.Count - 1) {
                components += ", ";
            }
        }

        string text = $@"object {name} {open}
    str tag = {entity.tag};
    
    f32 x = {entity.transform.position.X};
    f32 y = {entity.transform.position.Y};
    f32 z = {entity.transform.position.Z};

    f32 width = {entity.transform.scale.X};
    f32 height = {entity.transform.scale.Y};
    f32 depth = {entity.transform.scale.Z};

    f32 rx = {entity.transform.rotation.X}; // rotation x (pitch)
    f32 ry = {entity.transform.rotation.Y}; // rotation y (yaw)
    f32 rz = {entity.transform.rotation.Z}; // rotation z (roll)

    str components = {core.aengine.QUOTE}{components}{core.aengine.QUOTE};
{close}";

        if (clean) File.WriteAllText(path, text);
        else {
            using (StreamWriter sw = File.AppendText(path)) {
                sw.WriteLine(text);   
            }   
        }
    }

    private static string m_saveMeshComponent(string name, MeshComponent component) {
        string text = $@"object {name} {open}
    str type = {core.aengine.QUOTE}MeshComponent{core.aengine.QUOTE};

    i32 shape = {(int)component.shape};

    i32 r = {component.color.r};
    i32 g = {component.color.g};
    i32 b = {component.color.b};
    i32 a = {component.color.a};

    bool isModel = {component.isModel.ToString().ToLower()};

    str texture = {core.aengine.QUOTE}{component.texture.path}{core.aengine.QUOTE};
    str model = {core.aengine.QUOTE}{component.model.path}{core.aengine.QUOTE};

    str heightmap = {core.aengine.QUOTE}{component.terrainPath}{core.aengine.QUOTE};

    i32 scale = {component.scale};    
{close}";

        return text;
    }

    private static string m_saveRigidBody(string name, RigidBodyComponent component) {
        string text = $@"object {name} {open}
    str type = {core.aengine.QUOTE}RigidBodyComponent{core.aengine.QUOTE};
    
    f32 mass = {component.body.Mass};

    i32 shape = {(int)component.shapeType};
    i32 body_type = {Convert.ToInt32(component.body.IsStatic)};

    str model = {core.aengine.QUOTE}{component.model.path}{core.aengine.QUOTE};

    str heightmap = {core.aengine.QUOTE}{component.heightmap.path}{core.aengine.QUOTE};
{close}";

        return text;
    }

    private static string m_saveSoundComponent(string name, SpatialAudioComponent component) {
        string text = $@"object {name} {open}
    str type = {core.aengine.QUOTE}SpatialAudioComponent{core.aengine.QUOTE};

    str sound = {core.aengine.QUOTE}{component.sound.path}{core.aengine.QUOTE};
        
    f32 strength = {component.strength};
        
    bool can_play = {component.canPlay.ToString().ToLower()};
{close}";

        return text;
    }
    
    private static string m_saveLightComponent(string name, LightComponent component) {
        string text = $@"object {name} {open}
    string type = {core.aengine.QUOTE}LightComponent{core.aengine.QUOTE};
        
    f32 intensity = {component.intensity};

    i32 r = {component.core.color.r};
    i32 g = {component.core.color.g};
    i32 b = {component.core.color.b};
    i32 a = {component.core.color.a};

    string shader_vert = {core.aengine.QUOTE}{component.shader.vertPath}{core.aengine.QUOTE};
    string shader_frag = {core.aengine.QUOTE}{component.shader.fragPath}{core.aengine.QUOTE};

    bool enabled = {component.enabled};

    i32 light_type = {component.core.type};
{close}";

        return text;
    }
    
    private static string m_saveFluidComponent(string name, FluidComponent component) {
        string text = $@"object {name} {open}
    str type = {core.aengine.QUOTE}FluidComponent{core.aengine.QUOTE};

    str shader_vert = {core.aengine.QUOTE}{component.shader.vertPath}{core.aengine.QUOTE};
    str shader_frag = {core.aengine.QUOTE}{component.shader.fragPath}{core.aengine.QUOTE};

    str texture = {core.aengine.QUOTE}assets/water.png{core.aengine.QUOTE};

    i32 r = {component.color.r};
    i32 g = {component.color.g};
    i32 b = {component.color.b};
    i32 a = {component.color.a};

    f32 freqX = {component.freqX};
    f32 freqY = {component.freqY};
    f32 ampX = {component.ampX};
    f32 ampY = {component.ampY};
    f32 speedX = {component.speedX};
    f32 speedY = {component.speedY};
{close}";

        return text;
    }

    public static Entity loadPrefab(string path, string name, bool posZero = false, bool setDir = true) {
        if (setDir) {
            string prevDir = Directory.GetCurrentDirectory();
            // Console.WriteLine("prev dir: " + prevDir);

            ParsedData data = Parser.parse(Parser.read(path));
            ParsedObject obj = data.getObject(name);
            
            if (obj.modifier != "prefab") {
                Console.WriteLine($"Object {name} isn't a prefab");
                return null;
            }

            string newDir = String.Empty;
            string newPath = String.Empty;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
                newDir = prevDir + "/" + Path.GetDirectoryName(path).Replace("\\", "/");
            } else {
                newDir = prevDir + "\\" + Path.GetDirectoryName(path);
            }
            
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
                newPath = prevDir + "/" + path.Replace("\\", "//");
            } else {
                newPath = prevDir + "\\" + path.Replace("/", "\\");
            }
            // Console.WriteLine("new dir " + newDir);
            // Console.WriteLine("new path " + newPath);
            Directory.SetCurrentDirectory(newDir);

            string tag = obj.getValue<string>("tag");

            Entity result = new Entity(tag);

            float x = 0;
            float y = 0;
            float z = 0;

            if (!posZero) {
                x = obj.getValue<float>("x");
                y = obj.getValue<float>("y");
                z = obj.getValue<float>("z");
            }

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
            
            if (components != "") {
                char separator = ',';

                string[] componentNameArray = components.Split(separator, StringSplitOptions.TrimEntries);
                
                foreach (string s in componentNameArray) {
                    result.addComponent(m_loadComponent(result, newPath, s));
                }
            }

            Directory.SetCurrentDirectory(prevDir);
            return result;
        } else {

            ParsedData data = Parser.parse(Parser.read(path));
            ParsedObject obj = data.getObject(name);
            
            if (obj.modifier != "prefab") {
                Console.WriteLine($"Object {name} isn't a prefab");
                return null;
            }

            string tag = obj.getValue<string>("tag");

            Entity result = new Entity(tag);
            
            float x = 0;
            float y = 0;
            float z = 0;

            if (!posZero) {
                x = obj.getValue<float>("x");
                y = obj.getValue<float>("y");
                z = obj.getValue<float>("z");
            }

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
            
            if (components != "") {
                char separator = ',';

                string[] componentNameArray = components.Split(separator, StringSplitOptions.TrimEntries);
                
                foreach (string s in componentNameArray) {
                    result.addComponent(m_loadComponent(result, path, s));
                }
            }
            
            return result;
        }
        
        return null;
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

        Texture texture = new Texture();

        if (texturePath != "") {
            texture = Raylib.LoadTexture(texturePath);
        }

        MeshComponent result = null;
        
        if (shape is ShapeType.MODEL) {
            // Console.WriteLine(obj.getValue<string>("model"));
            // Console.WriteLine(Directory.GetCurrentDirectory());
            //
            result = new MeshComponent(entity,
                new aModel(obj.getValue<string>("model"), Raylib.LoadModel(obj.getValue<string>("model"))),
                color
            );
        } else if (shape is ShapeType.TERRAIN) {
            result = new MeshComponent(entity,
                new aTexture(obj.getValue<string>("heightmap"), Raylib.LoadTexture(obj.getValue<string>("heightmap"))),
                color,
                new aTexture(obj.getValue<string>("heightmap"), Raylib.LoadTexture(obj.getValue<string>("texture")))
            );
        } else {
            result = new MeshComponent(entity,
                shape,
                color
            );  
        }

        result.scale = obj.getValue<int>("scale");
        
        if (texturePath != "") result.setTexture(new aTexture(texturePath, texture));

        return result;
    }

    public static Component loadRigidBodyComponent(Entity entity, ParsedObject obj) {
        float mass = obj.getValue<float>("mass");
        ShapeType shape = (ShapeType)obj.getValue<int>("shape");
        BodyType bodyType = (BodyType)obj.getValue<int>("body_type");

        string modelPath = obj.getValue<string>("model");
        string heightmapPath = obj.getValue<string>("heightmap");

        if (modelPath != "") {
            aModel model = new aModel(modelPath);

            return new RigidBodyComponent(entity,
                model, mass, bodyType
            );
        }

        if (heightmapPath != "") {
            aTexture heightmap = new aTexture(heightmapPath);

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

        SpatialAudioComponent result = new SpatialAudioComponent(entity, new aSound(soundFile, sound));

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