using System;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common; 
using OpenTK.Windowing.Desktop;

namespace aengine.graphics
{
    public class Camera 
    {
        public Matrix4 projectionMatrix;    
        public Matrix4 viewMatrix;

        public float aspectRatio;
        public float fieldOfView;
        public float nearPlane;
        public float farPlane;

        public Vector3 cameraPosition;
        public Vector3 cameraTarget;
        public Vector3 cameraUp;

        public Camera()
        {
            
        }

    }
}