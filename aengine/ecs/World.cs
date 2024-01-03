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
using Console = System.Console;

namespace aengine.ecs
{
    public class World 
    {
        public static List<Entity> entities = new List<Entity>();
        public static bool debugRenderTerrain = false; // uses a lot of resources and makes the game slower
        public static bool renderColliders = false;

        public static CollisionSystem collisionSystem = new CollisionSystemSAP();
        public static Jitter.World world = new Jitter.World(collisionSystem);

        public static RLights lights = new RLights();

        public static Camera camera = null;
        
        public static DebugRenderer debugRenderer = new DebugRenderer();
        public static bool usePhysics = true;

        private static float fixedTimeStep = 1.0f / 60.0f;
        private static float accumulator = 0.0f;

        public static void removeEntity(Entity entity)
        {
            entities.Remove(entity);
        }

        public static void update(bool physics = true, bool multithread = false) {
            fixedUpdate(physics, multithread);
            
            int len = entities.Count;
            for (int i = 0; i < len; i++)
                entities[i].update();
        }

        private static void fixedUpdate(bool physics = true, bool multithread = false) {
            float deltaTime = Raylib.GetFrameTime();
            
            accumulator += deltaTime;
            
            while (accumulator >= fixedTimeStep) {
                world.Step(fixedTimeStep, multithread);
                
                accumulator -= fixedTimeStep;
            }

            usePhysics = physics;
        }

        public static void render()
        {
            int len = entities.Count;
            for (int i = 0; i < len; i++)
            {
                entities[i].render();
                
                if (entities[i].hasComponent<RigidBodyComponent>())
                {
                    entities[i].getComponent<RigidBodyComponent>().debug = renderColliders;
                }
                else if (entities[i].hasComponent<LightComponent>())
                {
                    entities[i].getComponent<LightComponent>().debug = renderColliders;
                } 
                else if (entities[i].hasComponent<SpatialAudioComponent>())
                {
                    entities[i].getComponent<SpatialAudioComponent>().debug = renderColliders;
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