using UnityEngine;

namespace MadKart
{
    public struct MeshColliderData
    {
        public readonly TriangleCollider[] Triangles; // triangles defined in world space...

        public MeshColliderData(MeshCollider meshCollider)
        {
            Mesh mesh = meshCollider.sharedMesh;
            Transform meshTransform = meshCollider.gameObject.transform;
            int[] triangles = mesh.triangles;
            Vector3[] vertices = mesh.vertices;

            Triangles = new TriangleCollider[triangles.Length / 3];

            for (int i = 0; i < triangles.Length; i += 3)
            {
                Vector3 vertex0 = meshTransform.TransformPoint(vertices[triangles[i]]);
                Vector3 vertex1 = meshTransform.TransformPoint(vertices[triangles[i + 1]]);
                Vector3 vertex2 = meshTransform.TransformPoint(vertices[triangles[i + 2]]);

                Triangles[i/3] = new TriangleCollider(vertex0, vertex1, vertex2);
            }
        }

        public Vector3 ClosestPoint(in Vector3 point)
        {
            Vector3 closestPoint = default;
            float smallestDistance = float.MaxValue;

            foreach (TriangleCollider triangle in Triangles)
            {
                Vector3 closestPointInTriangle = triangle.ClosestPoint(in point);
                float distanceToTriangle = (closestPointInTriangle - point).magnitude;
                if (distanceToTriangle < smallestDistance)
                {
                    smallestDistance = distanceToTriangle;
                    closestPoint = closestPointInTriangle;
                }
            }

            return closestPoint;
        }
    }
}