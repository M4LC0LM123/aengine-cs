using Raylib_CsLo;

namespace aengine.graphics;

public class aTexture {
    public string path;
    public Texture data;

    public aTexture() {
        path = String.Empty;
        data = new Texture();
    }

    public aTexture(string path, Texture data) {
        this.path = path;
        this.data = data;
    }
    
    public aTexture(Texture data) {
        path = String.Empty;
        this.data = data;
    }

    public aTexture(string path) {
        this.path = path;
        data = Raylib.LoadTexture(this.path);
    }

    public void dispose() {
        path = String.Empty;
        if (!data.Equals(default(Texture))) Raylib.UnloadTexture(data);
    }
}