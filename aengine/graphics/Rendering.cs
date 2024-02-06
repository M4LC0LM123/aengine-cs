using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using aengine_cs.aengine.windowing;
using aengine.ecs;
using Raylib_CsLo;
using static Raylib_CsLo.Raylib;
using static Raylib_CsLo.RlGl;
using static Raylib_CsLo.RayMath;

namespace aengine.graphics {
    public class Rendering {
        private static float linePointSizeDivisor = 3;

        public static void drawDebugAxies(float length = 4) {
            DrawLine3D(Vector3.Zero, new Vector3(length, 0, 0), BLUE);
            DrawLine3D(Vector3.Zero, new Vector3(0, length, 0), RED);
            DrawLine3D(Vector3.Zero, new Vector3(0, 0, length), GREEN);
        }

        public static Color getRandomColor(int a = 255) {
            return new Color(GetRandomValue(-1, 256), GetRandomValue(-1, 256), GetRandomValue(-1, 256), a);
        }

        public static void drawArrow(Vector3 startPos, Vector3 endPos, Color color) {
            DrawLine3D(startPos, endPos, color);
            float cubeWidth = (endPos.X - startPos.X) / linePointSizeDivisor;
            float cubeHeight = (endPos.Y - startPos.Y) / linePointSizeDivisor;
            float cubeLength = (endPos.Z - startPos.Z) / linePointSizeDivisor;
            Vector3 cubePos = new Vector3(endPos.X + cubeWidth / 2, endPos.Y + cubeHeight / 2,
                endPos.Z + cubeLength / 2);
            DrawCube(cubePos, cubeWidth, cubeHeight, cubeLength, color);
        }

        public static void drawSkyBox(Texture[] textures, Color tint, int scale = 200) {
            float fix = 0.5f;
            rlPushMatrix();
            rlTranslatef(World.camera.position.X, World.camera.position.Y, World.camera.position.Z);
            DrawCubeTexture(textures[0], new Vector3(0, 0, scale * 0.5f), scale, -scale, 0, tint); // front
            DrawCubeTexture(textures[1], new Vector3(0, 0, -scale * 0.5f), scale, -scale, 0, tint); // back
            DrawCubeTexture(textures[2], new Vector3(-scale * 0.5f, 0, 0), 0, -scale, scale, tint); // left
            DrawCubeTexture(textures[3], new Vector3(scale * 0.5f, 0, 0), 0, -scale, scale, tint); // right
            DrawCubeTexture(textures[4], new Vector3(0, scale * 0.5f, 0), -scale, 0, -scale, tint); // top
            DrawCubeTexture(textures[5], new Vector3(0, -scale * 0.5f, 0), -scale, 0, scale, tint); // bottom
            rlPopMatrix();
        }

        public static void drawSprite3D(Texture texture, Vector3 position, float width, float height, float rotation,
            float rotationY, Color color) {
            float x = position.X;
            float y = position.Y;
            float z = position.Z;

            rlSetTexture(texture.id);

            rlPushMatrix();
            rlTranslatef(x, y, z);
            rlRotatef(rotation, 0.0f, 1.0f, 0.0f);
            rlRotatef(rotationY, 1.0f, 0.0f, 0.0f);
            rlTranslatef(-x, -y, -z);

            rlBegin(RL_QUADS);
            rlColor4ub(color.r, color.g, color.b, color.a);
            rlNormal3f(0.0f, 0.0f, 1.0f); // Normal Pointing Towards Viewer
            rlTexCoord2f(1.0f, 1.0f);
            rlVertex3f(x - width / 2, y - height / 2, z);
            rlTexCoord2f(0.0f, 1.0f);
            rlVertex3f(x + width / 2, y - height / 2, z);
            rlTexCoord2f(0.0f, 0.0f);
            rlVertex3f(x + width / 2, y + height / 2, z);
            rlTexCoord2f(1.0f, 0.0f);
            rlVertex3f(x - width / 2, y + height / 2, z);

            rlNormal3f(0.0f, 0.0f, 1.0f); // Normal Pointing Towards Viewer
            rlTexCoord2f(1.0f, 1.0f);
            rlVertex3f(x + width / 2, y - height / 2, z);
            rlTexCoord2f(0.0f, 1.0f);
            rlVertex3f(x - width / 2, y - height / 2, z);
            rlTexCoord2f(0.0f, 0.0f);
            rlVertex3f(x - width / 2, y + height / 2, z);
            rlTexCoord2f(1.0f, 0.0f);
            rlVertex3f(x + width / 2, y + height / 2, z);
            rlEnd();
            rlPopMatrix();

            rlSetTexture(0);
        }

        public static void drawCubeWireframe(Vector3 position, Vector3 rotation, Vector3 scale, Color color) {
            rlPushMatrix();

            rlTranslatef(position.X, position.Y, position.Z);
            rlRotatef(rotation.X, 1, 0, 0);
            rlRotatef(rotation.Y, 0, 1, 0);
            rlRotatef(rotation.Z, 0, 0, 1);
            rlScalef(scale.X, scale.Y, scale.Z);

            rlBegin(RL_LINES);
            rlColor4ub(color.r, color.g, color.b, color.a);

            // botom face
            rlVertex3f(-0.5f, -0.5f, -0.5f);
            rlVertex3f(0.5f, -0.5f, -0.5f);

            rlVertex3f(0.5f, -0.5f, -0.5f);
            rlVertex3f(0.5f, -0.5f, 0.5f);

            rlVertex3f(0.5f, -0.5f, 0.5f);
            rlVertex3f(-0.5f, -0.5f, 0.5f);

            rlVertex3f(-0.5f, -0.5f, 0.5f);
            rlVertex3f(-0.5f, -0.5f, -0.5f);

            // Top face
            rlVertex3f(-0.5f, 0.5f, -0.5f);
            rlVertex3f(0.5f, 0.5f, -0.5f);

            rlVertex3f(0.5f, 0.5f, -0.5f);
            rlVertex3f(0.5f, 0.5f, 0.5f);

            rlVertex3f(0.5f, 0.5f, 0.5f);
            rlVertex3f(-0.5f, 0.5f, 0.5f);

            rlVertex3f(-0.5f, 0.5f, 0.5f);
            rlVertex3f(-0.5f, 0.5f, -0.5f);

            // Connecting lines
            rlVertex3f(-0.5f, -0.5f, -0.5f);
            rlVertex3f(-0.5f, 0.5f, -0.5f);

            rlVertex3f(0.5f, -0.5f, -0.5f);
            rlVertex3f(0.5f, 0.5f, -0.5f);

            rlVertex3f(0.5f, -0.5f, 0.5f);
            rlVertex3f(0.5f, 0.5f, 0.5f);

            rlVertex3f(-0.5f, -0.5f, 0.5f);
            rlVertex3f(-0.5f, 0.5f, 0.5f);

            rlEnd();

            rlPopMatrix();
        }

        public static void drawTexturedPlane(Texture texture, Vector3 position, float width, float depth,
            float rotation, Color color) {
            float x = position.X;
            float y = position.Y;
            float z = position.Z;

            rlSetTexture(texture.id);

            rlPushMatrix();
            rlTranslatef(x, y, z);
            rlRotatef(rotation, 0.0f, 1.0f, 0.0f);
            rlTranslatef(-x, -y, -z);

            rlBegin(RL_QUADS);
            rlColor4ub(color.r, color.g, color.b, color.a);
            // Top Face
            rlNormal3f(0.0f, 1.0f, 0.0f); // Normal Pointing Up
            rlTexCoord2f(0.0f, 1.0f);
            rlVertex3f(x - width / 2, y, z - depth / 2); // Top Left Of The Texture and Quad
            rlTexCoord2f(0.0f, 0.0f);
            rlVertex3f(x - width / 2, y, z + depth / 2); // Bottom Left Of The Texture and Quad
            rlTexCoord2f(1.0f, 0.0f);
            rlVertex3f(x + width / 2, y, z + depth / 2); // Bottom Right Of The Texture and Quad
            rlTexCoord2f(1.0f, 1.0f);
            rlVertex3f(x + width / 2, y, z - depth / 2); // Top Right Of The Texture and Quad
            // Bottom Face
            rlNormal3f(0.0f, -1.0f, 0.0f); // Normal Pointing Down
            rlTexCoord2f(1.0f, 1.0f);
            rlVertex3f(x - width / 2, y, z - depth / 2); // Top Right Of The Texture and Quad
            rlTexCoord2f(0.0f, 1.0f);
            rlVertex3f(x + width / 2, y, z - depth / 2); // Top Left Of The Texture and Quad
            rlTexCoord2f(0.0f, 0.0f);
            rlVertex3f(x + width / 2, y, z + depth / 2); // Bottom Left Of The Texture and Quad
            rlTexCoord2f(1.0f, 0.0f);
            rlVertex3f(x - width / 2, y, z + depth / 2); // Bottom Right Of The Texture and Quad
            rlEnd();
            rlPopMatrix();

            rlSetTexture(0);
        }

        public static void drawCrosshair(Color color, float size = 25, int thickness = 2) {
            DrawLineEx(new Vector2(Window.renderWidth / 2 - size / 2, Window.renderHeight / 2),
                new Vector2(Window.renderWidth / 2 + size / 2, Window.renderHeight / 2), thickness, color);
            DrawLineEx(new Vector2(Window.renderWidth / 2, Window.renderHeight / 2 - size / 2),
                new Vector2(Window.renderWidth / 2, Window.renderHeight / 2 + size / 2), thickness, color);
        }

        public static unsafe void allocateMeshData(Mesh* mesh, int triangleCount) {
            mesh->vertexCount = triangleCount * 3;
            mesh->triangleCount = triangleCount;

            mesh->vertices = (float*)MemAlloc((uint)(mesh->vertexCount * 3 * sizeof(float)));
            mesh->texcoords = (float*)MemAlloc((uint)(mesh->vertexCount * 2 * sizeof(float)));
            mesh->normals = (float*)MemAlloc((uint)(mesh->vertexCount * 3 * sizeof(float)));
        }

        private static unsafe Mesh genMeshCapsule(Vector3 startPos, Vector3 endPos, float radius, int slices,
            int rings) {
            Mesh mesh = new();
            int verticalSegments = rings * 2;
            int horizontalSegments = slices;

            allocateMeshData(&mesh, verticalSegments * horizontalSegments * 2);

            Vector3 direction = Vector3Subtract(endPos, startPos);
            Vector3 b0 = Vector3Normalize(direction);
            Vector3 b1 = Vector3Normalize(Vector3CrossProduct(b0, Vector3.UnitY));
            Vector3 b2 = Vector3Normalize(Vector3CrossProduct(b1, b0));
            Vector3 capCenter = endPos;

            float baseSliceAngle = (2.0f * PI) / slices;
            float baseRingAngle = PI * 0.5f / rings;

            int vertexIndex = 0;

            for (int c = 0; c < 2; c++) {
                for (int i = 0; i < rings; i++) {
                    for (int j = 0; j < slices; j++) {
                        float ringSin1 = MathF.Sin(baseSliceAngle * j) * MathF.Cos(baseRingAngle * i);
                        float ringCos1 = MathF.Cos(baseSliceAngle * j) * MathF.Cos(baseRingAngle * i);
                        Vector3 w1 = Vector3Add(Vector3Add(Vector3Scale(b0, capCenter.X), Vector3Scale(b1, ringSin1)),
                            Vector3Scale(b2, ringCos1));
                        w1 = Vector3Scale(w1, radius);

                        float ringSin2 = MathF.Sin(baseSliceAngle * (j + 1)) * MathF.Cos(baseRingAngle * i);
                        float ringCos2 = MathF.Cos(baseSliceAngle * (j + 1)) * MathF.Cos(baseRingAngle * i);
                        Vector3 w2 = Vector3Add(Vector3Add(Vector3Scale(b0, capCenter.X), Vector3Scale(b1, ringSin2)),
                            Vector3Scale(b2, ringCos2));
                        w2 = Vector3Scale(w2, radius);

                        float ringSin3 = MathF.Sin(baseSliceAngle * j) * MathF.Cos(baseRingAngle * (i + 1));
                        float ringCos3 = MathF.Cos(baseSliceAngle * j) * MathF.Cos(baseRingAngle * (i + 1));
                        Vector3 w3 = Vector3Add(Vector3Add(Vector3Scale(b0, capCenter.X), Vector3Scale(b1, ringSin3)),
                            Vector3Scale(b2, ringCos3));
                        w3 = Vector3Scale(w3, radius);

                        float ringSin4 = MathF.Sin(baseSliceAngle * (j + 1)) * MathF.Cos(baseRingAngle * (i + 1));
                        float ringCos4 = MathF.Cos(baseSliceAngle * (j + 1)) * MathF.Cos(baseRingAngle * (i + 1));
                        Vector3 w4 = Vector3Add(Vector3Add(Vector3Scale(b0, capCenter.X), Vector3Scale(b1, ringSin4)),
                            Vector3Scale(b2, ringCos4));
                        w4 = Vector3Scale(w4, radius);

                        // Make sure cap triangle normals are facing outwards
                        if (c == 0) {
                            mesh.vertices[vertexIndex * 3] = w1.X;
                            mesh.vertices[vertexIndex * 3 + 1] = w1.Y;
                            mesh.vertices[vertexIndex * 3 + 2] = w1.Z;

                            mesh.vertices[(vertexIndex + 1) * 3] = w2.X;
                            mesh.vertices[(vertexIndex + 1) * 3 + 1] = w2.Y;
                            mesh.vertices[(vertexIndex + 1) * 3 + 2] = w2.Z;

                            mesh.vertices[(vertexIndex + 2) * 3] = w3.X;
                            mesh.vertices[(vertexIndex + 2) * 3 + 1] = w3.Y;
                            mesh.vertices[(vertexIndex + 2) * 3 + 2] = w3.Z;

                            mesh.vertices[(vertexIndex + 3) * 3] = w2.X;
                            mesh.vertices[(vertexIndex + 3) * 3 + 1] = w2.Y;
                            mesh.vertices[(vertexIndex + 3) * 3 + 2] = w2.Z;

                            mesh.vertices[(vertexIndex + 4) * 3] = w4.X;
                            mesh.vertices[(vertexIndex + 4) * 3 + 1] = w4.Y;
                            mesh.vertices[(vertexIndex + 4) * 3 + 2] = w4.Z;

                            vertexIndex += 6;
                        }
                        else {
                            mesh.vertices[vertexIndex * 3] = w1.X;
                            mesh.vertices[vertexIndex * 3 + 1] = w1.Y;
                            mesh.vertices[vertexIndex * 3 + 2] = w1.Z;

                            mesh.vertices[(vertexIndex + 1) * 3] = w3.X;
                            mesh.vertices[(vertexIndex + 1) * 3 + 1] = w3.Y;
                            mesh.vertices[(vertexIndex + 1) * 3 + 2] = w3.Z;

                            mesh.vertices[(vertexIndex + 2) * 3] = w2.X;
                            mesh.vertices[(vertexIndex + 2) * 3 + 1] = w2.Y;
                            mesh.vertices[(vertexIndex + 2) * 3 + 2] = w2.Z;

                            mesh.vertices[(vertexIndex + 3) * 3] = w2.X;
                            mesh.vertices[(vertexIndex + 3) * 3 + 1] = w2.Y;
                            mesh.vertices[(vertexIndex + 3) * 3 + 2] = w2.Z;

                            mesh.vertices[(vertexIndex + 4) * 3] = w3.X;
                            mesh.vertices[(vertexIndex + 4) * 3 + 1] = w3.Y;
                            mesh.vertices[(vertexIndex + 4) * 3 + 2] = w3.Z;

                            mesh.vertices[(vertexIndex + 5) * 3] = w4.X;
                            mesh.vertices[(vertexIndex + 5) * 3 + 1] = w4.Y;
                            mesh.vertices[(vertexIndex + 5) * 3 + 2] = w4.Z;

                            vertexIndex += 6;
                        }
                    }
                }

                capCenter = startPos;
                b0 = Vector3Scale(b0, -1.0f);
            }

            UploadMesh(&mesh, false);

            return mesh;
        }

        public static Mesh genMeshCapsule(float radius, float height, int rings, int slices) {
            return genMeshCapsule(Vector3.Zero, Vector3AddValue(Vector3.Zero, height), radius, rings, slices);
        }
    }
}