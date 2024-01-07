using System.Numerics;
using System.Runtime.InteropServices;
using System.Text.Json;
using aengine_cs.aengine.windowing;
using aengine.ecs;
using aengine.graphics;
using NativeFileDialogSharp;
using Raylib_CsLo;
using Sandbox.aengine.Gui;
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
        foreach (var ent in World.entities.Values) {
            if (AxieMover.ACTIVE_ENT != ent)
                ent.selected = false;
            
            AxieMover.collision = GetRayCollisionBox(AxieMover.MOUSE_RAY,
                new BoundingBox(RayMath.Vector3Subtract(ent.transform.position, ent.transform.scale / 2),
                    RayMath.Vector3Add(ent.transform.position, ent.transform.scale / 2)));
            
            if (IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT) && !Gui.isMouseOver())
            {
                if (AxieMover.collision.hit && AxieMover.CURRENT_MODE is Mode.ROAM)
                {
                    ent.selected = true;
                    mover.position = ent.transform.position;
                    AxieMover.ACTIVE_ENT = ent;
                    Editor.xPos.text = MathF.Round(ent.transform.position.X, 2).ToString();
                    Editor.yPos.text = MathF.Round(ent.transform.position.Y, 2).ToString();
                    Editor.zPos.text = MathF.Round(ent.transform.position.Z, 2).ToString();
                    
                    Editor.xScale.text = MathF.Round(ent.transform.scale.X, 2).ToString();
                    Editor.yScale.text = MathF.Round(ent.transform.scale.Y, 2).ToString();
                    Editor.zScale.text = MathF.Round(ent.transform.scale.Z, 2).ToString();
                    
                    Editor.xRot.text = MathF.Round(ent.transform.rotation.X, 2).ToString();
                    Editor.yRot.text = MathF.Round(ent.transform.rotation.Y, 2).ToString();
                    Editor.zRot.text = MathF.Round(ent.transform.rotation.Z, 2).ToString();
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
        foreach (var ent in World.entities.Values) {
            Rendering.drawCubeWireframe(ent.transform.position, ent.transform.rotation, ent.transform.scale, WHITE);
            if (ent.hasComponent<RigidBodyComponent>()) {
                RigidBodyComponent rb = ent.getComponent<RigidBodyComponent>();
                rb.setPosition(ent.transform.position);
                rb.setRotation(ent.transform.rotation);
            }
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
        DialogResult result = Dialog.FileOpen();
        string path = String.Empty;

        if (result.IsOk) {
            if (result.Path != null) path = result.Path.Replace("\\", "/");
        
            if (path != null || path != String.Empty) {
                Prefab.saveScene(path, name);
            }
        } else {
            Console.WriteLine("cancelled");  
        }
    }

    public void load(string name) {
        World.entities.Clear();
        
        string prevDir = String.Empty;
        if (Directory.GetCurrentDirectory() != null) prevDir = Directory.GetCurrentDirectory();

        DialogResult result = Dialog.FileOpen();
        string path = String.Empty;
        if (result.IsOk) {
            if (result.Path != null) path = result.Path.Replace("\\", "/");
            
            string newDir = String.Empty;
            if (path != null) newDir = Path.GetDirectoryName(path);
        
            if (newDir != null) Directory.SetCurrentDirectory(newDir);
        
            // Console.WriteLine(path);
            if (path != null || path != String.Empty) {
                Prefab.loadScene(path, name, false);
            }
        } else {
            Console.WriteLine("cancelled");  
        }
        
        Directory.SetCurrentDirectory(prevDir);
    }

}