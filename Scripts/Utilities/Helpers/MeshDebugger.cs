using FPTemplate.Utilities;
using UnityEditor;
using UnityEngine;

public class MeshDebugger : ExtendedMonoBehaviour
{
    public MeshFilter MeshFilter => GetComponent<MeshFilter>();

    private void OnDrawGizmos()
    {
        if (!gameObject.activeInHierarchy || !enabled)
        {
            return;
        }
        Gizmos.matrix = transform.localToWorldMatrix;
        Handles.matrix = transform.localToWorldMatrix;
        var mesh = MeshFilter.sharedMesh;
        Gizmos.color = Color.blue;
        for (var i = 0; i < mesh.vertices.Length; i++)
        {
            var vertex = mesh.vertices[i];
            var normal = mesh.normals[i];
            Gizmos.DrawCube(vertex, Vector3.one * .1f);
            Gizmos.DrawLine(vertex, vertex + normal);
            Handles.Label(vertex + Vector3.up * .5f, $"{i}");
        }
        for (var i = 0; i < mesh.triangles.Length; i += 3)
        {
            var tri1 = mesh.triangles[i + 0];
            var tri2 = mesh.triangles[i + 1];
            var tri3 = mesh.triangles[i + 2];
            Gizmos.DrawLine(mesh.vertices[tri1], mesh.vertices[tri2]);
            Gizmos.DrawLine(mesh.vertices[tri2], mesh.vertices[tri3]);
            Gizmos.DrawLine(mesh.vertices[tri3], mesh.vertices[tri1]);

        }
    }
}
