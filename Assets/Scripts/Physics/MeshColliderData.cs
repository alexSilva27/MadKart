//MIT License

//Copyright (c) 2023 - Alexandre Silva

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.

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