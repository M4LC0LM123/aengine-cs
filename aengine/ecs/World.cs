using System.Numerics;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using aengine_cs.aengine.windowing;
using aengine.core;
using aengine.graphics;
using Jitter;
using Jitter.Collision;
using Raylib_CsLo;
using Console = System.Console;

namespace aengine.ecs {
    public class World {
        public static Dictionary<string, Entity> entities = new Dictionary<string, Entity>();
        public static List<Entity> renderable = new List<Entity>();
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

        public static void removeEntity(Entity entity) {
            entities.Remove(entity.tag);
        }

        public static void removeEntity(string tag) {
            entities.Remove(tag);
        }

        public static void update(bool physics = true, bool multithread = false) {
            fixedUpdate(physics, multithread);

            foreach (var (tag, entity) in entities) {
                entity.tag = tag;
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

        private static OpticalTransparency getOpticalTransparency(Entity entity) {
            if (entity.hasComponent<MeshComponent>()) {
                return entity.getComponent<MeshComponent>().opticalTransparency;
            }

            if (entity.hasComponent<FluidComponent>()) {
                return entity.getComponent<FluidComponent>().opticalTransparency;
            }

            if (entity.GetType() == typeof(ParticleSystem)) {
                ParticleSystem aps = (ParticleSystem)entity;
                return aps.opticalTransparency;
            }

            return OpticalTransparency.OPAQUE;
        }

        private static int compare(Entity a, Entity b) {
            if (a == null && b == null) return 0;
            if (a == null) return -1;
            if (b == null) return 1;

            // Determine optical transparency of entities
            OpticalTransparency transparencyA = getOpticalTransparency(a);
            OpticalTransparency transparencyB = getOpticalTransparency(b);

            // Compare transparency levels
            if (transparencyA != transparencyB) {
                // Different transparency levels, sort based on transparency
                return transparencyA.CompareTo(transparencyB);
            }
            
            float distanceA = Vector3.Distance(a.transform.position, camera.position);
            float distanceB = Vector3.Distance(b.transform.position, camera.position);
            return distanceB.CompareTo(distanceA);
        }

        public static void render() {
            if (Window.sortTransparentEntities)
                renderable.Sort((a, b) => {
                    int comp = compare(a, b);
                    if (comp != null) return comp;

                    return 0;
                });

            foreach (var entity in renderable) {
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

        public static void dispose() {
            foreach (var entity in entities.Values) {
                entity.dispose();
            }
        }
    }
}