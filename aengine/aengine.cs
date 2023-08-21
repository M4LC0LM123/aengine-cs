using System.Numerics;
using Jitter.LinearMath;

namespace aengine;

public struct aengine {
    public static int GLSL_VERSION = 330;
    public static string QUOTE = "\"";
    public static float MAX_SOUND_DISTANCE = 10;

    public static float deg2Rad(float degrees) {
        return degrees * (float)Math.PI / 180;
    }

    public static float rad2Deg(float radians) {
        return radians * 180 / (float)Math.PI;
    }

    public static Vector3 QuaternionToEulerAngles(Quaternion q) {
        float roll = (float)Math.Atan2(2 * (q.Y * q.Z + q.W * q.X), q.W * q.W - q.X * q.X - q.Y * q.Y + q.Z * q.Z);
        float pitch = (float)Math.Asin(-2 * (q.X * q.Z - q.W * q.Y));
        float yaw = (float)Math.Atan2(2 * (q.X * q.Y + q.W * q.Z), q.W * q.W + q.X * q.X - q.Y * q.Y - q.Z * q.Z);

        return new Vector3(rad2Deg(roll), rad2Deg(pitch), rad2Deg(yaw));
    }

    public static Vector3 MatrixToEuler(JMatrix matrix) {
        // Extract individual elements from the rotation matrix
        float m11 = matrix.M11;
        float m12 = matrix.M12;
        float m13 = matrix.M13;
        float m21 = matrix.M21;
        float m22 = matrix.M22;
        float m23 = matrix.M23;
        float m31 = matrix.M31;
        float m32 = matrix.M32;
        float m33 = matrix.M33;

        // Calculate Euler angles using XYZ convention
        float pitch = (float)Math.Atan2(-m23, Math.Sqrt(m13 * m13 + m33 * m33));
        float yaw = (float)Math.Atan2(m13, m33);
        float roll = (float)Math.Atan2(m21, m22);

        // Convert angles to degrees if needed
        pitch = rad2Deg(pitch);
        yaw = rad2Deg(yaw);
        roll = rad2Deg(roll);

        return new Vector3(pitch, yaw, roll);
    }
    
    public static OpenTK.Mathematics.Matrix4 MatrixRotateZYX(OpenTK.Mathematics.Vector3 rotationAngles) {
        float cosZ = MathF.Cos(rotationAngles.Z);
        float sinZ = MathF.Sin(rotationAngles.Z);
        float cosY = MathF.Cos(rotationAngles.Y);
        float sinY = MathF.Sin(rotationAngles.Y);
        float cosX = MathF.Cos(rotationAngles.X);
        float sinX = MathF.Sin(rotationAngles.X);

        OpenTK.Mathematics.Matrix4 matrix = new OpenTK.Mathematics.Matrix4();

        matrix.M11 = cosZ * cosY;
        matrix.M12 = sinZ * cosY;
        matrix.M13 = -sinY;
        matrix.M14 = 0.0f;

        matrix.M21 = cosZ * sinY * sinX - sinZ * cosX;
        matrix.M22 = sinZ * sinY * sinX + cosZ * cosX;
        matrix.M23 = cosY * sinX;
        matrix.M24 = 0.0f;

        matrix.M31 = cosZ * sinY * cosX + sinZ * sinX;
        matrix.M32 = sinZ * sinY * cosX - cosZ * sinX;
        matrix.M33 = cosY * cosX;
        matrix.M34 = 0.0f;

        matrix.M41 = 0.0f;
        matrix.M42 = 0.0f;
        matrix.M43 = 0.0f;
        matrix.M44 = 1.0f;

        return matrix;
    }

    public static Vector3 mullAddV3(Vector3 v1, Vector3 v2, float scalar) {
        return new Vector3(v1.X + v2.X * scalar, v1.Y + v2.Y * scalar, v1.Z + v2.Z * scalar);
    }

    public static float getAspectRatio(Matrix4x4 matrix) {
        return matrix.M11 / matrix.M22;
    }

    public static string removeFromEnd(string str, int length) {
        if (str.Length < length) {
            return string.Empty;
        }

        return str[..^length];
    }

    public static float getRandomFloat(float min, float max) {
        Random r = new Random();
        return (float)r.Next((int)min, (int)max);
    }

    public static int getRandomInt(int min, int max) {
        Random r = new Random();
        return r.Next(min, max);
    }
}