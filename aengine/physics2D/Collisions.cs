using System.Numerics;

namespace aengine.physics2D; 

public static class Collisions {
    public static bool checkCollisionAABB(PhysicsAABB a, PhysicsAABB b) {
        if(a.max.X <= b.min.X || b.max.X <= a.min.X ||
           a.max.Y <= b.min.Y || b.max.Y <= a.min.Y)
        {
            return false;
        }

        return true;
    }

    public static void findContactPoints(PhysicsBody bodyA, PhysicsBody bodyB, out Vector2 contactOne,
        out Vector2 contactTwo, out int contactCount) {
        contactOne = Vector2.Zero;
        contactTwo = Vector2.Zero;
        contactCount = 0;

        PhysicsShape shapeTypeA = bodyA.shape;
        PhysicsShape shapeTypeB = bodyB.shape;

        if (shapeTypeA is PhysicsShape.BOX) {
            if (shapeTypeB is PhysicsShape.BOX) {
            }
            else if (shapeTypeB is PhysicsShape.CIRCLE) {
                findContactPoint(bodyB.getPosition(), bodyB.radius, bodyA.getPosition(), bodyA.getTransformedVertices(),
                    out contactOne);
                contactCount = 1;
            }
        }
        else if (shapeTypeA is PhysicsShape.CIRCLE) {
            if (shapeTypeB is PhysicsShape.BOX) {
                findContactPoint(bodyA.getPosition(), bodyA.radius, bodyB.getPosition(), bodyB.getTransformedVertices(),
                    out contactOne);
                contactCount = 1;
            }
            else if (shapeTypeB is PhysicsShape.CIRCLE) {
                findContactPoint(bodyA.getPosition(), bodyA.radius, bodyB.getPosition(), out contactOne);
                contactCount = 1;
            }
        }
    }

    private static void findContactPoint(Vector2 circleCenter, float circleRadius, Vector2 polygonCenter,
        Vector2[] polygonVertices, out Vector2 cp) {
        cp = Vector2.Zero;

        float minDistSq = float.MaxValue;

        for (int i = 0; i < polygonVertices.Length; i++) {
            Vector2 va = polygonVertices[i];
            Vector2 vb = polygonVertices[(i + 1) % polygonVertices.Length];

            pointSegmentDistance(circleCenter, va, vb, out float distSq, out Vector2 contact);

            if (distSq < minDistSq) {
                minDistSq = distSq;
                cp = contact;
            }
        }
    }
    
    private static void findContactPoint(Vector2 centerA, float radiusA, Vector2 centerB, out Vector2 cp) {
        Vector2 ab = centerB - centerA;
        Vector2 dir = Vector2.Normalize(ab);
        cp = centerA + dir * radiusA;
    }
    
    private static void pointSegmentDistance(Vector2 p, Vector2 a, Vector2 b, out float distanceSquared, out Vector2 cp) {
        Vector2 ab = b - a;
        Vector2 ap = p - a;

        float proj = Vector2.Dot(ap, ab);
        float abLenSq = PhysicsUtils.lengthSquared(ab);
        float d = proj / abLenSq;

        if (d <= 0f) {
            cp = a;
        } else if (d >= 1.0f) {
            cp = b;
        } else {
            cp = a + ab * d;
        }

        distanceSquared = Vector2.DistanceSquared(p, cp);
    }
    
    internal static bool collide(PhysicsBody bodyA, PhysicsBody bodyB, out Vector2 normal, out float depth) {
        normal = Vector2.Zero;
        depth = 0;

        PhysicsShape physicsShapeA = bodyA.shape;
        PhysicsShape physicsShapeB = bodyB.shape;

        if (physicsShapeA is PhysicsShape.BOX) {
            if (physicsShapeB is PhysicsShape.BOX) {
                return checkPolyOverlap(bodyA.getPosition(), bodyA.getTransformedVertices(), bodyB.getPosition(), bodyB.getTransformedVertices(),
                    out normal, out depth);
            }
            
            if (physicsShapeB is PhysicsShape.CIRCLE) {
                bool result = checkPolyCircleOverlap(bodyB.getPosition(), bodyB.radius, bodyA.getPosition(),
                    bodyA.getTransformedVertices(), out normal, out depth);

                normal = -normal;
                return result;
            }
        } else if (physicsShapeA is PhysicsShape.CIRCLE) {
            if (physicsShapeB is PhysicsShape.BOX) {
                return checkPolyCircleOverlap(bodyA.getPosition(), bodyA.radius, bodyB.getPosition(),
                    bodyB.getTransformedVertices(), out normal, out depth);
            }
            
            if (physicsShapeB is PhysicsShape.CIRCLE) {
                return checkCircleOverlap(bodyA.getPosition(), bodyA.radius, bodyB.getPosition(),
                    bodyB.radius, out normal, out depth);
            }
        }

        return false;
    }
    
    public static bool checkPolyCircleOverlap(Vector2 circleCenter, float circleRadius, Vector2 polyCenter, Vector2[] vertices,
        out Vector2 normal, out float depth) {
        normal = Vector2.Zero;
        depth = float.MaxValue;
        
        Vector2 axis = Vector2.Zero;
        float axisDepth;

        float minA;
        float maxA;
        float minB;
        float maxB;
        
        for (int i = 0; i < vertices.Length; i++) {
            Vector2 va = vertices[i];
            Vector2 vb = vertices[(i + 1) % vertices.Length];

            Vector2 edge = vb - va;
            axis = Vector2.Zero with { X = -edge.Y, Y = edge.X }; 
            axis = Vector2.Normalize(axis);
            
            projectVertices(vertices, axis, out minA, out maxA);
            projectCircle(circleCenter, circleRadius, axis, out minB, out maxB);

            if (minA >= maxB || minB >= maxA) // seperated
                return false;

            axisDepth = MathF.Min(maxB - minA, maxA - minB);

            if (axisDepth < depth) {
                depth = axisDepth;
                normal = axis;
            }
        }

        int cpIndex = findClosestPointOnPoly(circleCenter, vertices);
        Vector2 cp = vertices[cpIndex];

        axis = cp - circleCenter;
        axis = Vector2.Normalize(axis);
        
        projectVertices(vertices, axis, out minA, out maxA);
        projectCircle(circleCenter, circleRadius, axis, out minB, out maxB);

        if (minA >= maxB || minB >= maxA) // seperated
            return false;

        axisDepth = MathF.Min(maxB - minA, maxA - minB);

        if (axisDepth < depth) {
            depth = axisDepth;
            normal = axis;
        }

        Vector2 dir = polyCenter - circleCenter;

        if (Vector2.Dot(dir, normal) < 0) {
            normal = -normal;
        }
        
        return true;
    }

    private static int findClosestPointOnPoly(Vector2 circleCenter, Vector2[] vertices) {
        int result = -1;
        float minDistance = float.MaxValue;
        
        for (var i = 0; i < vertices.Length; i++) {
            Vector2 v = vertices[i];
            float distance = Vector2.Distance(v, circleCenter);

            if (distance < minDistance) {
                minDistance = distance;
                result = i;
            }
        }

        return result;
    }
    
    private static void projectCircle(Vector2 center, float radius, Vector2 axis, out float min, out float max) {
        Vector2 dir = Vector2.Normalize(axis);
        Vector2 dirAndRadius = dir * radius;

        Vector2 p1 = center + dirAndRadius;
        Vector2 p2 = center - dirAndRadius;

        min = Vector2.Dot(p1, axis);
        max = Vector2.Dot(p2, axis);

        if (min > max) {
            (min, max) = (max, min);
        }
    }
    
    public static bool checkPolyOverlap(Vector2 centerA, Vector2[] verticesA, Vector2 centerB, Vector2[] verticesB, out Vector2 normal, out float depth) {
        normal = Vector2.Zero;
        depth = float.MaxValue;
        
        for (int i = 0; i < verticesA.Length; i++) {
            Vector2 va = verticesA[i];
            Vector2 vb = verticesB[(i + 1) % verticesA.Length];

            Vector2 edge = vb - va;
            Vector2 axis = Vector2.Zero with { X = -edge.Y, Y = edge.X };
            axis = Vector2.Normalize(axis);
            
            projectVertices(verticesA, axis, out float minA, out float maxA);
            projectVertices(verticesB, axis, out float minB, out float maxB);

            if (minA >= maxB || minB >= maxA) // seperated
                return false;

            float axisDepth = MathF.Min(maxB - minA, maxA - minB);

            if (axisDepth < depth) {
                depth = axisDepth;
                normal = axis;
            }
        }
        
        for (int i = 0; i < verticesB.Length; i++) {
            Vector2 va = verticesB[i];
            Vector2 vb = verticesB[(i + 1) % verticesB.Length];

            Vector2 edge = vb - va;
            Vector2 axis = Vector2.Zero with { X = -edge.Y, Y = edge.X };
            axis = Vector2.Normalize(axis);
            
            projectVertices(verticesA, axis, out float minA, out float maxA);
            projectVertices(verticesB, axis, out float minB, out float maxB);

            if (minA >= maxB || minB >= maxA) // seperated
                return false;
            
            float axisDepth = MathF.Min(maxB - minA, maxA - minB);

            if (axisDepth < depth) {
                depth = axisDepth;
                normal = axis;
            }
        }

        Vector2 dir = centerB - centerA;

        if (Vector2.Dot(dir, normal) < 0) {
            normal = -normal;
        }
        
        return true;
    }
    
    private static void projectVertices(Vector2[] vertices, Vector2 axis, out float min, out float max) {
        min = float.MaxValue;
        max = float.MinValue;

        for (int i = 0; i < vertices.Length; i++) {
            Vector2 v = vertices[i];
            float proj = Vector2.Dot(v, axis);

            if (proj < min)
                min = proj;

            if (proj > max)
                max = proj;
        }
    }
    
    public static bool checkCircleOverlap(Vector2 centerA, float radiusA, Vector2 centerB, float radiusB, out Vector2 normal, out float depth) {
        normal = Vector2.Zero;
        depth = 0f;

        float distance = Vector2.Distance(centerA, centerB);
        float radii = radiusA + radiusB;

        if(distance >= radii) {
            return false;
        }

        normal = Vector2.Normalize(centerB - centerA);
        depth = radii - distance;

        return true;
    }
}