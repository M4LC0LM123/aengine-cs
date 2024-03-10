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
    public bool outlined = false;

    public void update(TransformGizmo gizmo)
    {
        foreach (var ent in World.entities.Values) {
            if (TransformGizmo.ACTIVE_ENT != ent) {
                ent.selected = false;
            }
            
            TransformGizmo.collision = GetRayCollisionBox(TransformGizmo.MOUSE_RAY,
                new BoundingBox(RayMath.Vector3Subtract(ent.transform.position, ent.transform.scale / 2),
                    RayMath.Vector3Add(ent.transform.position, ent.transform.scale / 2)));
            
            if (IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT) && !Gui.isMouseOver())
            {
                if (TransformGizmo.collision.hit && TransformGizmo.CURRENT_MODE is Mode.ROAM)
                {
                    ent.selected = true;
                    gizmo.position = ent.transform.position;
                    TransformGizmo.ACTIVE_ENT = ent;
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
                TransformGizmo.IS_OBJ_ACTIVE = false;
                TransformGizmo.ACTIVE_ENT = null;
            }
            
            if (ent.selected)
            {
                TransformGizmo.IS_OBJ_ACTIVE = true;
                TransformGizmo.ACTIVE_ENT = ent;
            }
        }
        
        if (IsKeyPressed(KeyboardKey.KEY_DELETE) && TransformGizmo.IS_OBJ_ACTIVE)
        {
            TransformGizmo.IS_OBJ_ACTIVE = false;
            World.removeEntity(TransformGizmo.ACTIVE_ENT);
            TransformGizmo.ACTIVE_ENT = null;
        }
    }

    public void render()
    {
        foreach (var ent in World.entities.Values) {
            Rendering.drawCubeWireframe(ent.transform.position, -ent.transform.rotation, ent.transform.scale, WHITE);
            if (ent.hasComponent<RigidBodyComponent>()) {
                RigidBodyComponent rb = ent.getComponent<RigidBodyComponent>();
                rb.setPosition(ent.transform.position);
                rb.setRotation(ent.transform.rotation);
            }
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