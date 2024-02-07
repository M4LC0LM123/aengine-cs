using static Raylib_CsLo.Raylib;

namespace aengine_cs.aengine.misc; 

public static class Time {
    private static double prevTime = 0.0f;
    
    // do something at an interval in seconds
    public static bool interval(float interval) {
        double currTime = GetTime();

        if (currTime - prevTime >= interval) {
            prevTime = currTime;
            return true;
        }

        return false;
    }
}