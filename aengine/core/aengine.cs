using System.Numerics;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using aengine.ecs;
using Jitter.LinearMath;
using aengine.graphics;

namespace aengine.core
{
    public struct aengine
    {
        public static float deg2Rad(float degrees)
        {
            return degrees * (float) Math.PI / 180;
        }

        public static float rad2Deg(float radians)
        {
            return radians * 180 / (float) Math.PI;
        }

        public static Vector3 QuaternionToEulerAngles(Quaternion q, bool degrees)
        {
            float roll = (float) Math.Atan2(2 * (q.Y * q.Z + q.W * q.X), q.W * q.W - q.X * q.X - q.Y * q.Y + q.Z * q.Z);
            float pitch = (float) Math.Asin(-2 * (q.X * q.Z - q.W * q.Y));
            float yaw = (float) Math.Atan2(2 * (q.X * q.Y + q.W * q.Z), q.W * q.W + q.X * q.X - q.Y * q.Y - q.Z * q.Z);

            if (degrees)
                return new Vector3(rad2Deg(roll), rad2Deg(pitch), rad2Deg(yaw));
            else
                return new Vector3(roll, pitch, yaw);
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

            // Calculate Euler angles using ZYZ convention
            float roll = (float)Math.Atan2(m21, m11);
            float pitch = (float)Math.Acos(m33);
            float yaw = (float)Math.Atan2(-m32, m31);

            // Convert angles to degrees if needed
            roll = rad2Deg(roll);
            pitch = rad2Deg(pitch);
            yaw = rad2Deg(yaw);

            return new Vector3(roll, pitch, yaw);
        }

        public static OpenTK.Mathematics.Matrix4 MatrixRotateZYX(OpenTK.Mathematics.Vector3 rotationAngles)
        {
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

        public static float sineWave(float frequency = 0.5f, float amplitude = 1.0f)
        {
            double timeValue = graphics.Graphics.timer.Elapsed.TotalSeconds;
            float value = amplitude * (float) Math.Sin(timeValue * frequency) / 2.0f + 0.5f;
            return value;
        } 

        public static Vector2 MeasureText(Font font, string text, float fontSize)
        {
            // This function should calculate and return the width and height of the rendered text

            // For the sake of simplicity, let's assume each character has a fixed width and height
            // You may need to adjust this based on the actual font rendering library you are using
            float characterWidth = 0.1f * fontSize;
            float characterHeight = 0.2f * fontSize;

            // Calculate the width of the text by multiplying the number of characters with the character width
            float textWidth = text.Length * characterWidth;

            // The height of the text is just the character height
            float textHeight = characterHeight;

            // Return the calculated width and height as a Vector2
            return new Vector2(textWidth, textHeight);
        }

    }

}