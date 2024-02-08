using System.Numerics;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using aengine.ecs;
using Jitter.Dynamics;
using Jitter.LinearMath;

namespace aengine.core
{
    public struct aengine
    {
        public static int GLSL_VERSION = 330;
        public static string QUOTE = "\"";
        public static float MAX_SOUND_DISTANCE = 10;
        
        public static float deg2Rad(float degrees)
        {
            return degrees * (float) Math.PI / 180;
        }

        public static float rad2Deg(float radians)
        {
            return radians * 180 / (float) Math.PI;
        }
        
        public static Matrix4x4 bodyMatrix(RigidBody body)
        {
            JMatrix ori = body.Orientation;

            return new Matrix4x4(ori.M11, ori.M12, ori.M13, 0,
                ori.M21, ori.M22, ori.M23, 0.0f,
                ori.M31, ori.M32, ori.M33, 0.0f,
                0, 0, 0, 1.0f);
        }

        public static Vector3 QuaternionToEulerAngles(Quaternion q)
        {
            float roll = (float) Math.Atan2(2 * (q.Y * q.Z + q.W * q.X), q.W * q.W - q.X * q.X - q.Y * q.Y + q.Z * q.Z);
            float pitch = (float) Math.Asin(-2 * (q.X * q.Z - q.W * q.Y));
            float yaw = (float) Math.Atan2(2 * (q.X * q.Y + q.W * q.Z), q.W * q.W + q.X * q.X - q.Y * q.Y - q.Z * q.Z);
            
            return new Vector3(rad2Deg(roll), rad2Deg(pitch), rad2Deg(yaw));
        }

        public static Vector3 MatrixToEuler(JMatrix matrix)
        {
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
            
            return Vector3.Zero with {
                X = pitch,
                Y = yaw,
                Z = roll
            };
        }

        public static bool CheckCollisionAABB(AABB one, AABB two)
        {
            float oneMinX = one.x - one.width / 2;
            float oneMaxX = one.x + one.width / 2;
            float oneMinY = one.y - one.height / 2;
            float oneMaxY = one.y + one.height / 2;
            float oneMinZ = one.z - one.depth / 2;
            float oneMaxZ = one.z + one.depth / 2;

            float twoMinX = two.x - two.width / 2;
            float twoMaxX = two.x + two.width / 2;
            float twoMinY = two.y - two.height / 2;
            float twoMaxY = two.y + two.height / 2;
            float twoMinZ = two.z - two.depth / 2;
            float twoMaxZ = two.z + two.depth / 2;

            if (oneMinX <= twoMaxX && oneMaxX >= twoMinX &&
                oneMinY <= twoMaxY && oneMaxY >= twoMinY &&
                oneMinZ <= twoMaxZ && oneMaxZ >= twoMinZ) {
                return true; 
            }

            return false;  
        }

        public static Vector3 mullAddV3(Vector3 v1, Vector3 v2, float scalar)
        {
            return new Vector3(v1.X + v2.X * scalar, v1.Y + v2.Y * scalar, v1.Z + v2.Z * scalar);
        }

        public static JVector vecToJVec(Vector3 vec) {
            return JVector.Zero with {
                X = vec.X,
                Y = vec.Y,
                Z = vec.Z
            };
        }

        public static float getAspectRatio(Matrix4x4 matrix)
        {
            return matrix.M11 / matrix.M22;
        }
        
        public static string removeFromEnd(string str, int length)
        {
            if (str.Length < length)
            {
                return string.Empty;
            }

            return str[..^length];
        }

        public static float getRandomFloat(float min, float max)
        {
            Random r = new Random();
            return (float) r.Next((int)min, (int)max);
        }

        public static int getRandomInt(int min, int max)
        {
            Random r = new Random();
            return r.Next(min, max);
        }

        public static bool getRandomBool() {
            return System.Convert.ToBoolean(getRandomInt(0, 2));
        }

        public static float distance(Vector3 v1, Vector3 v2) {
            return MathF.Sqrt(MathF.Pow((v1.X - v2.X), 2) + MathF.Pow((v1.Y - v2.Y), 2) + MathF.Pow((v1.Z - v2.Z), 2));
        }

        public static string changeCharAtIndex(string str, int index, char @char) {
            if (index < 0 || index >= str.Length)
            {
                throw new IndexOutOfRangeException("Index is out of range");
            }

            char[] charArray = str.ToCharArray(); // Convert string to char array
            charArray[index] = @char; // Modify character at index
            return new string(charArray); // Convert char array back to string
        }

    }

}