using System;
using aengine;
using static aengine.core.aengine;
using aengine.ecs;
using aengine.graphics;
using System.Diagnostics;
using System.Numerics;
using aengine_cs.aengine.parser;
using aengine_cs.aengine.windowing;
using Jitter.Collision.Shapes;
using Raylib_CsLo;
using static Raylib_CsLo.Raylib;
using static Raylib_CsLo.RlGl;

namespace aengine.ecs
{
    public class MeshComponent : Component
    {
        public TransformComponent transform;
        public Color color;
        public aModel model = new aModel();
        public bool isModel;
        public aTexture texture = new aTexture();
        public float scale;
        public string terrainPath = "";
        public ShapeType shape;
        
        private string m_name = "mesh";

        public unsafe MeshComponent(Entity entity, Color color, aTexture texture)
        {
            if (entity.transform != null) transform = entity.transform;
            else transform = new TransformComponent(null);
            this.color = color;
            this.texture = texture;
            if (entity.transform != null)
                model = new aModel(LoadModelFromMesh(GenMeshCube(entity.transform.scale.X, entity.transform.scale.Y, entity.transform.scale.Z)));
            else model = new aModel(LoadModelFromMesh(GenMeshCube(1, 1, 1)));
            shape = ShapeType.BOX;
            scale = 1;
            model.data.materials[0].maps[(int)MATERIAL_MAP_DIFFUSE].texture = texture.data;
            isModel = true;
        }
        
        public MeshComponent(Entity entity, Color color, aTexture texture, bool model)
        {
            if (entity.transform != null) transform = entity.transform;
            else transform = new TransformComponent(null);
            this.color = color;
            this.model = new aModel(LoadModelFromMesh(GenMeshCube(1, 1, 1)));
            shape = ShapeType.BOX;
            scale = 1;
            this.texture = texture;
            isModel = model;
        }
        
        // ShapeType shape cannot be terrain here
        public unsafe MeshComponent(Entity entity, ShapeType shape, Color color, aTexture texture)
        {
            if (entity.transform != null) transform = entity.transform;
            else transform = new TransformComponent(null);
            this.color = color;
            this.texture = texture;
            this.shape = shape;
            if (shape is ShapeType.BOX) {
                model = new aModel(LoadModelFromMesh(GenMeshCube(transform.scale.X, transform.scale.Y,
                    transform.scale.Z)));
            } else if (shape is ShapeType.SPHERE) {
                model = new aModel(LoadModelFromMesh(GenMeshSphere(transform.scale.X, 15, 15)));
            } else if (shape is ShapeType.CYLINDER) {
                model = new aModel(LoadModelFromMesh(GenMeshCylinder(transform.scale.X, transform.scale.Y, 15)));
            } else if (shape is ShapeType.CONE) {
                model = new aModel(LoadModelFromMesh(GenMeshCone(transform.scale.X, transform.scale.Y, 15)));
            } else if (shape is ShapeType.CAPSULE) {
                model = new aModel(LoadModelFromMesh(Rendering.genMeshCapsule(0.5f, 1, 15, 15)));
            }
            scale = 1;
            model.data.materials[0].maps[(int)MATERIAL_MAP_DIFFUSE].texture = texture.data;
            isModel = true;
        }
        
        public MeshComponent(Entity entity, ShapeType shape, Color color)
        {
            if (entity.transform != null) transform = entity.transform;
            else transform = new TransformComponent(null);
            this.color = color;
            this.shape = shape;
            if (shape is ShapeType.BOX) {
                model = new aModel(LoadModelFromMesh(GenMeshCube(1, 1, 1)));
            } else if (shape is ShapeType.SPHERE) {
                model = new aModel(LoadModelFromMesh(GenMeshSphere(0.5f, 15, 15)));
            } else if (shape is ShapeType.CYLINDER) {
                model = new aModel(LoadModelFromMesh(GenMeshCylinder(0.5f, 1, 15)));
            } else if (shape is ShapeType.CONE) {
                model = new aModel(LoadModelFromMesh(GenMeshCone(0.5f, 1, 15)));
            } else if (shape is ShapeType.CAPSULE) {
                model = new aModel(LoadModelFromMesh(Rendering.genMeshCapsule(0.5f, 1, 15, 15)));
            }
            scale = 1;
            isModel = true;
        }
        
        // aModel model has to be a model not a primitive generated shape here
        public unsafe MeshComponent(Entity entity, aModel model, Color color, aTexture texture)
        {
            if (entity.transform != null) transform = entity.transform;
            else transform = new TransformComponent(null);
            this.color = color;
            this.texture = texture;
            this.model = model;
            shape = ShapeType.MODEL;
            scale = 1;
            model.data.materials[0].maps[(int)MATERIAL_MAP_DIFFUSE].texture = texture.data;
            isModel = true;
        }
        
        // aModel model has to be a model not a primitive generated shape here
        public MeshComponent(Entity entity, aModel model, Color color)
        {
            if (entity.transform != null) transform = entity.transform;
            else transform = new TransformComponent(null);
            this.color = color;
            this.model = model;
            shape = ShapeType.MODEL;
            scale = 1;
            isModel = true;
        }

        public unsafe MeshComponent(Entity entity, aTexture terrain, Color color, aTexture texture) {
            if (entity.transform != null) transform = entity.transform;
            else transform = new TransformComponent(null);
            this.color = color;
            if (texture != null) this.texture = texture;
            model = new aModel(LoadModelFromMesh(GenMeshHeightmap(LoadImageFromTexture(terrain.data), Vector3.One)));
            shape = ShapeType.TERRAIN;
            scale = 1;
            model.data.materials[0].maps[(int)MATERIAL_MAP_DIFFUSE].texture = this.texture.data;
            isModel = true;
            terrainPath = terrain.path;
        }

        public void setTerrain(aTexture terrain) {
            model = new aModel(LoadModelFromMesh(GenMeshHeightmap(LoadImageFromTexture(terrain.data), Vector3.One)));
            shape = ShapeType.TERRAIN;
            scale = 1;
            isModel = true;
            terrainPath = terrain.path;
        }
        
        public void setModel(aModel model) {
            this.model = model;
            shape = ShapeType.MODEL;
            scale = 1;
            isModel = true;
        }

        public unsafe void setTexture(aTexture texture, int mat = 0, int map = 0)
        {
            this.texture.data = texture.data;
            this.texture.path = texture.path;
            model.data.materials[mat].maps[map].texture = this.texture.data;
        }

        public unsafe void setShader(aShader shader, int mat = 0)
        {
            model.data.materials[mat].shader = shader.handle;
        }

        public unsafe void setColor(Color color) {
            model.data.materials[0].maps[(int)MATERIAL_MAP_DIFFUSE].color = color;
        }

        public void setShape(ShapeType shape) {
            this.shape = shape;
            isModel = true;
            if (shape is ShapeType.BOX) {
                model = new aModel(LoadModelFromMesh(GenMeshCube(1, 1, 1)));
                if (texture.path != String.Empty) setTexture(texture);
            } else if (shape is ShapeType.SPHERE) {
                model = new aModel(LoadModelFromMesh(GenMeshSphere(0.5f, 15, 15)));
                if (texture.path != String.Empty) setTexture(texture);
            } else if (shape is ShapeType.CYLINDER) {
                model = new aModel(LoadModelFromMesh(GenMeshCylinder(0.5f, 1, 15)));
                if (texture.path != String.Empty) setTexture(texture);
            } else if (shape is ShapeType.CONE) {
                model = new aModel(LoadModelFromMesh(GenMeshCone(0.5f, 1, 15)));
                if (texture.path != String.Empty) setTexture(texture);
            } else if (shape is ShapeType.CAPSULE) {
                model = new aModel(LoadModelFromMesh(Rendering.genMeshCapsule(0.5f, 1, 15, 15)));
                if (texture.path != String.Empty) setTexture(texture);
            } else if (shape is ShapeType.SPRITE) {
                isModel = false;
            }
        }

        public void applyAnimation(ModelArmature anim, int id) {
            if (id > anim.animations.Length || id < 0) {
                Console.WriteLine("id has to be less than the animation amount or greater than zero");
                return;
            }
            
            anim.frameCounter += (int)(anim.getSpeed() * GetFrameTime());
            UpdateModelAnimation(model.data, anim.animations[id], anim.frameCounter);
            if (anim.frameCounter >= anim.animations[id].frameCount) anim.frameCounter = 0;
        }
        
        public void applyAnimation(ModelArmature anim, int id, float speed = 100) {
            if (id > anim.animations.Length || id < 0) {
                Console.WriteLine("id has to be less than the animation amount or greater than zero");
                return;
            }
            
            anim.setSpeed(speed);
            anim.frameCounter += (int)(anim.getSpeed() * GetFrameTime());
            UpdateModelAnimation(model.data, anim.animations[id], anim.frameCounter);
            if (anim.frameCounter >= anim.animations[id].frameCount) anim.frameCounter = 0;
        }

        private void setScale(Vector3 scale)
        {
            model.data.transform = RayMath.MatrixScale(scale.X, scale.Y, scale.Z);
        }

        private void setRotation(Vector3 rotation)
        {
            model.data.transform = RayMath.MatrixRotateXYZ(new Vector3(deg2Rad(rotation.X), deg2Rad(rotation.Y), deg2Rad(rotation.Z)));
        }

        private void setTransform(Vector3 scale, Vector3 rotation) {
            model.data.transform = RayMath.MatrixMultiply(
                RayMath.MatrixRotateXYZ(Vector3.Zero with { X = deg2Rad(rotation.X), Y = deg2Rad(rotation.Y), Z = deg2Rad(rotation.Z)}),
                RayMath.MatrixScale(scale.X, scale.Y, scale.Z)
            );
        }

        public void update(Entity entity)
        {
            if (entity != null)  {
                transform = entity.transform;
                setTransform(transform.scale, transform.rotation);
            }
        }

        public void render()
        {
            if (!Window.isEditor && isModel) DrawModel(model.data, transform.position, scale, color);
            else if (Window.isEditor && isModel && shape is ShapeType.TERRAIN) DrawModel(model.data, transform.position - transform.scale * 0.5f, scale, color);
            else if (Window.isEditor && isModel) DrawModel(model.data, transform.position, scale, color);
            else Rendering.drawSprite3D(texture.data, transform.position, transform.scale.X, transform.scale.Y, transform.rotation.X, transform.rotation.Y, color);
        }

        public void dispose()
        {
            if (model != null) model.dispose();
            if (texture != null) texture.dispose();
        }

        public string fileName() {
            return m_name;
        }

        public string getType() {
            return "MeshComponent";
        }
        
    }
}