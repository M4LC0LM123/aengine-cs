using System.Numerics;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Jitter.LinearMath;
using Raylib_CsLo;

namespace aengine.ecs {
    public class RigidBodyComponent : Component {
        public Shape shape;
        public RigidBody body;
        public BodyType type;
        public ShapeType shapeType;
        public bool debug = false;

        private TransformComponent m_transform;

        private Model m_collisionModel;

        public RigidBodyComponent(Entity entity, float mass = 1.0f, BodyType type = BodyType.DYNAMIC, ShapeType shape = ShapeType.BOX) {
            this.type = type;
            if (shape == ShapeType.BOX) {
                this.shape = new BoxShape(new Jitter.LinearMath.JVector(entity.transform.scale.X,
                    entity.transform.scale.Y, entity.transform.scale.Z));
            }
            else if (shape == ShapeType.SPHERE) {
                this.shape = new SphereShape(entity.transform.scale.X);
            }
            else if (shape == ShapeType.CAPSULE) {
                this.shape = new CapsuleShape(entity.transform.scale.X, entity.transform.scale.Y);
            }
            else if (shape == ShapeType.CYLINDER) {
                this.shape = new CylinderShape(entity.transform.scale.X, entity.transform.scale.Y);
            }
            else if (shape == ShapeType.CONE) {
                this.shape = new ConeShape(entity.transform.scale.Y, entity.transform.scale.X);
            }

            body = new RigidBody(this.shape);
            body.Position = new JVector(entity.transform.position.X, entity.transform.position.Y,
                entity.transform.position.Z);
            body.Orientation = JMatrix.CreateFromYawPitchRoll(entity.transform.rotation.Y * RayMath.DEG2RAD,
                entity.transform.rotation.X * RayMath.DEG2RAD, entity.transform.rotation.Z * RayMath.DEG2RAD);
            body.Mass = mass;
            shapeType = shape;

            switch (this.type) {
                case BodyType.DYNAMIC:
                    body.IsStatic = false;
                    break;
                case BodyType.STATIC:
                    body.IsStatic = true;
                    break;
            }

            m_transform = entity.transform;

            World.world.AddBody(body);
        }
        
        public unsafe RigidBodyComponent(Entity entity, Model model, float mass = 1.0f, BodyType type = BodyType.DYNAMIC) {
            this.type = type;
            
            List<JVector> vertices = new List<JVector>();

            for (int i = 0; i < model.meshCount; i++) {
                Mesh mesh = model.meshes[i];
                for (int j = 0; j < mesh.vertexCount; j += 3) {
                    float x = mesh.vertices[j];
                    float y = mesh.vertices[j + 1];
                    float z = mesh.vertices[j + 2];

                    JVector vertex = new JVector(x, y, z);
                
                    vertices.Add(vertex);
                }
            }

            shape = new ConvexHullShape(vertices);

            body = new RigidBody(shape);
            body.Position = new JVector(entity.transform.position.X, entity.transform.position.Y,
                entity.transform.position.Z);
            body.Orientation = JMatrix.CreateFromYawPitchRoll(entity.transform.rotation.Y * RayMath.DEG2RAD,
                entity.transform.rotation.X * RayMath.DEG2RAD, entity.transform.rotation.Z * RayMath.DEG2RAD);
            body.Mass = mass;
            shapeType = ShapeType.MODEL;

            switch (this.type) {
                case BodyType.DYNAMIC:
                    body.IsStatic = false;
                    break;
                case BodyType.STATIC:
                    body.IsStatic = true;
                    break;
            }

            m_transform = entity.transform;

            World.world.AddBody(body);

            m_collisionModel = model;
        }

        public void setLinearVelocity(Vector3 velocity) {
            body.LinearVelocity = new JVector(velocity.X, velocity.Y, velocity.Z);
        }

        public void setLinearVelocity(float x, float y, float z) {
            body.LinearVelocity = new JVector(x, y, z);
        }

        public void setAngularVelocity(Vector3 velocity) {
            body.AngularVelocity = new JVector(velocity.X, velocity.Y, velocity.Z);
        }

        public void setAngularVelocity(float x, float y, float z) {
            body.AngularVelocity = new JVector(x, y, z);
        }

        public void applyImpulse(float x, float y, float z) {
            this.body.ApplyImpulse(new Jitter.LinearMath.JVector(x, y, z));
        }

        public void applyImpulse(Vector3 impulse) {
            this.body.ApplyImpulse(new Jitter.LinearMath.JVector(impulse.X, impulse.Y, impulse.Z));
        }

        public void applyForce(Vector3 force) {
            this.body.AddForce(new Jitter.LinearMath.JVector(force.X, force.Y, force.Z));
        }

        public void applyForce(float x, float y, float z) {
            this.body.AddForce(new Jitter.LinearMath.JVector(x, y, z));
        }

        public void applyTorque(Vector3 torque) {
            this.body.AddTorque(new Jitter.LinearMath.JVector(torque.X, torque.Y, torque.Z));
        }

        public void applyTorque(float x, float y, float z) {
            this.body.AddTorque(new Jitter.LinearMath.JVector(x, y, z));
        }

        public void setX(float x) {
            body.Position = new JVector(x, body.Position.Y, body.Position.Z);
        }

        public void setY(float y) {
            body.Position = new JVector(body.Position.X, y, body.Position.Z);
        }

        public void setZ(float z) {
            body.Position = new JVector(body.Position.X, body.Position.Y, z);
        }

        public void setPosition(float x, float y, float z) {
            body.Position = new JVector(x, y, z);
        }

        public void setPosition(Vector3 position) {
            body.Position = new JVector(position.X, position.Y, position.Z);
        }

        public void affectedByGravity(bool affected) {
            body.AffectedByGravity = affected;
        }

        public void update(Entity entity) {
            if (entity != null && entity.transform != null && body != null) {
                if (shapeType == ShapeType.CONE)
                    entity.transform.position = new Vector3(body.Position.X,
                        body.Position.Y - entity.transform.scale.Y / 2.675f, body.Position.Z);
                else
                    entity.transform.position = new Vector3(body.Position.X, body.Position.Y, body.Position.Z);
                if (shapeType == ShapeType.CYLINDER)
                    entity.transform.rotation = new Vector3(core.aengine.MatrixToEuler(body.Orientation).X - 90,
                        core.aengine.MatrixToEuler(body.Orientation).Y - 90,
                        core.aengine.MatrixToEuler(body.Orientation).Z);
                else
                    entity.transform.rotation = core.aengine.MatrixToEuler(body.Orientation);
                m_transform = entity.transform;
            }
        }

        public void render() {
            if (debug) {
                if (shapeType == ShapeType.BOX)
                    Raylib_CsLo.Raylib.DrawCubeWires(m_transform.position, m_transform.scale.X, m_transform.scale.Y,
                        m_transform.scale.Z, Raylib_CsLo.Raylib.GREEN);
                if (shapeType == ShapeType.SPHERE)
                    Raylib_CsLo.Raylib.DrawSphereWires(m_transform.position, m_transform.scale.X, 15, 15,
                        Raylib_CsLo.Raylib.GREEN);
                if (shapeType == ShapeType.CYLINDER)
                    Raylib_CsLo.Raylib.DrawCylinderWires(m_transform.position, m_transform.scale.X, m_transform.scale.X,
                        m_transform.scale.Y, 15, Raylib_CsLo.Raylib.GREEN);
                if (shapeType == ShapeType.MODEL)
                    Raylib.DrawModelWires(m_collisionModel, m_transform.position, 1, Raylib.GREEN);
            }
        }

        public void dispose() {
            World.world.RemoveBody(body);
        }
    }
}