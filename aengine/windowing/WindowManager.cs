using Raylib_CsLo;
using static Raylib_CsLo.Raylib;

namespace aengine_cs.aengine.windowing; 

public class WindowManager {
    private static int m_width = 800;
    private static int m_height = 600;
    private static int m_renderWidth = 800;
    private static int m_renderHeight = 600;
    private static string m_title = String.Empty;
    private static uint m_configFlags = (uint)ConfigFlags.FLAG_WINDOW_RESIZABLE;
    private static TraceLogLevel m_traceLogLevel = TraceLogLevel.LOG_NONE;
    private static KeyboardKey m_exitKey = KeyboardKey.KEY_NULL;
    private static int m_targetFps = 60;
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
        set => m_renderWidth = value;
    }

    public static int renderHeight {
        get => m_renderHeight;
        set => m_renderHeight = value;
    }

    public static void setResolution(int w, int h) {
        m_renderWidth = w;
        m_renderHeight = h;
    }
}