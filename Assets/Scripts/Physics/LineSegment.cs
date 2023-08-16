using UnityEngine;

namespace MadKart
{
    public struct LineSegment
    {
        public readonly Vector3 Origin;
        public readonly Vector3 End;

        public LineSegment(Vector3 origin, Vector3 end)
        {
            Origin = origin;
            End = end;
        }

        // it is expected that pointInLineSegment belongs to the supporting line of the lineSegment.
        public float GetCoefficientUnclamped(in Vector3 pointInLineSegment)
        {
            Vector3 scaledDirection = End - Origin;

			float xAbs = Mathf.Abs(scaledDirection.x);
			float yAbs = Mathf.Abs(scaledDirection.y);
			float zAbs = Mathf.Abs(scaledDirection.z);

			if (xAbs > yAbs && xAbs > zAbs)
			{
				// use X...
				return (pointInLineSegment.x - Origin.x) / scaledDirection.x;
			}
			else if (yAbs > zAbs)
			{
				// use Y...
				return (pointInLineSegment.y - Origin.y) / scaledDirection.y;
			}
			else
			{
				// use Z...
				return (pointInLineSegment.z - Origin.z) / scaledDirection.z;
			}
		}

        public Vector3 ClosestPoint(in Vector3 point)
        {
            Vector3 projectedPointInLine = Origin + Vector3.Project(point - Origin, (End - Origin).normalized);

			// is the projected point between Origin and End?
			float coefficientInLineSegment = GetCoefficientUnclamped(in projectedPointInLine);

			if (coefficientInLineSegment >= 0f && coefficientInLineSegment <= 1f)
            {
				return projectedPointInLine;
            }
			else if (coefficientInLineSegment < 0f)
            {
				return Origin;
            }
			else
            {
				return End;
            }
		}
    }
}