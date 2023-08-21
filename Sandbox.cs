using System.Numerics;
using aengine.graphics;
using aengine.input;
using Assimp;
using OpenTK;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL;
using Camera = aengine.graphics.Camera;
using GLFW_WINDOW = OpenTK.Windowing.GraphicsLibraryFramework.Window;
using PrimitiveType = OpenTK.Graphics.OpenGL.PrimitiveType;
using TextureWrapMode = OpenTK.Graphics.OpenGL.TextureWrapMode;
using Window = aengine.window.Window;

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

        AssimpContext context = new AssimpContext();
        Scene scene = context.ImportFile("assets/models/scientist.glb",
            PostProcessSteps.Triangulate | PostProcessSteps.FlipUVs);
        
        Texture albedo = new Texture("assets/albedo.png");
        
        float rotation = 0;
        float speed = 10;
        
        GL.Enable(EnableCap.Fog);
        GL.Fog(FogParameter.FogMode, (int)FogMode.Linear);
        GL.Fog(FogParameter.FogStart, 5.0f);
        GL.Fog(FogParameter.FogEnd, 100.0f);
        
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

            rotation += 100 * Window.getDeltaTime();
            
            GL.PushMatrix();
            
            GL.Enable(EnableCap.Lighting);
            GL.Enable(EnableCap.Light0); 
            GL.ShadeModel(ShadingModel.Smooth);
            
            float[] lightPosition = { 5.0f, 5.0f, 0.0f, 2.0f }; 
            float[] lightAmbient = { 2.0f, 2.0f, 2.0f, 1.0f };
            float[] lightDiffuse = { 0.4f, 0.32f, 0.2f, 0.12f };
            float[] lightSpecular = { 0.2f, 0.2f, 0.2f, 0.2f };

            GL.Light(LightName.Light0, LightParameter.Position, lightPosition);
            GL.Light(LightName.Light0, LightParameter.Ambient, lightAmbient);
            GL.Light(LightName.Light0, LightParameter.Diffuse, lightDiffuse);
            GL.Light(LightName.Light0, LightParameter.Specular, lightSpecular);
            
            float[] materialAmbient = { 0.2f, 0.2f, 0.2f, 1.0f };
            float[] materialDiffuse = { 0.8f, 0.8f, 0.8f, 1.0f };
            float[] materialSpecular = { 1.0f, 1.0f, 1.0f, 1.0f };
            float materialShininess = 0.0f;

            GL.Material(MaterialFace.Front, MaterialParameter.Ambient, materialAmbient);
            GL.Material(MaterialFace.Front, MaterialParameter.Diffuse, materialDiffuse);
            GL.Material(MaterialFace.Front, MaterialParameter.Specular, materialSpecular);
            GL.Material(MaterialFace.Front, MaterialParameter.Shininess, materialShininess);
            
            GL.Translate(0, 0, 0);
            GL.Rotate(0, 0, 0, 1);
            GL.Rotate(rotation, 0, 1, 0);
            GL.Rotate(-90, 1, 0, 0);
            GL.Scale(0.1f, 0.1f, 0.1f);
            
            GL.Enable(EnableCap.DepthTest); 
            GL.DepthFunc(DepthFunction.Lequal); 

            GL.Enable(EnableCap.Texture2D);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.BindTexture(TextureTarget.Texture2D, albedo.id);
            
            GL.Begin(PrimitiveType.Triangles);

            Vector3 uvw = Vector3.Zero;
            Vector3 normal = Vector3.Zero;
            
            foreach (var mesh in scene.Meshes) {
                for (int i = 0; i < mesh.FaceCount; i++) {
                    Face face = mesh.Faces[i];
                    if (face != null && face.HasIndices && face.IndexCount != null) {
                        for (int j = 0; j < face.IndexCount; j++) {
                            int vertexIndex = face.Indices[j];

                            normal.X = mesh.Normals[vertexIndex].X;
                            normal.Y = mesh.Normals[vertexIndex].Y;
                            normal.Z = mesh.Normals[vertexIndex].Z;
                            
                            uvw.X = mesh.TextureCoordinateChannels[0][vertexIndex].X;
                            uvw.Y = mesh.TextureCoordinateChannels[0][vertexIndex].Y;
                            uvw.Z = mesh.TextureCoordinateChannels[0][vertexIndex].Z;
                            
                            GL.Normal3(normal.X, normal.Y, normal.Z);
                            GL.TexCoord2(uvw.X, 1 - uvw.Y);
                            GL.Vertex3(mesh.Vertices[vertexIndex].X, mesh.Vertices[vertexIndex].Y, mesh.Vertices[vertexIndex].Z);
                        }
                    }
                }
            }
            
            GL.End();
            
            GL.BindTexture(TextureTarget.Texture2D, 0);
            
            GL.PopMatrix();
            
            GL.Disable(EnableCap.Lighting);
            
            Rendering.drawText(font, "Hello opengl!", 10, 10, 0.5f,
                Colors.BLACK with { g = (float)Math.Sin(Window.getTimer().Elapsed.TotalSeconds) / 2.0f + 0.5f });
            
            Window.end();
        }

        Window.dispose();
    }
}