using System.Numerics;
using Raylib_CsLo;
using static Raylib_CsLo.Raylib;

namespace aengine_cs.aengine.windowing; 

public class Window {
    private static int m_width = 800;
    private static int m_height = 600;
    private static int m_renderWidth = 800;
    private static int m_renderHeight = 600;
    private static string m_title = String.Empty;
    private static uint m_configFlags = (uint)ConfigFlags.FLAG_WINDOW_RESIZABLE;
    private static TraceLogLevel m_traceLogLevel = TraceLogLevel.LOG_NONE;
    private static KeyboardKey m_exitKey = KeyboardKey.KEY_NULL;
    private static int m_targetFps = 60;
    public static Vector2 mousePosition = Vector2.Zero;
    public static RenderTexture target;

    public static void create() {
        SetConfigFlags(m_configFlags);
        SetTraceLogLevel((int)m_traceLogLevel);
        InitWindow(m_width, m_height, m_title);
        SetExitKey(m_exitKey);
        SetTargetFPS(m_targetFps);
        InitAudioDevice();

        target = LoadRenderTexture(m_renderWidth, m_renderHeight);
    }

    public static int renderWidth {
        get => m_renderWidth;
        set {
            m_renderWidth = value;
            reloadTarget();
        }
    }

    public static int renderHeight {
        get => m_renderHeight;
        set {
            m_renderHeight = value;
            reloadTarget();
        }
    }

    public static void setResolution(int w, int h) {
        m_renderWidth = w;
        m_renderHeight = h;
        
        reloadTarget();
    }

    public static int width {
        get => m_width;
        set {
            m_width = value;
            reloadWindow();
        }
    }

    public static int height {
        get => m_height;
        set {
            m_height = value;
            reloadWindow();
        }
    }

    public static void setSize(int w, int h) {
        width = w;
        height = h;
    }
    
    public static uint configFlags {
        get => m_configFlags;
        set {
            m_configFlags = value;
            SetConfigFlags(m_configFlags);
        }
    }

    public static TraceLogLevel traceLogLevel {
        get => m_traceLogLevel;
        set {
            m_traceLogLevel = value;
            SetTraceLogLevel((int)m_traceLogLevel);
        }
    }

    public static string title {
        get => m_title;
        set {
            m_title = value ?? throw new ArgumentNullException(nameof(value));
            SetWindowTitle(title);
        }
    }

    public static KeyboardKey exitKey {
        get => m_exitKey;
        set {
            m_exitKey = value;
            SetExitKey(m_exitKey);
        }
    }

    public static int targetFps {
        get => m_targetFps;
        set {
            m_targetFps = value;
            SetTargetFPS(m_targetFps);
        }
    }

    public static bool tick() {
        if (IsWindowResized()) {
            m_width = GetScreenWidth();
            m_height = GetScreenHeight();
        }
        
        mousePosition.X = GetMouseX() * ((float)renderWidth / width);
        mousePosition.Y = GetMouseY() * ((float)renderHeight / height);

        return !WindowShouldClose();
    }
    
    public static void beginRender() {
        BeginTextureMode(target);
    }

    public static void endRender() {
        EndTextureMode();
        
        BeginDrawing();
        
        DrawTexturePro(target.texture, new Rectangle(0, 0, target.texture.width, -target.texture.height), 
            new Rectangle(0, 0, GetScreenWidth(), GetScreenHeight()), Vector2.Zero, 0, WHITE);
        
        EndDrawing();
    }

    private static void reloadTarget() {
        target = LoadRenderTexture(m_renderWidth, m_renderHeight);
    }

    private static void reloadWindow() {
        SetWindowSize(m_width, m_height);
    }
    
}