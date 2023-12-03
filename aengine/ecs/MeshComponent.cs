using System;
using aengine;
using static aengine.core.aengine;
using aengine.ecs;
using aengine.graphics;
using System.Diagnostics;
using System.Numerics;
using aengine_cs.aengine.parser;
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
            scale = 1;
            model.data.materials[0].maps[(int)MATERIAL_MAP_DIFFUSE].texture = texture.data;
            isModel = true;
        }
        
        public MeshComponent(Entity entity, Color color, aTexture texture, bool model)
        {
            if (entity.transform != null) transform = entity.transform;
            else transform = new TransformComponent(null);
            this.color = color;
            this.model = new aModel();
            scale = 1;
            this.texture = texture;
            isModel = model;
        }
        
        public unsafe MeshComponent(Entity entity, Mesh mesh, Color color, aTexture texture)
        {
            if (entity.transform != null) transform = entity.transform;
            else transform = new TransformComponent(null);
            this.color = color;
            this.texture = texture;
            model = new aModel(LoadModelFromMesh(mesh));
            scale = 1;
            model.data.materials[0].maps[(int)MATERIAL_MAP_DIFFUSE].texture = texture.data;
            isModel = true;
        }
        
        public MeshComponent(Entity entity, Mesh mesh, Color color)
        {
            if (entity.transform != null) transform = entity.transform;
            else transform = new TransformComponent(null);
            this.color = color;
            model = new aModel(LoadModelFromMesh(mesh));
            scale = 1;
            isModel = true;
        }
        
        public unsafe MeshComponent(Entity entity, aModel model, Color color, aTexture texture)
        {
            if (entity.transform != null) transform = entity.transform;
            else transform = new TransformComponent(null);
            this.color = color;
            this.texture = texture;
            this.model = model;
            scale = 1;
            model.data.materials[0].maps[(int)MATERIAL_MAP_DIFFUSE].texture = texture.data;
            isModel = true;
        }
        
        public MeshComponent(Entity entity, aModel model, Color color)
        {
            if (entity.transform != null) transform = entity.transform;
            else transform = new TransformComponent(null);
            this.color = color;
            this.model = model;
            scale = 1;
            isModel = true;
        }

        public unsafe void setTexture(Texture texture, int mat = 0, int map = 0)
        {
            model.data.materials[mat].maps[map].texture = texture;
        }

        public unsafe void setShader(aShader shader, int mat = 0)
        {
            model.data.materials[mat].shader = shader.shader;
        }

        private void setScale(Vector3 scale)
        {
            model.data.transform = RayMath.MatrixScale(scale.X, scale.Y, scale.Z);
        }

        private void setRotation(Vector3 rotation)
        {
            model.data.transform = RayMath.MatrixRotateXYZ(new Vector3(deg2Rad(rotation.X), deg2Rad(rotation.Y), deg2Rad(rotation.Z)));
        }

        public void update(Entity entity)
        {
            if (entity != null)
            {
                if (entity.hasComponent<RigidBodyComponent>()) {
                    if (entity.getComponent<RigidBodyComponent>().shapeType == ShapeType.CYLINDER) {
                        transform.position = entity.transform.position;
                        transform.rotation = entity.transform.rotation with { X = entity.transform.rotation.X + 90 };
                        transform.scale = entity.transform.scale;
                    } else {
                        transform = entity.transform;  
                    }
                } else {
                    transform = entity.transform;         
                }
                setScale(transform.scale);
                setRotation(transform.rotation);
            }
        }

        public void render()
        {
            if (isModel) DrawModel(model.data, transform.position, scale, color);
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