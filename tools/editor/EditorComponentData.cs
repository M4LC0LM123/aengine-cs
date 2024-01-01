using aengine.ecs;
using aengine.graphics;
using Jitter.Collision.Shapes;
using NativeFileDialogSharp;
using Raylib_CsLo;
using Sandbox.aengine.Gui;

namespace Editor;

public class EditorComponentData {
    // _____________________ mesh component _______________________
    public static GuiTextBox MshapeBox = new GuiTextBox(); // ShapeType (int)

    public static GuiTextBox rBox = new GuiTextBox(); // int
    public static GuiTextBox gBox = new GuiTextBox(); // int
    public static GuiTextBox bBox = new GuiTextBox(); // int
    public static GuiTextBox aBox = new GuiTextBox(); // int

    public static GuiTextBox isModelBox = new GuiTextBox(); // bool
    // _____________________ mesh component _______________________

    // _____________________ rb component _________________________
    public static GuiTextBox massBox = new GuiTextBox(); // float

    public static GuiTextBox RshapeBox = new GuiTextBox(); // ShapeType (int)
    // _____________________ rb component _________________________

    public static Component activeComponent = null;

    public static void setComponent(Component component) {
        activeComponent = component;

        if (activeComponent.getType() == "MeshComponent") {
            MeshComponent mc = (MeshComponent)activeComponent;
            MshapeBox.text = ((int)mc.shape).ToString();
            rBox.text = mc.color.r.ToString();
            gBox.text = mc.color.g.ToString();
            bBox.text = mc.color.b.ToString();
            aBox.text = mc.color.a.ToString();
        } else if (activeComponent.getType() == "RigidBodyComponent") {
            RigidBodyComponent rb = (RigidBodyComponent)activeComponent;
            massBox.text = rb.body.Mass.ToString();
            RshapeBox.text = ((int)rb.shapeType).ToString();
        }
    }

    public static void render(GuiWindow window) {
        if (activeComponent == null)
            return;
        
        if (activeComponent.getType() == "MeshComponent") {
            MeshComponent mc = (MeshComponent)activeComponent;

            MshapeBox.render(10, 10, 50, 25, window);
            Gui.GuiTextPro(Gui.font, "shape", 75, 10, 20, Raylib.WHITE, window);
            if (MshapeBox.text != String.Empty) mc.setShape((ShapeType)int.Parse(MshapeBox.text));

            rBox.render(10, 45, 50, 25, window);
            gBox.render(65, 45, 50, 25, window);
            bBox.render(120, 45, 50, 25, window);
            aBox.render(175, 45, 50, 25, window);
            Gui.GuiTextPro(Gui.font, "color", 235, 45, 20, Raylib.WHITE, window);

            if (rBox.text != String.Empty) mc.color.r = (byte)int.Parse(rBox.text);
            if (gBox.text != String.Empty) mc.color.g = (byte)int.Parse(gBox.text);
            if (bBox.text != String.Empty) mc.color.b = (byte)int.Parse(bBox.text);
            if (aBox.text != String.Empty) mc.color.a = (byte)int.Parse(aBox.text);

            isModelBox.render(10, 80, 60, 25, window);
            Gui.GuiTextPro(Gui.font, "is model", 85, 80, 20, Raylib.WHITE, window);

            if (isModelBox.text == "true" || isModelBox.text == "false") mc.isModel = bool.Parse(isModelBox.text);

            if (Gui.GuiButton("Load texture", 10, 115, 150, 25, window)) {
                DialogResult result = Dialog.FileOpen();

                if (result.IsOk) {
                    string path = String.Empty;
                    if (result.Path != null) path = result.Path.Replace("\\", "/");
                    
                    // loads relative to the chosen scene file
                    string loadPath = getRelativePath(path,
                        Dialog.FolderPicker().Path.Replace("\\", "/"));
                    // Console.WriteLine(loadPath);

                    mc.setTexture(new aTexture(loadPath, Raylib.LoadTexture(path)));
                } else {
                    Console.WriteLine("cancelled");  
                }
            }
            Gui.GuiTexture(165, 115, 25, 25, mc.texture, window);
            
            if (Gui.GuiButton("Load model", 10, 150, 150, 25, window)) {
                DialogResult result = Dialog.FileOpen();

                if (result.IsOk) {
                    string path = String.Empty;
                    if (result.Path != null) path = result.Path.Replace("\\", "/");
                    
                    // loads relative to the chosen scene file
                    string loadPath = getRelativePath(path,
                        Dialog.FolderPicker().Path.Replace("\\", "/"));
                    // Console.WriteLine(loadPath);

                    MshapeBox.text = "6";
                    mc.setModel(new aModel(loadPath, Raylib.LoadModel(path)));
                } else {
                    Console.WriteLine("cancelled");  
                }
            }
            
            if (Gui.GuiButton("Load terrain", 10, 185, 150, 25, window)) {
                DialogResult result = Dialog.FileOpen();

                if (result.IsOk) {
                    string path = String.Empty;
                    if (result.Path != null) path = result.Path.Replace("\\", "/");
                    
                    // loads relative to the chosen scene file
                    string loadPath = getRelativePath(path,
                        Dialog.FolderPicker().Path.Replace("\\", "/"));
                    // Console.WriteLine(loadPath);

                    MshapeBox.text = "5";
                    mc.terrainPath = loadPath;
                    mc.setTerrain(new aTexture(loadPath, Raylib.LoadTexture(path)));
                } else {
                    Console.WriteLine("cancelled");  
                }
            }
        } else if (activeComponent.getType() == "RigidBodyComponent") {
            RigidBodyComponent rb = (RigidBodyComponent)activeComponent;

            massBox.render(10, 10, 50, 25, window);
            Gui.GuiTextPro(Gui.font, "mass", 75, 10, 20, Raylib.WHITE, window);
            if (massBox.text != String.Empty)
                rb.body.SetMassProperties(rb.body.Inertia, float.Parse(massBox.text), false);
            
            RshapeBox.render(10, 45, 50, 25, window);
            Gui.GuiTextPro(Gui.font, "shape", 75, 45, 20, Raylib.WHITE, window);
            if (RshapeBox.text != String.Empty) {
                rb.setShape((ShapeType)int.Parse(RshapeBox.text));
            }
            
            Gui.GuiTextPro(Gui.font, "is static", 75, 80, 20, Raylib.WHITE, window);
            rb.body.IsStatic = Gui.GuiTickBox(rb.body.IsStatic, 10, 80, 25, 25, window);
            
            if (Gui.GuiButton("Load model", 10, 125, 150, 25, window)) {
                DialogResult result = Dialog.FileOpen();

                if (result.IsOk) {
                    string path = String.Empty;
                    if (result.Path != null) path = result.Path.Replace("\\", "/");
                    
                    // loads relative to the chosen scene file
                    string loadPath = getRelativePath(path,
                        Dialog.FolderPicker().Path.Replace("\\", "/"));
                    // Console.WriteLine(loadPath);

                    RshapeBox.text = "6";
                    rb.setModelShape(new aModel(loadPath, Raylib.LoadModel(path)));
                } else {
                    Console.WriteLine("cancelled");  
                }
            }
            
            if (Gui.GuiButton("Load terrain", 10, 160, 150, 25, window)) {
                DialogResult result = Dialog.FileOpen();

                if (result.IsOk) {
                    string path = String.Empty;
                    if (result.Path != null) path = result.Path.Replace("\\", "/");
                    
                    // loads relative to the chosen scene file
                    string loadPath = getRelativePath(path,
                        Dialog.FolderPicker().Path.Replace("\\", "/"));
                    // Console.WriteLine(loadPath);

                    RshapeBox.text = "5";
                    rb.setTerrainShape(new aTexture(loadPath, Raylib.LoadTexture(path)));
                } else {
                    Console.WriteLine("cancelled");  
                }
            }
        }
    }

    public static string getRelativePath(string filePath, string currentDirectory) {
        Uri fileUri = new Uri(filePath);
        Uri directoryUri = new Uri(currentDirectory + "/");

        Uri relativeUri = directoryUri.MakeRelativeUri(fileUri);

        return Uri.UnescapeDataString(relativeUri.ToString());
    }
}