using UnityEngine;

namespace MadKart
{
    public struct TriangleCollider
    {
        public readonly Vector3 Point0, Point1, Point2;
        public readonly Plane TrianglePlane; // cached for more performance...
        public readonly Matrix4x4 TransformationMatrix; // cached for more performance...

        public TriangleCollider(Vector3 point0, Vector3 point1, Vector3 point2)
        {
            Point0 = point0;
            Point1 = point1;
            Point2 = point2;

            TrianglePlane = new(Point0, Point1, Point2);

            // use triangle barycentric coordinates to find out if any point belongs to the triangle...
            // x ∗ (P1−P0) + y ∗ (P2−P0) + z * planeNormal = P−P0
            Vector4 column0 = Point1 - Point0;
            Vector4 column1 = Point2 - Point0;
            Vector4 column2 = TrianglePlane.normal;
            Vector4 column3 = new(0f, 0f, 0f, 1f);
            TransformationMatrix = new Matrix4x4(column0, column1, column2, column3).inverse;
        }

        public Vector3 ClosestPoint(in Vector3 point)
        {
            Vector3 projectedPointInPlane = TrianglePlane.ClosestPointOnPlane(point);
            Vector4 barycentricCoords = TransformationMatrix * (projectedPointInPlane - Point0);

            if (barycentricCoords.x >= 0 &&
                barycentricCoords.y >= 0 &&
                barycentricCoords.x + barycentricCoords.y <= 1)
            {
                return projectedPointInPlane;
            }
            else
            {
                LineSegment segment0 = new(Point0, Point1);
                LineSegment segment1 = new(Point0, Point2);
                LineSegment segment2 = new(Point1, Point2);

                Vector3 closestPointToSegment0 = segment0.ClosestPoint(in point);
                Vector3 closestPointToSegment1 = segment1.ClosestPoint(in point);
                Vector3 closestPointToSegment2 = segment2.ClosestPoint(in point);

                float distanceToSegment0 = (point - closestPointToSegment0).magnitude;
                float distanceToSegment1 = (point - closestPointToSegment1).magnitude;
                float distanceToSegment2 = (point - closestPointToSegment2).magnitude;

                if (distanceToSegment0 < distanceToSegment1 && distanceToSegment0 < distanceToSegment2)
                {
                    return closestPointToSegment0;
                }
                else if (distanceToSegment1 < distanceToSegment2)
                {
                    return closestPointToSegment1;
                }
                else
                {
                    return closestPointToSegment2;
                }
            }
        }
    }
}