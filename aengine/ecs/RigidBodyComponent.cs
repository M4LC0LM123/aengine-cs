using System.Numerics;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;
using aengine.graphics;
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

        public aModel model = new aModel();
        public aTexture heightmap = new aTexture();

        private TransformComponent m_transform;
        private string m_name = "rb";

        public RigidBodyComponent(Entity entity, float mass = 1.0f, BodyType type = BodyType.DYNAMIC, ShapeType shape = ShapeType.BOX) {
            this.type = type;
            if (shape == ShapeType.BOX) {
                this.shape = new BoxShape(new Jitter.LinearMath.JVector(entity.transform.scale.X,
                    entity.transform.scale.Y, entity.transform.scale.Z));
            } else if (shape == ShapeType.SPHERE) {
                this.shape = new SphereShape(entity.transform.scale.X * 0.5f);
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
            body.Orientation = JMatrix.CreateFromYawPitchRoll(-entity.transform.rotation.Y * RayMath.DEG2RAD,
                -entity.transform.rotation.X * RayMath.DEG2RAD, -entity.transform.rotation.Z * RayMath.DEG2RAD);
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
        
        public unsafe RigidBodyComponent(Entity entity, aModel model, float mass = 1.0f, BodyType type = BodyType.DYNAMIC) {
            this.type = type;
            
            List<JVector> vertices = new List<JVector>();
        
            for (int i = 0; i < model.data.meshCount; i++) {
                Mesh mesh = model.data.meshes[i];
                for (int j = 0; j < mesh.vertexCount; j += 3) {
                    float x = mesh.vertices[j];
                    float y = mesh.vertices[j + 1];
                    float z = mesh.vertices[j + 2];
        
                    JVector vertex = new JVector(x, y, z);
                
                    vertices.Add(vertex);
                }
            }

            this.model = model;
        
            shape = new ConvexHullShape(vertices);
            
            body = new RigidBody(shape);
            body.Position = new JVector(entity.transform.position.X, entity.transform.position.Y,
                entity.transform.position.Z);
            body.Orientation = JMatrix.CreateFromYawPitchRoll(-entity.transform.rotation.Y * RayMath.DEG2RAD,
                -entity.transform.rotation.X * RayMath.DEG2RAD, -entity.transform.rotation.Z * RayMath.DEG2RAD);
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
        
        public unsafe RigidBodyComponent(Entity entity, aTexture heightmap, float mass = 1.0f, BodyType type = BodyType.DYNAMIC) {
            this.type = type;
            shapeType = ShapeType.TERRAIN;

            this.heightmap = heightmap;
            
            Image image = Raylib.LoadImageFromTexture(heightmap.data);
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
            body.Orientation = JMatrix.CreateFromYawPitchRoll(-entity.transform.rotation.Y * RayMath.DEG2RAD,
                -entity.transform.rotation.X * RayMath.DEG2RAD, -entity.transform.rotation.Z * RayMath.DEG2RAD);
            body.Mass = mass;

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

        // shape cannot be a model or a heightmap
        public void setShape(ShapeType shape) {
            if (shape is ShapeType.BOX) {
                body.Shape = new BoxShape(vecToJVec(m_transform.scale));
            } else if (shape is ShapeType.SPHERE) {
                body.Shape = new SphereShape(m_transform.scale.X * 0.5f);
            } else if (shape is ShapeType.CAPSULE) {
                body.Shape = new CapsuleShape(m_transform.scale.X, m_transform.scale.Y);
            } else if (shape is ShapeType.CYLINDER) {
                body.Shape = new CylinderShape(m_transform.scale.X, m_transform.scale.Y);
            } else if (shape is ShapeType.CONE) {
                body.Shape = new ConeShape(m_transform.scale.X, m_transform.scale.Y);
            }
            
            body.Shape.UpdateShape();
            shapeType = shape;
        }

        public unsafe void setModelShape(aModel model) {
            List<JVector> vertices = new List<JVector>();
        
            for (int i = 0; i < model.data.meshCount; i++) {
                Mesh mesh = model.data.meshes[i];
                for (int j = 0; j < mesh.vertexCount; j += 3) {
                    float x = mesh.vertices[j];
                    float y = mesh.vertices[j + 1];
                    float z = mesh.vertices[j + 2];
        
                    JVector vertex = new JVector(x, y, z);
                
                    vertices.Add(vertex);
                }
            }

            this.model = model;
        
            body.Shape = new ConvexHullShape(vertices);
            body.Shape.UpdateShape();
            shapeType = ShapeType.MODEL;
        }

        public unsafe void setTerrainShape(aTexture heightmap) {
            this.heightmap = heightmap;
            
            Image image = Raylib.LoadImageFromTexture(heightmap.data);
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
                    heights2d[x, y] = heights[y * image.width + x] * m_transform.scale.Y;
                }
            }

            shape = new TerrainShape(heights2d, m_transform.scale.X / image.width, m_transform.scale.Z / image.height);
            
            Raylib.UnloadImage(image);

            body.Shape.UpdateShape();
            shapeType = ShapeType.TERRAIN;
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
            body.Position = JVector.Zero with {
                X = x,
                Y = y,
                Z = z
            };
        }

        public void setPosition(Vector3 position) {
            body.Position = new JVector(position.X, position.Y, position.Z);
        }

        public void setRotation(Vector3 rotation) {
            body.Orientation = JMatrix.CreateFromYawPitchRoll(-rotation.Y, -rotation.X, -rotation.Z);
        }

        public void setScale(Vector3 scale) {
            if (shapeType is ShapeType.BOX) {
                body.Shape = new BoxShape(vecToJVec(scale));
            } else if (shapeType is ShapeType.SPHERE) {
                body.Shape = new SphereShape(scale.X * 0.5f);
            } else if (shapeType is ShapeType.CAPSULE) {
                body.Shape = new CapsuleShape(scale.X, scale.Y);
            } else if (shapeType is ShapeType.CYLINDER) {
                body.Shape = new CylinderShape(scale.X, scale.Y);
            } else if (shapeType is ShapeType.CONE) {
                body.Shape = new ConeShape(scale.X, scale.Y);
            }
            
            body.Shape.UpdateShape();
        }

        public void affectedByGravity(bool affected) {
            body.AffectedByGravity = affected;
        }

        public void update(Entity entity) {
            if (entity != null && body != null && World.usePhysics) {
                if (shapeType == ShapeType.CONE) {
                    entity.transform.position = Vector3.Zero with {
                        X = body.Position.X,
                        Y = body.Position.Y - entity.transform.scale.Y / 2.675f,
                        Z = body.Position.Z
                    };
                } else if (shapeType == ShapeType.CYLINDER) {
                    entity.transform.position = Vector3.Zero with {
                        X = body.Position.X,
                        Y = body.Position.Y - entity.transform.scale.Y * 0.5f,
                        Z = body.Position.Z
                    };
                } else {
                    entity.transform.position = Vector3.Zero with {
                        X = body.Position.X,
                        Y = body.Position.Y,
                        Z = body.Position.Z
                    };
                }

                entity.transform.rotation = MatrixToEuler(body.Orientation);
                
                m_transform = entity.transform;
            }
        }

        public void render() {
            if (debug) {
                if (World.debugRenderTerrain) {
                    body.DebugDraw(World.debugRenderer);
                } else {
                    if (shapeType != ShapeType.TERRAIN) {
                        body.DebugDraw(World.debugRenderer);
                    }  
                }
            }
        }

        public void dispose() {
            model.dispose();
            heightmap.dispose();
            World.world.RemoveBody(body);
        }

        public string fileName() {
            return m_name;
        }
        
        public string getType() {
            return "RigidBodyComponent";
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

        // public Shape shape;
        // public RigidBody body;
        // public BodyType type;
        // public ShapeType shapeType;
        // public bool debug = false;
        //
        // public aModel model = new aModel();
        // public aTexture heightmap = new aTexture();
        //
        // private TransformComponent m_transform;
        // private string m_name = "rb";
        
        public Component copy() {
            RigidBodyComponent copy;

            if (shape.GetType() == typeof(ConvexHullShape)) {
                copy = new RigidBodyComponent(null, model, body.Mass, type);
            } else if (shape.GetType() == typeof(TerrainShape)) {
                copy = new RigidBodyComponent(null, heightmap, body.Mass, type);
            } else {
                copy = new RigidBodyComponent(null, body.Mass, type, shapeType);
            }
            
            copy.setPosition(m_transform.position);
            copy.setRotation(m_transform.rotation);
            copy.setScale(m_transform.scale);

            return copy;
        }
    }
}