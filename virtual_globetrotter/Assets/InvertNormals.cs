using UnityEngine;

public class InvertNormals : MonoBehaviour
{
    void Start()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Vector3[] normals = mesh.normals;

        for (int i = 0; i < normals.Length; i++)
        {
            normals[i] = -normals[i]; // Invert the normals
        }

        mesh.normals = normals;

        // Reverse the winding order of triangles
        for (int i = 0; i < mesh.subMeshCount; i++)
        {
            int[] triangles = mesh.GetTriangles(i);
            for (int j = 0; j < triangles.Length; j += 3)
            {
                int temp = triangles[j];
                triangles[j] = triangles[j + 1];
                triangles[j + 1] = temp;
            }
            mesh.SetTriangles(triangles, i);
        }
    }
}
