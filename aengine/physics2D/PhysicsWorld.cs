namespace aengine.physics2D; 

public sealed class PhysicsWorld {
    public static readonly float minBodySize = 0.01f * 0.01f;
    public static readonly float maxBodySize = 1_000_000;

    public static readonly float minDensity = 0.25f; // g/cm^3
    public static readonly float maxDensity = 21.4f;
}