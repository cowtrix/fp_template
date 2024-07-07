using UnityEngine;

namespace FPTemplate.Utilities.Extensions
{
    public static class MeshExtensions
    {
        public static Mesh CreateQuadMesh(Vector2 size, Vector3 normal)
        {
            Mesh mesh = new Mesh();

            Vector3[] vertices = new Vector3[4];
            int[] triangles = new int[6];
            Vector3[] normals = new Vector3[4];
            Vector2[] uv = new Vector2[4];

            // Calculate the right and up vectors based on the normal
            Vector3 right = Vector3.Cross(normal, Vector3.up);
            if (right == Vector3.zero)
            {
                right = Vector3.Cross(normal, Vector3.right);
            }
            Vector3 up = Vector3.Cross(right, normal);

            // Define the vertices in local space
            vertices[0] = -right * size.x * 0.5f - up * size.y * 0.5f;
            vertices[1] = right * size.x * 0.5f - up * size.y * 0.5f;
            vertices[2] = right * size.x * 0.5f + up * size.y * 0.5f;
            vertices[3] = -right * size.x * 0.5f + up * size.y * 0.5f;

            // Define the triangles
            triangles[0] = 0;
            triangles[1] = 2;
            triangles[2] = 1;
            triangles[3] = 0;
            triangles[4] = 3;
            triangles[5] = 2;

            // Define the normals
            for (int i = 0; i < normals.Length; i++)
            {
                normals[i] = normal;
            }

            // Define the UVs
            uv[0] = new Vector2(0, 0);
            uv[1] = new Vector2(1, 0);
            uv[2] = new Vector2(1, 1);
            uv[3] = new Vector2(0, 1);

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.normals = normals;
            mesh.uv = uv;

            return mesh;
        }
    }
}