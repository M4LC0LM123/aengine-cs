using System;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common; 
using OpenTK.Windowing.Desktop;
using StbImageSharp;
using aengine;
using static aengine.core.aengine;
using static aengine.graphics.Graphics;
using static aengine.graphics.Rendering;
using aengine.ecs;
using aengine.graphics;
using aengine.input;
using SharpFNT;
using aengine.loader;
using System.Diagnostics;

namespace aengine.ecs
{
    public class MeshComponent : Component
    {
        private TransformComponent transform;
        public ShapeType shape;
        public Texture texture;
        public Color color;
        private String m_etag;

        public MeshComponent(Entity entity)
        {
            if (entity.transform != null) this.transform = entity.transform;
            else this.transform = new TransformComponent(null);
            this.texture = new Texture();
            this.color = Colors.WHITE;
            this.m_etag = entity.tag;
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
                    drawTexturedCube(this.texture, this.transform.position, this.transform.scale, this.transform.rotation, this.color);
                    break;
                case ShapeType.SPHERE:
                    drawTexturedSphere(this.texture, this.transform.position, this.transform.scale, this.transform.rotation, this.color);
                    break;
                case ShapeType.CYLINDER:
                    drawTexturedCylinder(this.texture, this.transform.position, this.transform.scale, this.transform.rotation, this.color);
                    break;
                case ShapeType.CAPSULE:
                    drawTexturedCapsule(this.texture, this.transform.position, this.transform.scale, this.transform.rotation, this.color);
                    break;
                case ShapeType.SPRITE3D:
                    drawSprite3D(this.texture, this.transform.position, this.transform.scale.X, this.transform.scale.Y, this.transform.rotation.X, this.color);
                    break;
            }
        }

        public override void dispose()
        {
            base.dispose();
        }

    }
}