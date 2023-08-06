package com.aengineeditor.game;

import com.badlogic.gdx.math.Vector3;
import com.badlogic.gdx.math.collision.BoundingBox;

public class Collision {
    public static Vector3 normalize(Vector3 v) {
        float magnitude = (float)Math.sqrt(v.x*v.x + v.y*v.y + v.z*v.z);
        float x = v.x/magnitude;
        float y = v.y/magnitude;
        float z = v.z/magnitude;
        return new Vector3(x, y, z);
    }

    public static boolean isRayColliding(Vector3 position, Vector3 target, Transform transform)
    {
        ShapeType shape = ShapeType.BOX;
        if (shape == ShapeType.BOX)
        {
            Vector3 rayDirection = normalize(target.sub(position));
            Vector3 boxSize = transform.scale;
            Vector3 boxMin = new Vector3(transform.position.x - boxSize.x * 0.5f, transform.position.y - boxSize.y * 0.5f, transform.position.z - boxSize.z * 0.5f);
            Vector3 boxMax = new Vector3(transform.position.x + boxSize.x * 0.5f, transform.position.y + boxSize.y * 0.5f, transform.position.z + boxSize.z * 0.5f);

            float tmin = (boxMin.x - position.x) / rayDirection.x;
            float tmax = (boxMax.x - position.x) / rayDirection.x;

            if (tmin > tmax)
            {
                float temp = tmin;
                tmin = tmax;
                tmax = temp;
            }

            float tymin = (boxMin.y - position.y) / rayDirection.y;
            float tymax = (boxMax.y - position.y) / rayDirection.y;

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

            float tzmin = (boxMin.z - position.z) / rayDirection.z;
            float tzmax = (boxMax.z - position.z) / rayDirection.z;

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

        return false; // Return false for any other shape types.
    }

    public static boolean intersectRay(BoundingBox box, Vector3 origin, Vector3 direction) {
        float tmin, tmax, tymin, tymax, tzmin, tzmax;

        Vector3 min = box.min;
        Vector3 max = box.max;

        float divx = 1.0f / direction.x;
        if (divx >= 0) {
            tmin = (min.x - origin.x) * divx;
            tmax = (max.x - origin.x) * divx;
        } else {
            tmin = (max.x - origin.x) * divx;
            tmax = (min.x - origin.x) * divx;
        }

        float divy = 1.0f / direction.y;
        if (divy >= 0) {
            tymin = (min.y - origin.y) * divy;
            tymax = (max.y - origin.y) * divy;
        } else {
            tymin = (max.y - origin.y) * divy;
            tymax = (min.y - origin.y) * divy;
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

        float divz = 1.0f / direction.z;
        if (divz >= 0) {
            tzmin = (min.z - origin.z) * divz;
            tzmax = (max.z - origin.z) * divz;
        } else {
            tzmin = (max.z - origin.z) * divz;
            tzmax = (min.z - origin.z) * divz;
        }

        if ((tmin > tzmax) || (tzmin > tmax)) {
            return false;
        }

        return true;
    }

}
