using System.Numerics;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using aengine.core;
using aengine.graphics;
using Jitter;
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
        
        public static DebugRenderer debugRenderer = new DebugRenderer();

        public static void removeEntity(Entity entity)
        {
            entities.Remove(entity);
        }

        public static void update()
        {
            World.world.Step(1.0f/60.0f, false);
            
            int len = entities.Count;
            for (int i = 0; i < len; i++)
                entities[i].update();
        }

        public static void render()
        {
            int len = entities.Count;
            for (int i = 0; i < len; i++)
            {
                entities[i].render();
                
                if (entities[i].hasComponent<RigidBodyComponent>())
                {
                    entities[i].getComponent<RigidBodyComponent>().debug = RenderColliders;
                }
                else if (entities[i].hasComponent<LightComponent>())
                {
                    entities[i].getComponent<LightComponent>().debug = RenderColliders;
                } 
                else if (entities[i].hasComponent<SpatialAudioComponent>())
                {
                    entities[i].getComponent<SpatialAudioComponent>().debug = RenderColliders;
                }
            }
        }

        public static void dispose()
        {
            int len = entities.Count;
            for (int i = 0; i < len; i++)
                entities[i].dispose();
        }

    }
}