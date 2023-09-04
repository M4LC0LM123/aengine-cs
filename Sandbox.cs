using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using aengine.graphics;
using aengine.input;
using Assimp;
using OpenTK;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL;
using Camera = aengine.graphics.Camera;
using GLFW_WINDOW = OpenTK.Windowing.GraphicsLibraryFramework.Window;
using Light = aengine.graphics.Light;
using Material = aengine.graphics.Material;
using PrimitiveType = OpenTK.Graphics.OpenGL.PrimitiveType;
using TextureWrapMode = OpenTK.Graphics.OpenGL.TextureWrapMode;
using Window = aengine.window.Window;
using A_FACE = Assimp.Face;
using Mesh = aengine.graphics.Mesh;

namespace Sandbox;

public class Sandbox {
    public static void Main(string[] args) {
        Directory.SetCurrentDirectory("../../../");
        
        Window.init(800, 600, "aengine - opengl immediate mode fps: ", 5000);
        Window.setIcon(new Texture("assets/logo.png"));

        Camera camera = new Camera(Vector3.One * 2, 90);
        bool isMouseLocked = false;

        Font font = new Font("assets/fonts/arial.fnt");
        
        Input.keyHandle += key => {
            if (Input.isKeyDown(Keys.Escape)) isMouseLocked = !isMouseLocked;
        };
        
        Texture albedo = new Texture("assets/albedo.png");

        Model model = new Model("assets/models/trench.obj");
        model.position.Y = -5;
        model.texture = albedo;
        model.scale = Vector3.One;
        
        Light light = new Light();
        light.intensity = 0.1f;
        light.position = new Vector3(5.0f, 5.0f, 0.0f);

        float speed;
        
        Fog.enable();
        Fog.create(0, Vector3.Zero, Colors.TEAL);
        
        GL.Enable(EnableCap.DepthTest); 
        GL.DepthFunc(DepthFunction.Lequal);
        
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        
        GL.Enable(EnableCap.CullFace);
        GL.CullFace(CullFaceMode.Back);
        
        while (!Window.windowShouldClose()) {
            Window.update();
            Window.setTitle("aengine - opengl immediate mode fps: " + Window.getFPS());
            if (Input.isKeyDown(Keys.LeftShift))
                speed = 50f;
            else
                speed = 10f;
            
            camera.setFirstPerson(0.1f, isMouseLocked);
            camera.setDefaultFPSControls(speed, isMouseLocked, true);
            camera.defaultFpsMatrix();
            camera.update();
            
            Window.begin();
            Window.setProjectionMatrix(camera);
            Rendering.clearBackground(Colors.TEAL);
            
            Rendering.drawDebugAxes();
            Rendering.drawGrid(100);
            
            GL.Enable(EnableCap.Lighting);
            light.enable(LightName.Light0);
            
            model.render();
            
            GL.Disable(EnableCap.Lighting);
            
            Rendering.drawRectangleTexture(albedo,5, 10, 275, 50, Colors.WHITE);
            
            Rendering.drawText(font, "Hello opengl!", 10, 10, 0.5f,
                Colors.BLACK with { g = (float)Math.Sin(Window.getTimer().Elapsed.TotalSeconds) / 2.0f + 0.5f });
            
            Window.end();
        }
        
        albedo.dispose();
        Window.dispose();
    }
}