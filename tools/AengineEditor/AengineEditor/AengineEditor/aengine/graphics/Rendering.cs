using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Raylib_CsLo;
using static Raylib_CsLo.Raylib;
using static Raylib_CsLo.RlGl;

namespace aengine.graphics
{
    public class Rendering
    {
        private static float linePointSizeDivisor = 3; 
        public static void drawDebugAxies(float length = 4)
        {
            DrawLine3D(Vector3.Zero, new  Vector3(length, 0, 0), BLUE);
            DrawLine3D(Vector3.Zero, new  Vector3(0, length, 0), RED);
            DrawLine3D(Vector3.Zero, new  Vector3(0, 0, length), GREEN);
        }

        public static Color getRandomColor(int a = 255)
        {
            return new Color(GetRandomValue(-1, 256), GetRandomValue(-1, 256), GetRandomValue(-1, 256), a);
        }

        public static void drawArrow(Vector3 startPos, Vector3 endPos, Color color)
        {
            DrawLine3D(startPos, endPos, color);
            float cubeWidth = (endPos.X - startPos.X)/linePointSizeDivisor;
            float cubeHeight = (endPos.Y - startPos.Y)/linePointSizeDivisor;
            float cubeLength = (endPos.Z - startPos.Z)/linePointSizeDivisor;
            Vector3 cubePos = new Vector3(endPos.X + cubeWidth/2, endPos.Y + cubeHeight/2, endPos.Z + cubeLength/2);
            DrawCube(cubePos, cubeWidth, cubeHeight, cubeLength, color);
        }
        
        public static void drawSkyBox(Texture[] textures, Color tint, int scale = 200)
        {
            float fix = 0.5f;
            DrawCubeTexture(textures[0], new Vector3(0, 0, scale/2), scale, -scale, 0, tint);   // front
            DrawCubeTexture(textures[1], new Vector3(0, 0, -scale/2), scale, -scale, 0, tint);   // back
            DrawCubeTexture(textures[2], new Vector3(-scale/2, 0, 0), 0, -scale, scale, tint);   // left
            DrawCubeTexture(textures[3], new Vector3(scale/2, 0, 0), 0, -scale, scale, tint);   // right
            DrawCubeTexture(textures[4], new Vector3(0, scale/2, 0), -scale, 0, -scale, tint);     // top
            DrawCubeTexture(textures[5], new Vector3(0, -scale/2, 0), -scale, 0, scale, tint); // bottom
        }
        
        public static void drawSprite3D(Texture texture, Vector3 position, float width, float height, float rotation, float rotationY, Color color)
        {
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
            rlNormal3f(0.0f, 0.0f, 1.0f);       // Normal Pointing Towards Viewer
            rlTexCoord2f(1.0f, 1.0f); rlVertex3f(x - width/2, y - height/2, z);  
            rlTexCoord2f(0.0f, 1.0f); rlVertex3f(x + width/2, y - height/2, z);  
            rlTexCoord2f(0.0f, 0.0f); rlVertex3f(x + width/2, y + height/2, z);  
            rlTexCoord2f(1.0f, 0.0f); rlVertex3f(x - width/2, y + height/2, z);  

            rlNormal3f(0.0f, 0.0f, 1.0f);       // Normal Pointing Towards Viewer
            rlTexCoord2f(1.0f, 1.0f); rlVertex3f(x + width/2, y - height/2, z); 
            rlTexCoord2f(0.0f, 1.0f); rlVertex3f(x - width/2, y - height/2, z); 
            rlTexCoord2f(0.0f, 0.0f); rlVertex3f(x - width/2, y + height/2, z); 
            rlTexCoord2f(1.0f, 0.0f); rlVertex3f(x + width/2, y + height/2, z); 
            rlEnd();
            rlPopMatrix();

            rlSetTexture(0);
        }

        public static void drawTexturedPlane(Texture texture, Vector3 position, float width, float depth,
            float rotation, Color color)
        {
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
            rlNormal3f(0.0f, 1.0f, 0.0f);       // Normal Pointing Up
            rlTexCoord2f(0.0f, 1.0f); rlVertex3f(x - width/2, y, z - depth/2);  // Top Left Of The Texture and Quad
            rlTexCoord2f(0.0f, 0.0f); rlVertex3f(x - width/2, y, z + depth/2);  // Bottom Left Of The Texture and Quad
            rlTexCoord2f(1.0f, 0.0f); rlVertex3f(x + width/2, y, z + depth/2);  // Bottom Right Of The Texture and Quad
            rlTexCoord2f(1.0f, 1.0f); rlVertex3f(x + width/2, y, z - depth/2);  // Top Right Of The Texture and Quad
            // Bottom Face
            rlNormal3f(0.0f, - 1.0f, 0.0f);     // Normal Pointing Down
            rlTexCoord2f(1.0f, 1.0f); rlVertex3f(x - width/2, y, z - depth/2);  // Top Right Of The Texture and Quad
            rlTexCoord2f(0.0f, 1.0f); rlVertex3f(x + width/2, y, z - depth/2);  // Top Left Of The Texture and Quad
            rlTexCoord2f(0.0f, 0.0f); rlVertex3f(x + width/2, y, z + depth/2);  // Bottom Left Of The Texture and Quad
            rlTexCoord2f(1.0f, 0.0f); rlVertex3f(x - width/2, y, z + depth/2);  // Bottom Right Of The Texture and Quad
            rlEnd();
            rlPopMatrix();

            rlSetTexture(0);
        }

        public static void drawCrosshair(Color color, float size = 25, int thickness = 2)
        {
            DrawLineEx(new Vector2(GetScreenWidth()/2 - size/2, GetScreenHeight()/2), new Vector2(GetScreenWidth()/2 + size/2, GetScreenHeight()/2), thickness, color);
            DrawLineEx(new Vector2(GetScreenWidth()/2, GetScreenHeight()/2 - size/2), new Vector2(GetScreenWidth()/2, GetScreenHeight()/2 + size/2), thickness, color);
        }
        
        public static unsafe Mesh genMeshCapsule(float radius, float height, int rings, int slices)
        {
            Mesh mesh = new Mesh();

            int numVertices = (rings + 1) * (slices + 1);
            int numIndices = rings * slices * 6;

            mesh.vertexCount = numVertices;
            mesh.triangleCount = numIndices / 3;

            mesh.vertices = (float*)MemAlloc((uint)(numVertices * 3 * sizeof(float)));
            mesh.texcoords = (float*)MemAlloc((uint)(numVertices * 2 * sizeof(float)));
            mesh.indices = (ushort*)MemAlloc((uint)(numIndices * sizeof(ushort)));

            int vertexIndex = 0;
            int indexIndex = 0;

            for (int i = 0; i <= rings; i++)
            {
                for (int j = 0; j <= slices; j++)
                {
                    float theta = (float)(i * Math.PI / rings);
                    float phi = (float)(j * 2 * Math.PI / slices);

                    float x = (float)(Math.Sin(theta) * Math.Cos(phi));
                    float y = (float)(Math.Cos(theta));
                    float z = (float)(Math.Sin(theta) * Math.Sin(phi));

                    mesh.vertices[vertexIndex] = x * radius;
                    mesh.vertices[vertexIndex + 1] = y * radius + height * 0.5f;
                    mesh.vertices[vertexIndex + 2] = z * radius;

                    mesh.texcoords[vertexIndex] = (float)j / slices;
                    mesh.texcoords[vertexIndex + 1] = (float)i / rings;

                    if (i < rings && j < slices)
                    {
                        mesh.indices[indexIndex] = (ushort)vertexIndex;
                        mesh.indices[indexIndex + 1] = (ushort)(vertexIndex + slices + 1);
                        mesh.indices[indexIndex + 2] = (ushort)(vertexIndex + slices);

                        mesh.indices[indexIndex + 3] = (ushort)(vertexIndex + slices + 1);
                        mesh.indices[indexIndex + 4] = (ushort)vertexIndex;
                        mesh.indices[indexIndex + 5] = (ushort)(vertexIndex + 1);

                        indexIndex += 6;
                    }

                    vertexIndex += 3;
                }
            }

            UploadMesh(&mesh, false);

            return mesh;
        }


    }   
}