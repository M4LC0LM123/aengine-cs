using System.Numerics;
using Raylib_CsLo;

namespace aengine.ecs;

public class ParticleSpawn
{
    public static Vector3 circle(float radius)
    {

        Random random = new Random();
        double angle = random.NextDouble() * 2 * Math.PI;

        // Generate a random radius between 0 and the given circle radius
        // To ensure uniform distribution within the circle, take the square root of the random value
        double randomRadius = Math.Sqrt(random.NextDouble()) * radius;

        // Convert polar coordinates to Cartesian coordinates (x, y)
        double x = randomRadius * Math.Cos(angle);
        double y = randomRadius * Math.Sin(angle);

        return new Vector3((float) x, 0, (float) y);
    }
    
    public static Vector3 sphere(float radius)
    {

        Random random = new Random();
        double polarAngle = random.NextDouble() * Math.PI;

        // Generate a random azimuthal angle φ between 0 and 2*pi (360 degrees)
        double azimuthalAngle = random.NextDouble() * 2 * Math.PI;

        // Generate a random radius between 0 and the given sphere radius
        // To ensure uniform distribution within the sphere, take the cube root of the random value
        double randomRadius = Math.Pow(random.NextDouble(), 1.0 / 3.0) * radius;

        // Convert spherical coordinates to Cartesian coordinates (x, y, z)
        double x = randomRadius * Math.Sin(polarAngle) * Math.Cos(azimuthalAngle);
        double y = randomRadius * Math.Sin(polarAngle) * Math.Sin(azimuthalAngle);
        double z = randomRadius * Math.Cos(polarAngle);

        return new Vector3((float) x, (float) y, (float) z);
    }

    public static Vector3 cube(float width, float height, float depth)
    {
        return new Vector3(core.aengine.getRandomFloat(-width / 2, width / 2),
            core.aengine.getRandomFloat(-height / 2, height / 2), core.aengine.getRandomFloat(-depth / 2, depth / 2));
    }
    
}