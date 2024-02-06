using System.Diagnostics;

namespace aengine_cs.aengine.misc; 

public static class Time {
    private static Stopwatch m_stopwatch = new Stopwatch();

    public static void init() {
        m_stopwatch.Start();
    }

    public static bool every() {
        return false;
    }
}