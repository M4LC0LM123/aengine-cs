using Raylib_CsLo;

namespace aengine.graphics; 

public class aSound {
    public string path;
    public Sound data;

    public aSound() {
        path = String.Empty;
        data = new Sound();
    }

    public aSound(string path, Sound data) {
        this.path = path;
        this.data = data;
    }
    
    public aSound(Sound data) {
        path = String.Empty;
        this.data = data;
    }

    public aSound(string path) {
        this.path = path;
        data = Raylib.LoadSound(this.path);
    }

    public void dispose() {
        path = String.Empty;
        Raylib.UnloadSound(data);
    }
}