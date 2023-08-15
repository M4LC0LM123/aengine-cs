using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace aengine.ecs
{
    public class ScenePrefab
    {
        public List<SceneObject> data;

        public ScenePrefab(string path)
        {
            data = JsonSerializer.Deserialize<List<SceneObject>>(File.ReadAllText(path));
        }
    }
}