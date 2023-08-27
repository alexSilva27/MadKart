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