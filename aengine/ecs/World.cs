using System.Numerics;
using System;
﻿using BepuPhysics;
using BepuPhysics.Collidables;
using BepuPhysics.CollisionDetection;
using BepuPhysics.Constraints;
using BepuUtilities;
using BepuUtilities.Memory;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using aengine.core;

namespace aengine.ecs
{
    public class World 
    {
        public static List<Entity> entities = new List<Entity>();
        public static float GRAVITY = 10;
        public static Simulation simulation = Simulation.Create(new BufferPool(), new NarrowPhaseCallbacks(), new PoseIntegratorCallbacks(new Vector3(0, -GRAVITY, 0)), new SolveDescription(8, 1));
        public static ThreadDispatcher threadDispatcher = new ThreadDispatcher(Environment.ProcessorCount);

        public static void update()
        {
            simulation.Timestep(1.0f / 60.0f, World.threadDispatcher);
            
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
            World.simulation.BufferPool.Clear();
            World.simulation.Dispose();
            threadDispatcher.Dispose();
            foreach (Entity entity in World.entities)
            {
                entity.dispose();
            }
        }

    }
}