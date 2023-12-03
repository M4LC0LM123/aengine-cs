using Raylib_CsLo;

namespace aengine.graphics; 

public class aModel {
    public string path;
    public Model data;

    public aModel() {
        path = String.Empty;
        data = new Model();
    }

    public aModel(string path, Model data) {
        this.path = path;
        this.data = data;
    }

    public aModel(Model data) {
        path = String.Empty;
        this.data = data;
    }

    public aModel(string path) {
        this.path = path;
        data = Raylib.LoadModel(this.path);
    }

    public void dispose() {
        path = String.Empty;
        if (!data.Equals(default(Model))) Raylib.UnloadModel(data);
    }
}