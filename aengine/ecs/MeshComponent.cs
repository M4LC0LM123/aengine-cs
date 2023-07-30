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
        public float scale;

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
        }
        
        public unsafe MeshComponent(Entity entity, Mesh mesh, Color color, Texture texture)
        {
            if (entity.transform != null) this.transform = entity.transform;
            else transform = new TransformComponent(null);
            this.color = color;
            model = LoadModelFromMesh(mesh);
            scale = 1;
            model.materials[0].maps[(int)MATERIAL_MAP_DIFFUSE].texture = texture;
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
            model.transform = RayMath.MatrixRotateXYZ(rotation);
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
            DrawModel(model, transform.position, scale, color);
        }

        public override void dispose()
        {
            UnloadModel(model);
            base.dispose();
        }

    }
}