using Raylib_CsLo;
using static Raylib_CsLo.Raylib;
using System.Numerics;
using aengine.core;

namespace aengine.ecs
{
    public class MeshComponent : Component
    {
        private TransformComponent transform;
        public ShapeType shape;
        public Model model;
        private Vector3 modelScale;
        public Texture texture;
        public Color color;
        public Color tint;
        private String m_etag;

        public MeshComponent(Entity entity)
        {
            this.transform = entity.transform;
            this.model = new Model();
            this.texture = new Texture();
            this.color = BLANK;
            this.tint = BLANK;
            this.m_etag = entity.tag;
        }

        public void setModelScale(Vector3 scale)
        {
            this.modelScale = scale;
            this.model.transform = RayMath.MatrixScale(scale.X, scale.Y, scale.Z);
        }

        public void setModelRotation(Vector3 rotation)
        {
            this.model.transform = RayMath.MatrixRotateXYZ(new Vector3(aengine.core.aengine.deg2Rad(rotation.X), aengine.core.aengine.deg2Rad(rotation.Y), aengine.core.aengine.deg2Rad(rotation.Z)));
        }

        public override void update(Entity entity)
        {
            base.update(entity);
            this.transform = entity.transform;
        }

        public override void render()
        {
            base.render();
            switch (this.shape)
            {
                case ShapeType.BOX:
                    aengine.core.aengine.DrawCubeTexturePro(this.texture, new Vector3(this.transform.position.X, this.transform.position.Y, this.transform.position.Z), this.transform.scale.X, this.transform.scale.Y, this.transform.scale.Z, this.transform.rotation, this.color);
                    break;
                case ShapeType.SPHERE:
                    aengine.core.aengine.DrawSphere(this.transform.position, this.transform.scale.X, this.color);
                    break;
                case ShapeType.CYLINDER:
                    DrawCylinder(this.transform.position, this.transform.scale.X, this.transform.scale.Z, this.transform.scale.Y, 50, this.color);
                    break;
                default:
                    aengine.core.aengine.DrawCubeTexturePro(this.texture, new Vector3(this.transform.position.X, this.transform.position.Y, this.transform.position.Z), this.transform.scale.X, this.transform.scale.Y, this.transform.scale.Z, this.transform.rotation, this.color);
                    break;
            }

            DrawModel(this.model, new Vector3(this.transform.position.X, this.transform.position.Y, this.transform.position.Z), this.modelScale.X, this.tint);
        }

        public override void dispose()
        {
            base.dispose();
            UnloadTexture(this.texture);
            UnloadModel(this.model);
        }

    }
}