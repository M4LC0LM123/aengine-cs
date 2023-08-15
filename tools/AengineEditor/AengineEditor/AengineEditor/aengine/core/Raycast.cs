using System.Numerics;
using aengine.ecs;

namespace aengine.core;

public class Raycast
{
    public Vector3 position;
    public Vector3 target;

    public Raycast()
    {
        position = new Vector3();
        target = new Vector3();
    }
    
    public Raycast(Vector3 position, Vector3 target)
    {
        this.position = position;
        this.target = target;
    }

    public void debugRender()
    {
        Raylib_CsLo.Raylib.DrawLine3D(position, target, Raylib_CsLo.Raylib.GREEN);
    }

    public bool isColliding(TransformComponent transform, ShapeType shape)
    {
        if (shape == ShapeType.BOX)
        {
            Vector3 rayDirection = Vector3.Normalize(target - position);
            Vector3 boxSize = transform.scale;
            Vector3 boxMin = transform.position - boxSize * 0.5f;
            Vector3 boxMax = transform.position + boxSize * 0.5f;

            float tmin = (boxMin.X - position.X) / rayDirection.X;
            float tmax = (boxMax.X - position.X) / rayDirection.X;

            if (tmin > tmax)
            {
                float temp = tmin;
                tmin = tmax;
                tmax = temp;
            }

            float tymin = (boxMin.Y - position.Y) / rayDirection.Y;
            float tymax = (boxMax.Y - position.Y) / rayDirection.Y;

            if (tymin > tymax)
            {
                float temp = tymin;
                tymin = tymax;
                tymax = temp;
            }

            if ((tmin > tymax) || (tymin > tmax))
            {
                return false;
            }

            if (tymin > tmin)
            {
                tmin = tymin;
            }

            if (tymax < tmax)
            {
                tmax = tymax;
            }

            float tzmin = (boxMin.Z - position.Z) / rayDirection.Z;
            float tzmax = (boxMax.Z - position.Z) / rayDirection.Z;

            if (tzmin > tzmax)
            {
                float temp = tzmin;
                tzmin = tzmax;
                tzmax = temp;
            }

            if ((tmin > tzmax) || (tzmin > tmax))
            {
                return false;
            }

            if (tzmin > tmin)
            {
                tmin = tzmin;
            }

            if (tzmax < tmax)
            {
                tmax = tzmax;
            }

            return true;
        }
        
        if (shape == ShapeType.SPHERE)
        {
            Vector3 rayDirection = Vector3.Normalize(target - position);
            Vector3 sphereCenter = transform.position;
            float sphereRadius = transform.scale.X;

            Vector3 oc = position - sphereCenter;
            float a = Vector3.Dot(rayDirection, rayDirection);
            float b = 2.0f * Vector3.Dot(oc, rayDirection);
            float c = Vector3.Dot(oc, oc) - sphereRadius * sphereRadius;
            float discriminant = b * b - 4 * a * c;

            if (discriminant < 0)
            {
                return false;
            }

            float sqrtDiscriminant = (float)Math.Sqrt(discriminant);
            float t1 = (-b - sqrtDiscriminant) / (2.0f * a);
            float t2 = (-b + sqrtDiscriminant) / (2.0f * a);

            if (t1 >= 0 || t2 >= 0)
            {
                return true;
            }

            return false;
        }

        return false; // Return false for any other shape types.
    }

}