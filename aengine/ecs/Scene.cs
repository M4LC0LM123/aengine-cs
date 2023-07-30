using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace aengine.ecs
{
    public class Scene
    {
        public List<SceneObject> data;

        public Scene(string path)
        {
            data = JsonSerializer.Deserialize<List<SceneObject>>(File.ReadAllText(path));
        }
    }
}