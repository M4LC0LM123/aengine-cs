using aengine.ecs;
using Raylib_CsLo;
using Sandbox.aengine.Gui;

namespace Editor; 

public class EditorComponentData {
    // _____________________ mesh component _______________________
    public static GuiTextBox MshapeBox = new GuiTextBox();      // ShapeType (int)
    
    public static GuiTextBox rBox = new GuiTextBox();           // int
    public static GuiTextBox gBox = new GuiTextBox();           // int
    public static GuiTextBox bBox = new GuiTextBox();           // int
    public static GuiTextBox aBox = new GuiTextBox();           // int

    public static GuiTextBox isModelBox = new GuiTextBox();     // bool

    public static GuiTextBox textureBox = new GuiTextBox();     // string
    public static GuiTextBox MmodelBox   = new GuiTextBox();    // string

    public static GuiTextBox MHeightmapBox = new GuiTextBox();  // string

    public static GuiTextBox scaleBox = new GuiTextBox();       // int
    // _____________________ mesh component _______________________
    
    // _____________________ rb component _________________________
    public static GuiTextBox massBox = new GuiTextBox();        // float
    
    public static GuiTextBox RshapeBox = new GuiTextBox();      // ShapeType (int)
    public static GuiTextBox bodyTypeBox = new GuiTextBox();    // BodyType  (int)
    
    public static GuiTextBox RModelBox = new GuiTextBox();      // string
    
    public static GuiTextBox RHeightmapBox = new GuiTextBox();  // string
    // _____________________ rb component _________________________

    public static Component activeComponent = null;

    public static void setComponent(Component component) {
        activeComponent = component;
        
        if (activeComponent.getType() == "MeshComponent") {
            MeshComponent mc = (MeshComponent) activeComponent;
            MshapeBox.text = ((int)mc.shape).ToString();
            rBox.text = mc.color.r.ToString();
            gBox.text = mc.color.g.ToString();
            bBox.text = mc.color.b.ToString();
            aBox.text = mc.color.a.ToString();
        }
    }
    
    public static void render(GuiWindow window) {
        if (activeComponent.getType() == "MeshComponent") {
            MeshComponent mc = (MeshComponent) activeComponent;
            
            MshapeBox.render(10, 10, 100, 25, window);
            if (MshapeBox.text != String.Empty) mc.setShape((ShapeType) int.Parse(MshapeBox.text));
            
            rBox.render(10, 45, 100, 25, window);
            gBox.render(10, 80, 100, 25, window);
            bBox.render(10, 115, 100, 25, window);
            aBox.render(10, 150, 100, 25, window);
            
            if (rBox.text != String.Empty) mc.color.r = (byte) int.Parse(rBox.text);
            if (gBox.text != String.Empty) mc.color.g = (byte) int.Parse(gBox.text);
            if (bBox.text != String.Empty) mc.color.b = (byte) int.Parse(bBox.text);
            if (aBox.text != String.Empty) mc.color.a = (byte) int.Parse(aBox.text);
        }
    }
}