using System.Numerics;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using aengine.core;
using Jitter.Collision;

namespace aengine.ecs
{
    public class World 
    {
        public static List<Entity> entities = new List<Entity>();
        public static float GRAVITY = 9.81f;

        public static CollisionSystem collisionSystem = new CollisionSystemSAP();
        public static Jitter.World world = new Jitter.World(collisionSystem);

        public static void update()
        {
            World.world.Step(graphics.Graphics.getDeltaTime(), false);
            foreach (Entity entity in World.entities)
            {
                entity.update();
            }
        }

        public static void render()
        {
            foreach (Entity entity in World.entities)
            {
                entity.render();
            }
        }

        public static void dispose()
        {
            foreach (Entity entity in World.entities)
            {
                entity.dispose();
            }
        }

    }
}