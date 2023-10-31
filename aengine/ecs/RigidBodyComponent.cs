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
using static aengine.core.aengine;
using static Raylib_CsLo.RlGl;

namespace aengine.ecs {
    public class RigidBodyComponent : Component {
        public Shape shape;
        public RigidBody body;
        public BodyType type;
        public ShapeType shapeType;
        public bool debug = false;

        private TransformComponent m_transform;

        public RigidBodyComponent(Entity entity, float mass = 1.0f, BodyType type = BodyType.DYNAMIC, ShapeType shape = ShapeType.BOX) {
            this.type = type;
            if (shape == ShapeType.BOX) {
                this.shape = new BoxShape(new Jitter.LinearMath.JVector(entity.transform.scale.X,
                    entity.transform.scale.Y, entity.transform.scale.Z));
            } else if (shape == ShapeType.SPHERE) {
                this.shape = new SphereShape(entity.transform.scale.X);
            } else if (shape == ShapeType.CAPSULE) {
                this.shape = new CapsuleShape(entity.transform.scale.X, entity.transform.scale.Y);
            } else if (shape == ShapeType.CYLINDER) {
                this.shape = new CylinderShape(entity.transform.scale.X, entity.transform.scale.Y);
            } else if (shape == ShapeType.CONE) {
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
            body.EnableDebugDraw = true;
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
            
            body.EnableDebugDraw = true;
        }
        
        // public unsafe RigidBodyComponent(Entity entity, Model model, float mass = 1.0f, BodyType type = BodyType.DYNAMIC) {
        //     this.type = type;
        //
        //     // Create a list of convex shapes to represent the compound shape
        //     List<CompoundShape.TransformedShape> convexShapes = new List<CompoundShape.TransformedShape>();
        //     
        //     for (int i = 0; i < model.meshCount; i++)
        //     {
        //         Mesh mesh = model.meshes[i];
        //         
        //         JVector position = calculatePosition(calculateBoundingBox(mesh));
        //
        //         List<JVector> vertices = new List<JVector>();
        //         
        //         for (int j = 0; j < mesh.vertexCount; j += 3) {
        //             float x = mesh.vertices[j];
        //             float y = mesh.vertices[j + 1];
        //             float z = mesh.vertices[j + 2];
        //
        //             JVector vertex = new JVector(x, y, z);
        //
        //             vertices.Add(vertex);
        //         }
        //         
        //         ConvexHullShape convexShape = new ConvexHullShape(vertices);
        //         CompoundShape.TransformedShape transformedShape = new CompoundShape.TransformedShape(convexShape, JMatrix.Identity,
        //             position);
        //         // Console.WriteLine(transformedShape.BoundingBox.Max);
        //
        //         // Create a convex shape (e.g., a box) for each mesh
        //         // ConvexHullShape convexShape = new BoxShape(width, height, depth);
        //         // convexShape.Position = position;
        //
        //         // Add the convex shape to the list
        //         convexShapes.Add(transformedShape);
        //     }
        //
        //     // Console.WriteLine(convexShapes.Count);
        //     
        //     // Create a compound shape from the list of convex shapes
        //     shape = new CompoundShape(convexShapes);
        //     
        //     body = new RigidBody(shape);
        //     body.Position = new JVector(entity.transform.position.X, entity.transform.position.Y,
        //         entity.transform.position.Z);
        //     body.Orientation = JMatrix.CreateFromYawPitchRoll(entity.transform.rotation.Y * RayMath.DEG2RAD,
        //         entity.transform.rotation.X * RayMath.DEG2RAD, entity.transform.rotation.Z * RayMath.DEG2RAD);
        //     body.Mass = mass;
        //     shapeType = ShapeType.MODEL;
        //
        //     switch (this.type) {
        //         case BodyType.DYNAMIC:
        //             body.IsStatic = false;
        //             break;
        //         case BodyType.STATIC:
        //             body.IsStatic = true;
        //             break;
        //     }
        //
        //     m_transform = entity.transform;
        //
        //     World.world.AddBody(body);
        //     
        //     body.EnableDebugDraw = true;
        // }
        
        public unsafe RigidBodyComponent(Entity entity, Texture heightmap, float mass = 1.0f, BodyType type = BodyType.DYNAMIC) {
            this.type = type;
            shapeType = ShapeType.TERRAIN;

            Image image = Raylib.LoadImageFromTexture(heightmap);
            float[] heights = new float[image.width * image.height];

            Color* imageData = Raylib.LoadImageColors(image);
            
            for (int y = 0; y < image.height; y++)
            {
                for (int x = 0; x < image.width; x++)
                {
                    // Calculate the index for the current pixel
                    int index = y * image.width + x;

                    // Get the grayscale value from the pixel
                    float grayscaleValue = imageData[index].r / 255.0f;

                    // Store the grayscale value as a height (0 to 1)
                    heights[index] = grayscaleValue;
                }
            }
            
            float[,] heights2d = new float[image.height, image.width];

            // Populate the 2D array from the flat array
            for (int y = 0; y < image.height; y++)
            {
                for (int x = 0; x < image.width; x++)
                {
                    // Assign the value to the 2D array
                    heights2d[x, y] = heights[y * image.width + x] * entity.transform.scale.Y;
                }
            }

            shape = new TerrainShape(heights2d, entity.transform.scale.X / image.width, entity.transform.scale.Z / image.height);
            
            Raylib.UnloadImage(image);
            
            body = new RigidBody(shape);
            body.Position = new JVector(entity.transform.position.X - entity.transform.scale.X/2, entity.transform.position.Y  - entity.transform.scale.Y/2,
                entity.transform.position.Z - entity.transform.scale.Z/2);
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
            
            body.EnableDebugDraw = false;
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
                    entity.transform.rotation = new Vector3(MatrixToEuler(body.Orientation).X - 90,
                        MatrixToEuler(body.Orientation).Y - 90,
                        MatrixToEuler(body.Orientation).Z);
                else
                    entity.transform.rotation = MatrixToEuler(body.Orientation);
                m_transform = entity.transform;
            }
        }

        public void render() {
            if (debug) {
                if (shapeType != ShapeType.TERRAIN) body.DebugDraw(World.debugRenderer);
            }
        }

        public void dispose() {
            World.world.RemoveBody(body);
        }
        
        private unsafe BoundingBox calculateBoundingBox(Mesh mesh) {
            BoundingBox boundingBox = new BoundingBox();

                for (int i = 0; i < mesh.vertexCount; i += 3) {
                float x = mesh.vertices[i];
                float y = mesh.vertices[i + 1];
                float z = mesh.vertices[i + 2];
                
                // Update the bounding box min and max coordinates
                if (x < boundingBox.min.X) boundingBox.min.X = x;
                if (x > boundingBox.max.X) boundingBox.max.X = x;
                if (y < boundingBox.min.Y) boundingBox.min.Y = y;
                if (y > boundingBox.max.Y) boundingBox.max.Y = y;
                if (z < boundingBox.min.Z) boundingBox.min.Z = z;
                if (z > boundingBox.max.Z) boundingBox.max.Z = z;
            }

            return boundingBox;
        }

        // Calculate the position based on the bounding box
        private JVector calculatePosition(BoundingBox boundingBox) {
            // Calculate the center of the bounding box
            float centerX = (boundingBox.min.X + boundingBox.max.X) / 2.0f;
            float centerY = (boundingBox.min.Y + boundingBox.max.Y) / 2.0f;
            float centerZ = (boundingBox.min.Z + boundingBox.max.Z) / 2.0f;

            return new JVector(centerX, centerY, centerZ);
        }
        
    }
}