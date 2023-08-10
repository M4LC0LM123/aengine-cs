using System.Numerics;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using aengine.core;
using aengine.graphics;
using Jitter.Collision;
using Raylib_CsLo;

namespace aengine.ecs
{
    public class World 
    {
        public static List<Entity> entities = new List<Entity>();
        public static bool RenderColliders = false;

        public static CollisionSystem collisionSystem = new CollisionSystemSAP();
        public static Jitter.World world = new Jitter.World(collisionSystem);

        public static RLights lights = new RLights();

        public static Camera camera = null;

        public static void removeEntity(Entity entity)
        {
            entities.Remove(entity);
        }

        public static void update()
        {
            World.world.Step(Raylib.GetFrameTime(), false);
            
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

                if (RenderColliders)
                {
                    if (entity.hasComponent<RigidBodyComponent>())
                    {
                        entity.getComponent<RigidBodyComponent>().debugRender();
                    }
                }
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