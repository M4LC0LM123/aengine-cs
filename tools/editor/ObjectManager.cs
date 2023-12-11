using System.Numerics;
using System.Runtime.InteropServices;
using System.Text.Json;
using aengine.ecs;
using aengine.graphics;
using NativeFileDialogSharp;
using Raylib_CsLo;
using static Raylib_CsLo.Raylib;

namespace Editor;

public class ObjectManager
{
    public List<Object> objects;
    public bool outlined = false;

    public ObjectManager()
    {
        objects = new List<Object>();
    }
    
    public void addOBject(int id)
    {
        objects.Add(new Object(id));
    }

    public void addObject(int id, Vector3 position)
    {
        objects.Add(new Object(id, position));
    }

    public void removeObject(Object obj)
    {
        objects.Remove(obj);
    }

    public void update(AxieMover mover)
    {
        foreach (var ent in World.entities) {
            if (AxieMover.ACTIVE_ENT != ent)
                ent.selected = false;
            
            AxieMover.collision = GetRayCollisionBox(AxieMover.MOUSE_RAY,
                new BoundingBox(RayMath.Vector3Subtract(ent.transform.position, ent.transform.scale / 2),
                    RayMath.Vector3Add(ent.transform.position, ent.transform.scale / 2)));
            
            if (IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT))
            {
                if (AxieMover.collision.hit && AxieMover.CURRENT_MODE is Mode.ROAM)
                {
                    ent.selected = true;
                    mover.position = ent.transform.position;
                    AxieMover.ACTIVE_ENT = ent;
                }
            }

            if (IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_RIGHT))
            {
                ent.selected = false;
                AxieMover.IS_OBJ_ACTIVE = false;
                AxieMover.ACTIVE_ENT = null;
            }
            
            if (ent.selected)
            {
                AxieMover.IS_OBJ_ACTIVE = true;
                AxieMover.ACTIVE_ENT = ent;
            }
        }
        
        if (IsKeyPressed(KeyboardKey.KEY_DELETE) && AxieMover.IS_OBJ_ACTIVE)
        {
            AxieMover.IS_OBJ_ACTIVE = false;
            World.removeEntity(AxieMover.ACTIVE_ENT);
            AxieMover.ACTIVE_ENT = null;
        }
    }

    public void render()
    {
        foreach (var ent in World.entities)
        {
            Rendering.drawCubeWireframe(ent.transform.position, ent.transform.rotation, ent.transform.scale, WHITE);
        }
    }

    public void saveJson()
    {
        List<Dictionary<string, object>> jsonObjects = new List<Dictionary<string, object>>();

        foreach (var obj in objects)
        {
            jsonObjects.Add(obj.ToDictionary());
        }

        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        string json = JsonSerializer.Serialize(jsonObjects, options);

        if (Dialog.FileSave().Path != null) {
            File.WriteAllText(Dialog.FileSave().Path, json);   
        } else {
            Console.WriteLine("cancelled");  
        }
    }

    public void loadJson()
    {
        objects.Clear();

        if (Dialog.FileOpen().Path != null) {
            List<SceneObject> data = JsonSerializer.Deserialize<List<SceneObject>>(File.ReadAllText(Dialog.FileOpen().Path));
            foreach (var obj in data)
            {
                Object new_object = new Object(obj.id, new Vector3(obj.x, obj.y, obj.z));
                new_object.rotation = new Vector3(obj.rx, obj.ry, obj.rz);
                new_object.scale = new Vector3(obj.w, obj.h, obj.d);
                objects.Add(new_object);
            }   
        } else {
            Console.WriteLine("cancelled");  
        }
    }

    public void save(string name) {
        string path = Dialog.FileSave().Path.Replace("\\", "/");
        
        if (path != null || path != String.Empty) {
            Prefab.saveScene(path, name);
        } else {
            Console.WriteLine("cancelled");  
        }
    }

    public void load(string name) {
        World.entities.Clear();
        
        string prevDir = Directory.GetCurrentDirectory();
        string path = Dialog.FileOpen().Path.Replace("\\", "/");
        
        string newDir = Path.GetDirectoryName(path);
        
        Directory.SetCurrentDirectory(newDir);
        
        // Console.WriteLine(path);
        if (path != null || path != String.Empty) {
            Prefab.loadScene(path, name, false);
        } else {
            Console.WriteLine("cancelled");  
        }
        
        Directory.SetCurrentDirectory(prevDir);
    }

}