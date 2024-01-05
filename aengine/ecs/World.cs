using System.Numerics;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using aengine.core;
using aengine.graphics;
using Jitter2;
using Jitter2.Collision;
using Raylib_CsLo;
using Console = System.Console;

namespace aengine.ecs
{
    public class World 
    {
        public static Dictionary<string, Entity> entities = new Dictionary<string, Entity>();
        public static bool debugRenderTerrain = false; // uses a lot of resources and makes the game slower
        public static bool renderColliders = false;
        
        public static Jitter2.World world = new Jitter2.World();

        public static RLights lights = new RLights();

        public static Camera camera = null;
        
        public static DebugRenderer debugRenderer = new DebugRenderer();
        public static bool usePhysics = true;

        private static float fixedTimeStep = 1.0f / 60.0f;
        private static float accumulator = 0.0f;

        public static void removeEntity(Entity entity)  {
            entities.Remove(entity.tag);
        }
        
        public static void removeEntity(string tag)  {
            entities.Remove(tag);
        }

        public static void update(bool physics = true, bool multithread = false) {
            fixedUpdate(physics, multithread);

            foreach (var entity in entities.Values) {
                entity.update();
            }
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

        public static void render() {
            foreach (var entity in entities.Values) {
                entity.render();

                if (entity.hasComponent<RigidBodyComponent>()) {
                    entity.getComponent<RigidBodyComponent>().debug = renderColliders;
                }
                else if (entity.hasComponent<LightComponent>()) {
                    entity.getComponent<LightComponent>().debug = renderColliders;
                }
                else if (entity.hasComponent<SpatialAudioComponent>()) {
                    entity.getComponent<SpatialAudioComponent>().debug = renderColliders;
                }
            }
        }

        public static Entity getEntity(string tag) {
            if (entities.TryGetValue(tag, out Entity result)) {
                return result;
            }

            return null;
        }

        public static bool hasTag(string tag) {
            return entities.ContainsKey(tag);
        }

        public static void dispose()  {
            foreach (var entity in entities.Values) {
                entity.dispose();
            }
        }

    }
}