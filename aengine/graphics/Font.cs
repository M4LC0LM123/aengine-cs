using SharpFNT;
using static aengine.aengine;

namespace aengine.graphics; 

public class Font {
    public BitmapFont bitmap;
    public Texture atlas;

    public Font(string path, string atlasPath) {
        bitmap = BitmapFont.FromFile(path);
        atlas = new Texture(atlasPath);
    }

    public Font(string path) {
        bitmap = BitmapFont.FromFile(path);
        string tPath = removeFromEnd(path, 3) + "png";
        atlas = new Texture(tPath);
    }
}