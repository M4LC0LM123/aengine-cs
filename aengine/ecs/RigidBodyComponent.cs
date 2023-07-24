using System.Numerics;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;

namespace aengine.ecs
{
    public class RigidBodyComponent : Component
    {
        public Shape shape;
        public RigidBody body;
        public BodyType type;

        public RigidBodyComponent(Entity entity, float mass = 1.0f, BodyType type = BodyType.DYNAMIC, ShapeType shape = ShapeType.BOX)
        {
            this.type = type;
            if (shape == ShapeType.BOX)
            {
                this.shape = new BoxShape(new Jitter.LinearMath.JVector(entity.transform.scale.X, entity.transform.scale.Y, entity.transform.scale.Z));
            }
            else if (shape == ShapeType.SPHERE)
            {
                this.shape = new SphereShape(entity.transform.scale.X);
            }
            else if (shape == ShapeType.CAPSULE)
            {
                this.shape = new CapsuleShape(entity.transform.scale.X, entity.transform.scale.Y);
            }
            else if (shape == ShapeType.CYLINDER)
            {
                this.shape = new CylinderShape(entity.transform.scale.X, entity.transform.scale.Y);
            }
            this.body = new RigidBody(this.shape);
            this.body.Position = new Jitter.LinearMath.JVector(entity.transform.position.X, entity.transform.position.Y, entity.transform.position.Z); 

            switch (this.type)
            {
                case BodyType.DYNAMIC:
                    this.body.IsStatic = false;
                    break;
                case BodyType.STATIC:
                    this.body.IsStatic = true;
                    break;
            }

            World.world.AddBody(this.body);
        }

        public void init(Entity entity, float mass = 1.0f, BodyType type = BodyType.DYNAMIC, ShapeType shape = ShapeType.BOX)
        {
            this.type = type;
            if (shape == ShapeType.BOX)
            {
                this.shape = new BoxShape(new Jitter.LinearMath.JVector(entity.transform.scale.X, entity.transform.scale.Y, entity.transform.scale.Z));
            }
            else if (shape == ShapeType.SPHERE)
            {
                this.shape = new SphereShape(entity.transform.scale.X);
            }
            else if (shape == ShapeType.CAPSULE)
            {
                this.shape = new CapsuleShape(entity.transform.scale.X, entity.transform.scale.Y);
            }
            else if (shape == ShapeType.CYLINDER)
            {
                this.shape = new CylinderShape(entity.transform.scale.X, entity.transform.scale.Y);
            }
            this.body = new RigidBody(this.shape);
            this.body.Position = new Jitter.LinearMath.JVector(entity.transform.position.X, entity.transform.position.Y, entity.transform.position.Z); 

            switch (this.type)
            {
                case BodyType.DYNAMIC:
                    this.body.IsStatic = false;
                    break;
                case BodyType.STATIC:
                    this.body.IsStatic = true;
                    break;
            }

            World.world.AddBody(this.body);
        }

        public void applyImpulse(float x, float y, float z)
        {
            this.body.ApplyImpulse(new Jitter.LinearMath.JVector(x,  y, z));
        }

        public void applyImpulse(Vector3 impulse)
        {
            this.body.ApplyImpulse(new Jitter.LinearMath.JVector(impulse.X, impulse.Y, impulse.Z));
        }

        public void applyForce(Vector3 force)
        {
            this.body.AddForce(new Jitter.LinearMath.JVector(force.X, force.Y, force.Z));
        }

        public void applyTorque(Vector3 torque)
        {
            this.body.AddTorque(new Jitter.LinearMath.JVector(torque.X, torque.Y, torque.Z));
        }

        public void setX(float x)
        {
            body.Position.Set(x, 0, 0);
        }
        
        public void setY(float y)
        {
            body.Position.Set(0, y, 0);
        }

        public void setZ(float z)
        {
            body.Position.Set(0, 0, z);
        }

        public void setPosition(float x, float y, float z)
        {
            body.Position.Set(x, y, z);
        }

        public void setPosition(Vector3 position)
        {
            body.Position.Set(position.X, position.Y, position.Z);
        }

        public override void update(Entity entity)
        {
            base.update(entity);

            if (entity != null && entity.transform != null && body != null)
            {
                entity.transform.position = new Vector3(body.Position.X, body.Position.Y, body.Position.Z);
                entity.transform.rotation = aengine.core.aengine.MatrixToEuler(body.Orientation);
            }
        }
    }
}