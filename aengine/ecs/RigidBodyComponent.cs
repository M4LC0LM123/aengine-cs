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

namespace aengine.ecs
{
    public class RigidBodyComponent : Component
    {
        public int bodyIndex;
        public BodyType type;

        public RigidBodyComponent(Entity entity)
        {

        }

        public void init(Entity entity, float mass = 1.0f, BodyType type = BodyType.DYNAMIC, ShapeType shape = ShapeType.BOX)
        {
            this.type = type;
            switch (this.type)
            {
                case BodyType.DYNAMIC:
                    shapeCheckDyn(entity, mass, shape);
                    break;
                case BodyType.STATIC:
                    shapeCheckStat(entity, shape); 
                    break;
            }
        }

        private void shapeCheckStat(Entity entity, ShapeType shape)
        {
            if (shape == ShapeType.BOX)
            {
                World.simulation.Statics.Add(new StaticDescription(entity.transform.position, new Quaternion(entity.transform.rotation, 1), World.simulation.Shapes.Add(new Box(entity.transform.scale.X, entity.transform.scale.Y, entity.transform.scale.Z))));
            }
            else if (shape == ShapeType.SPHERE)
            {
                World.simulation.Statics.Add(new StaticDescription(entity.transform.position, new Quaternion(entity.transform.rotation, 1), World.simulation.Shapes.Add(new Sphere(entity.transform.scale.X))));
            }
            else if (shape == ShapeType.CAPSULE)
            {
                World.simulation.Statics.Add(new StaticDescription(entity.transform.position, new Quaternion(entity.transform.rotation, 1), World.simulation.Shapes.Add(new Capsule(entity.transform.scale.X, entity.transform.scale.Y))));
            }
            else if (shape == ShapeType.CYLINDER)
            {
                World.simulation.Statics.Add(new StaticDescription(entity.transform.position, new Quaternion(entity.transform.rotation, 1), World.simulation.Shapes.Add(new Cylinder(entity.transform.scale.X, entity.transform.scale.Y))));
            }
        }

        private void shapeCheckDyn(Entity entity, float mass, ShapeType shape)
        {
            if (shape == ShapeType.BOX)
            {
                var box = new Box(entity.transform.scale.X, entity.transform.scale.Y, entity.transform.scale.Z);
                var inertia = box.ComputeInertia(mass);
                var index = World.simulation.Shapes.Add(box);
                this.bodyIndex = index.Index;
                var desc = BodyDescription.CreateDynamic(entity.transform.position, inertia, index, 0.01f);
                World.simulation.Bodies.Add(desc);
            }
            else if (shape == ShapeType.SPHERE)
            {
                var sphere = new Sphere(entity.transform.scale.X);
                var inertia = sphere.ComputeInertia(mass);
                var index = World.simulation.Shapes.Add(sphere);
                this.bodyIndex = index.Index;
                var desc = BodyDescription.CreateDynamic(entity.transform.position, inertia, index, 0.01f);
                World.simulation.Bodies.Add(desc);
            }
            else if (shape == ShapeType.CAPSULE)
            {
                var capsule = new Capsule(entity.transform.scale.X, entity.transform.scale.Y);
                var inertia = capsule.ComputeInertia(mass);
                var index = World.simulation.Shapes.Add(capsule);
                this.bodyIndex = index.Index;
                var desc = BodyDescription.CreateDynamic(entity.transform.position, inertia, index, 0.01f);
                World.simulation.Bodies.Add(desc);
            }
            else if (shape == ShapeType.CYLINDER)
            {
                var cyl = new Cylinder(entity.transform.scale.X, entity.transform.scale.Y);
                var inertia = cyl.ComputeInertia(mass);
                var index = World.simulation.Shapes.Add(cyl);
                this.bodyIndex = index.Index;
                var desc = BodyDescription.CreateDynamic(entity.transform.position, inertia, index, 0.01f);
                World.simulation.Bodies.Add(desc);
            }
        }

        public override void update(Entity entity)
        {
            base.update(entity);

            if (this.type == BodyType.DYNAMIC)
            {
                BodyHandle handle = World.simulation.Bodies.ActiveSet.IndexToHandle[this.bodyIndex];
                var body = World.simulation.Bodies.GetBodyReference(handle);
                entity.transform.position = body.Pose.Position;
                entity.transform.rotation = aengine.core.aengine.QuaternionToEulerAngles(body.Pose.Orientation, true);
            }
        }
    }
}