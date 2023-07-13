namespace aengine.ecs
{
    public enum ShapeType
    {
        BOX = 0,
        SPHERE = 1,
        CYLINDER = 2, // can be a cone based on transform scale
        CAPSULE = 3,
        TRIANGLE = 4,
        MESH = 5
    }
}