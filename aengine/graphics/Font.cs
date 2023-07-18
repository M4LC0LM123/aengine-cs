using System;
using SharpFNT;

namespace aengine.graphics
{
    public class Font 
    {
        public BitmapFont bitmap;
        public Texture atlas;

        public Font(string path, string atlasPath)
        {
            this.bitmap = BitmapFont.FromFile(path);
            this.atlas = new Texture(atlasPath);
        }

        public Font(string path)
        {
            this.bitmap = BitmapFont.FromFile(path);
            string tPath = aengine.core.aengine.removeFromEnd(path, 3) + "png";
            this.atlas = new Texture(tPath);
        }

    }
}