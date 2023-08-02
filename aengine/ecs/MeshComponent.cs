using System;
using aengine;
using static aengine.core.aengine;
using aengine.ecs;
using aengine.graphics;
using System.Diagnostics;
using System.Numerics;
using Raylib_CsLo;
using static Raylib_CsLo.Raylib;

namespace aengine.ecs
{
    public class MeshComponent : Component
    {
        private TransformComponent transform;
        public Color color;
        public Model model;
        public bool isModel;
        public Texture texture;
        public float scale;
        public ShapeType shape;

        public unsafe MeshComponent(Entity entity, Color color, Texture texture)
        {
            if (entity.transform != null) this.transform = entity.transform;
            else transform = new TransformComponent(null);
            this.color = color;
            if (entity.transform != null)
                model = LoadModelFromMesh(GenMeshCube(entity.transform.scale.X, entity.transform.scale.Y, entity.transform.scale.Z));
            else model = LoadModelFromMesh(GenMeshCube(1, 1, 1));
            scale = 1;
            model.materials[0].maps[(int)MATERIAL_MAP_DIFFUSE].texture = texture;
            isModel = true;
        }
        
        public unsafe MeshComponent(Entity entity, Color color, Texture texture, bool model = false)
        {
            if (entity.transform != null) this.transform = entity.transform;
            else transform = new TransformComponent(null);
            this.color = color;
            this.model = new Model();
            scale = 1;
            this.texture = texture;
            isModel = model;
        }
        
        public unsafe MeshComponent(Entity entity, Mesh mesh, Color color, Texture texture)
        {
            if (entity.transform != null) this.transform = entity.transform;
            else transform = new TransformComponent(null);
            this.color = color;
            model = LoadModelFromMesh(mesh);
            scale = 1;
            model.materials[0].maps[(int)MATERIAL_MAP_DIFFUSE].texture = texture;
            isModel = true;
        }
        
        public unsafe MeshComponent(Entity entity, Model model, Color color, Texture texture)
        {
            if (entity.transform != null) this.transform = entity.transform;
            else transform = new TransformComponent(null);
            this.color = color;
            this.model = model;
            scale = 1;
            model.materials[0].maps[(int)MATERIAL_MAP_DIFFUSE].texture = texture;
            isModel = true;
        }

        public unsafe void setTexture(Texture texture, int mat = 0, int map = 0)
        {
            model.materials[mat].maps[map].texture = texture;
        }

        public unsafe void setShader(Shader shader, int mat = 0)
        {
            model.materials[mat].shader = shader;
        }

        private void setSCale(Vector3 scale)
        {
            model.transform = RayMath.MatrixScale(scale.X, scale.Y, scale.Z);
        }

        private void setRotation(Vector3 rotation)
        {
            model.transform = RayMath.MatrixRotateXYZ(new Vector3(deg2Rad(rotation.X), deg2Rad(rotation.Y), deg2Rad(rotation.Z)));
            // model.transform = Matrix4x4.CreateFromYawPitchRoll(rotation.X, rotation.Y, rotation.Z);
        }

        public override void update(Entity entity)
        {
            base.update(entity);
            transform = entity.transform;
            setSCale(transform.scale);
            setRotation(transform.rotation);
        }

        public override void render()
        {
            base.render();
            if (isModel) DrawModel(model, transform.position, scale, color);
            else Rendering.drawSprite3D(texture, transform.position, transform.scale.X, transform.scale.Y, transform.rotation.X, color);
        }

        public override void dispose()
        {
            UnloadModel(model);
            base.dispose();
        }

    }
}