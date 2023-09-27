using System.Numerics;
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
        foreach (var obj in objects)
        {
            obj.update();
            
            AxieMover.collision = GetRayCollisionBox(AxieMover.MOUSE_RAY,
                new BoundingBox(RayMath.Vector3Subtract(obj.position, obj.scale / 2),
                    RayMath.Vector3Add(obj.position, obj.scale / 2)));
            
            if (IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT))
            {
                if (AxieMover.collision.hit)
                {
                    obj.selected = true;
                    mover.position = obj.position;
                    AxieMover.ACTIVE_OBJ = obj;
                }
            }

            if (IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT) && IsKeyDown(KeyboardKey.KEY_LEFT_CONTROL))
            {
                if (!AxieMover.collision.hit)
                {
                    obj.selected = false;
                    AxieMover.IS_OBJ_ACTIVE = false;
                    AxieMover.ACTIVE_OBJ = null;
                }
            }
            
            if (obj.selected)
            {
                AxieMover.IS_OBJ_ACTIVE = true;
                AxieMover.ACTIVE_OBJ = obj;
            }
        }
        
        if (IsKeyPressed(KeyboardKey.KEY_BACKSPACE) && AxieMover.IS_OBJ_ACTIVE)
        {
            AxieMover.IS_OBJ_ACTIVE = false;
            removeObject(AxieMover.ACTIVE_OBJ);
            AxieMover.ACTIVE_OBJ = null;
        }
    }

    public void render()
    {
        foreach (var obj in objects)
        {
            obj.render(outlined);
        }
    }

    public void save()
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

    public void load()
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

}