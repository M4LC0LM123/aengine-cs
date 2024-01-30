using System.Numerics;
using aengine.ecs;
using static aengine.core.aengine;
using Raylib_CsLo;

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
        Raylib.DrawLine3D(position, target, Raylib.GREEN);
    }

    public Vector3 getContactPoint(TransformComponent transform, ShapeType shape) {
        Vector3 rayDirection = Vector3.Normalize(target - position);

        if (shape == ShapeType.BOX) {
            float tmin, tmax, tymin, tymax, tzmin, tzmax;

            Vector3 boxSize = transform.scale;
            Vector3 boxMin = transform.position - boxSize * 0.5f;
            Vector3 boxMax = transform.position + boxSize * 0.5f;

            tmin = (boxMin.X - position.X) / rayDirection.X;
            tmax = (boxMax.X - position.X) / rayDirection.X;

            if (tmin > tmax) {
                (tmin, tmax) = (tmax, tmin);
            }

            tymin = (boxMin.Y - position.Y) / rayDirection.Y;
            tymax = (boxMax.Y - position.Y) / rayDirection.Y;

            if (tymin > tymax) {
                (tymin, tymax) = (tymax, tymin);
            }

            if ((tmin > tymax) || (tymin > tmax)) {
                return Vector3.Zero;
            }

            if (tymin > tmin) {
                tmin = tymin;
            }

            if (tymax < tmax) {
                tmax = tymax;
            }

            tzmin = (boxMin.Z - position.Z) / rayDirection.Z;
            tzmax = (boxMax.Z - position.Z) / rayDirection.Z;

            if (tzmin > tzmax) {
                (tzmin, tzmax) = (tzmax, tzmin);
            }

            if ((tmin > tzmax) || (tzmin > tmax)) {
                return Vector3.Zero;
            }

            if (tzmin > tmin) {
                tmin = tzmin;
            }

            if (tzmax < tmax) {
                tmax = tzmax;
            }

            // Calculate contact point for a box
            return position + rayDirection * tmin;
        }

        if (shape == ShapeType.SPHERE) {
            float t1, t2;

            Vector3 sphereCenter = transform.position;
            float sphereRadius = transform.scale.X * 0.5f;

            Vector3 oc = position - sphereCenter;
            float a = Vector3.Dot(rayDirection, rayDirection);
            float b = 2.0f * Vector3.Dot(oc, rayDirection);
            float c = Vector3.Dot(oc, oc) - sphereRadius * sphereRadius;
            float discriminant = b * b - 4 * a * c;

            if (discriminant < 0) {
                return Vector3.Zero;
            }

            float sqrtDiscriminant = (float)Math.Sqrt(discriminant);
            t1 = (-b - sqrtDiscriminant) / (2.0f * a);
            t2 = (-b + sqrtDiscriminant) / (2.0f * a);

            if (t1 >= 0 || t2 >= 0) {
                // Calculate contact point for a sphere
                return position + rayDirection * t1; // You can choose either t1 or t2
            }

            return Vector3.Zero;
        }

        return Vector3.Zero; // Return Vector3.Zero for any other shape types or no collision.
    }

    public bool isColliding(TransformComponent transform, ShapeType shape, out Vector3 contactPoint) {
        contactPoint = Vector3.Zero;

        if (shape == ShapeType.BOX) {
            float tmin, tmax, tymin, tymax, tzmin, tzmax;
            
            // apply rotation to ray
            Matrix4x4 rotationMatrix = Matrix4x4.CreateFromYawPitchRoll(
                deg2Rad(transform.rotation.Y),
                deg2Rad(transform.rotation.X),
                deg2Rad(transform.rotation.Z));

            Vector3 rayDirection = Vector3.Normalize(target - position);
            Vector3 rotatedRayDirection = Vector3.Transform(rayDirection, rotationMatrix);
            
            Vector3 boxSize = transform.scale;
            Vector3 boxMin = transform.position - boxSize * 0.5f;
            Vector3 boxMax = transform.position + boxSize * 0.5f;

            tmin = (boxMin.X - position.X) / rayDirection.X;
            tmax = (boxMax.X - position.X) / rayDirection.X;

            if (tmin > tmax) {
                (tmin, tmax) = (tmax, tmin);
            }

            tymin = (boxMin.Y - position.Y) / rayDirection.Y;
            tymax = (boxMax.Y - position.Y) / rayDirection.Y;

            if (tymin > tymax) {
                (tymin, tymax) = (tymax, tymin);
            }

            if ((tmin > tymax) || (tymin > tmax)) {
                return false;
            }

            if (tymin > tmin) {
                tmin = tymin;
            }

            if (tymax < tmax) {
                tmax = tymax;
            }

            tzmin = (boxMin.Z - position.Z) / rayDirection.Z;
            tzmax = (boxMax.Z - position.Z) / rayDirection.Z;

            if (tzmin > tzmax) {
                (tzmin, tzmax) = (tzmax, tzmin);
            }

            if ((tmin > tzmax) || (tzmin > tmax)) {
                return false;
            }

            if (tzmin > tmin) {
                tmin = tzmin;
            }

            if (tzmax < tmax) {
                tmax = tzmax;
            }

            // Calculate contact point for a box
            contactPoint = position + rotatedRayDirection * tmin;
            return true;
        }
         
        if (shape == ShapeType.SPHERE) {
            float t1, t2;

            Vector3 rayDirection = Vector3.Normalize(target - position);
            Vector3 sphereCenter = transform.position;
            float sphereRadius = transform.scale.X * 0.5f;

            Vector3 oc = position - sphereCenter;
            float a = Vector3.Dot(rayDirection, rayDirection);
            float b = 2.0f * Vector3.Dot(oc, rayDirection);
            float c = Vector3.Dot(oc, oc) - sphereRadius * sphereRadius;
            float discriminant = b * b - 4 * a * c;

            if (discriminant < 0) {
                return false;
            }

            float sqrtDiscriminant = (float)Math.Sqrt(discriminant);
            t1 = (-b - sqrtDiscriminant) / (2.0f * a);
            t2 = (-b + sqrtDiscriminant) / (2.0f * a);

            if (t1 >= 0 || t2 >= 0) {
                // Calculate contact point for a sphere
                contactPoint = position + rayDirection * t1; // You can choose either t1 or t2
                return true;
            }

            return false;
        }

        return false;
    }

}