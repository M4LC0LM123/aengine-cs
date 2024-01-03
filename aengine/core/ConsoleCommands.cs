using System.Numerics;
using aengine_cs.aengine.windowing;
using aengine.ecs;
using aengine.graphics;
using Raylib_CsLo;

namespace aengine.core;

public class ConsoleCommands {
    public static void listCMDS(Console console, string[] args) {
        foreach (ConsoleCommand command in console.commands) {
            console.print($"{command.name} - {command.description}");
        }
    }

    public static void print(Console console, string[] args) {
        string str = console.commandInput.text.Remove(0, 6);
        console.print(str);
    }

    public static void setFPS(Console console, string[] args) {
        if (Int32.TryParse(console.commandInput.text.Remove(0, 10), out int fps)) {
            console.print($"set fps to {fps}");
            Window.targetFps = fps;
        }
        else {
            console.print(aengine.QUOTE + console.commandInput.text.Remove(0, 8) + aengine.QUOTE + "is not a number");
            console.print($"example usage of {aengine.QUOTE}w_fps_set{aengine.QUOTE}: w_fps_set 60");
        }
    }

    public static void maximizeWindow(Console console, string[] args) {
        Raylib.MaximizeWindow();
    }

    public static void minimizeWindow(Console console, string[] args) {
        Raylib.MinimizeWindow();
    }

    public static void clear(Console console, string[] args) {
        console.clear();
    }

    public static void newBody(Console console, string[] args) {
        Entity body = new Entity();
        body.transform.position = World.camera.position with { Y = World.camera.position.Y + 5 };
        body.transform.scale = Vector3.One;
        body.addComponent(new MeshComponent(body, ShapeType.SPHERE, Rendering.getRandomColor(),
            new aTexture(Raylib.LoadTextureFromImage(Raylib.GenImageColor(16, 16, Rendering.getRandomColor())))));
        body.addComponent(new RigidBodyComponent(body, 1.0f, BodyType.DYNAMIC, ShapeType.SPHERE));

        console.print("added spherical body of tag" + body.tag);
    }

    public static void setWidth(Console console, string[] args) {
        if (Int32.TryParse(console.commandInput.text.Remove(0, 7), out int width)) {
            Raylib.SetWindowSize(width, Raylib.GetScreenHeight());
            
            console.print($"set the width to {width}");
        }
        else {
            console.print(aengine.QUOTE + console.commandInput.text.Remove(0, 7) + aengine.QUOTE + "is not a number");
            console.print($"example usage of {aengine.QUOTE}w_width{aengine.QUOTE}: w_width 800");
        }
    }
    
    public static void setHeight(Console console, string[] args) {
        if (Int32.TryParse(console.commandInput.text.Remove(0, 8), out int height)) {
            Raylib.SetWindowSize(Raylib.GetScreenWidth(), height);
            
            console.print($"set the height to {height}");
        }
        else {
            console.print(aengine.QUOTE + console.commandInput.text.Remove(0, 8) + aengine.QUOTE + "is not a number");
            console.print($"example usage of {aengine.QUOTE}w_height{aengine.QUOTE}: w_height 600");
        }
    }
    
    public static void setRenderWidth(Console console, string[] args) {
        if (Int32.TryParse(console.commandInput.text.Remove(0, 15), out int width)) {
            Window.setResolution(width, Window.renderHeight);
            
            console.print($"set the width to {width}");
        }
        else {
            console.print(aengine.QUOTE + console.commandInput.text.Remove(0, 15) + aengine.QUOTE + "is not a number");
            console.print($"example usage of {aengine.QUOTE}w_render_width{aengine.QUOTE}: w_render_width 800");
        }
    }
    
    public static void setRenderHeight(Console console, string[] args) {
        if (Int32.TryParse(console.commandInput.text.Remove(0, 16), out int height)) {
            Window.setResolution(Window.renderWidth, height);
            
            console.print($"set the height to {height}");
        }
        else {
            console.print(aengine.QUOTE + console.commandInput.text.Remove(0, 16) + aengine.QUOTE + "is not a number");
            console.print($"example usage of {aengine.QUOTE}w_render_height{aengine.QUOTE}: w_render_height 600");
        }
    }

    public static void close(Console console, string[] args) {
        World.dispose();
        Raylib.CloseAudioDevice();
        Raylib.CloseWindow();
        Environment.Exit(0);
    }
    
}