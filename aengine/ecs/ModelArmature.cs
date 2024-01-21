using Raylib_CsLo;
using static Raylib_CsLo.Raylib;

namespace aengine.ecs; 

public class ModelArmature {
    public ModelAnimation[] animations;
    public int frameCounter = 0;
    private float speed;

    public ModelArmature(string path, float speed = 100) {
        animations = LoadModelAnimations(path);
        this.speed = speed;
    }

    public float getSpeed() {
        return speed;
    }

    public void setSpeed(float speed) {
        this.speed = speed;
    }
}