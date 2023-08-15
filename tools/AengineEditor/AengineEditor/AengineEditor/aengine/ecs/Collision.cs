using Raylib_CsLo;

namespace aengine.ecs;

public class Collision
{
    public static bool collision(BoundingBox box, BoundingBox other)
    {
        if (box.max.X < other.min.X || box.min.X > other.max.X) return false;
        if (box.max.Y < other.min.Y || box.min.Y > other.max.Y) return false;
        if (box.max.Z < other.min.Z || box.min.Z > other.max.Z) return false;
        return true;
    }
}