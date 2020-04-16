using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathFunctions : MonoBehaviour
{
    //Moviment accelerat exponencialment
    public static Vector3 EaseInExp(float t, Vector3 origin, Vector3 dest, float duration, int e)
    {
        t /= duration;
        return (dest - origin) * Mathf.Pow(t, e) + origin;
    }

    //Accelerar float exponencialment
    public static float EaseInExp(float t, float vStart, float vEnd, float duration, int e)
    {
        t /= duration;
        return (vEnd - vStart) * Mathf.Pow(t, e) + vStart;
    }

    //Moviment desaccelerat exponencialment
    public static Vector3 EaseOutExp(float t, Vector3 origin, Vector3 dest, float duration, int e)
    {
        t /= duration;
        t--;
        return (dest - origin) * (Mathf.Pow(t, e) + 1) + origin;
    }

    //Desaccelerar float exponencialment
    public static float EaseOutExp(float t, float vStart, float vEnd, float duration, int e)
    {
        t /= duration;
        t--;
        return (vEnd - vStart) * (Mathf.Pow(t, e) + 1) + vStart;
    }

    //Retorna un index aleatori dins d'una llista de probabilitats
    //Retorna -1 en cas d'error
    public static int RandomProbability(List<float> weightList)
    {
        int index = -1;
        float rand = Random.Range(0, 10000) / 10000f;
        float sum = 0;

        for (int i = 0; i < weightList.Count; i++)
        {
            sum += weightList[i];
            if (sum >= rand)
            {
                index = i;
                break;
            }
        }
        return index;
    }

    /*
    * Aproxima la direcció "dir" a la direcció en 90º més propera, en forma d'enter 0-3
    * 0 -> Dreta
    * 1 -> Amunt
    * 2 -> Esquerra
    * 3 -> Abaix
    */
    public static int GetDirection(Vector2 dir)
    {
        if (Mathf.Abs(dir.x) >= Mathf.Abs(dir.y))
        {
            if (dir.x >= 0)
            {
                return 0;
            }
            else
            {
                return 2;
            }
        }
        else
        {
            if (dir.y >= 0)
            {
                return 1;
            }
            else
            {
                return 3;
            }
        }
    }


    public static float DistanceToLine(Ray ray, Vector3 point)
    {
        return Vector3.Cross(ray.direction, point - ray.origin).magnitude;
    }

    //This function returns a point which is a projection from a point to a line.
    //The line is regarded infinite. If the line is finite, use ProjectPointOnLineSegment() instead.
    public static Vector3 ProjectPointOnLine(Vector3 linePoint, Vector3 lineVec, Vector3 point)
    {

        //get vector from point on line to point in space
        Vector3 linePointToPoint = point - linePoint;

        float t = Vector3.Dot(linePointToPoint, lineVec);

        return linePoint + lineVec * t;
    }

    //This function returns a point which is a projection from a point to a line segment.
    //If the projected point lies outside of the line segment, the projected point will 
    //be clamped to the appropriate line edge.
    //If the line is infinite instead of a segment, use ProjectPointOnLine() instead.
    public static Vector3 ProjectPointOnLineSegment(Vector3 linePoint1, Vector3 linePoint2, Vector3 point)
    {

        Vector3 vector = linePoint2 - linePoint1;

        Vector3 projectedPoint = ProjectPointOnLine(linePoint1, vector.normalized, point);

        int side = PointOnWhichSideOfLineSegment(linePoint1, linePoint2, projectedPoint);

        //The projected point is on the line segment
        if (side == 0)
        {

            return projectedPoint;
        }

        if (side == 1)
        {

            return linePoint1;
        }

        if (side == 2)
        {

            return linePoint2;
        }

        //output is invalid
        return Vector3.zero;
    }

    //This function returnsthe side of the point. 0-> inside, 1-> linePoint1, 2->linePoint2
    public static int ProjectPointOnLineSegmentSide(Vector3 linePoint1, Vector3 linePoint2, Vector3 point)
    {

        Vector3 vector = linePoint2 - linePoint1;

        Vector3 projectedPoint = ProjectPointOnLine(linePoint1, vector.normalized, point);

        int side = PointOnWhichSideOfLineSegment(linePoint1, linePoint2, projectedPoint);

        //The projected point is on the line segment
        return side;
    }

    //This function finds out on which side of a line segment the point is located.
    //The point is assumed to be on a line created by linePoint1 and linePoint2. If the point is not on
    //the line segment, project it on the line using ProjectPointOnLine() first.
    //Returns 0 if point is on the line segment.
    //Returns 1 if point is outside of the line segment and located on the side of linePoint1.
    //Returns 2 if point is outside of the line segment and located on the side of linePoint2.
    public static int PointOnWhichSideOfLineSegment(Vector3 linePoint1, Vector3 linePoint2, Vector3 point)
    {

        Vector3 lineVec = linePoint2 - linePoint1;
        Vector3 pointVec = point - linePoint1;

        float dot = Vector3.Dot(pointVec, lineVec);

        //point is on side of linePoint2, compared to linePoint1
        if (dot > 0)
        {

            //point is on the line segment
            if (pointVec.magnitude <= lineVec.magnitude)
            {

                return 0;
            }

            //point is not on the line segment and it is on the side of linePoint2
            else
            {

                return 2;
            }
        }

        //Point is not on side of linePoint2, compared to linePoint1.
        //Point is not on the line segment and it is on the side of linePoint1.
        else
        {

            return 1;
        }
    }

    //arrayToCurve is original Vector3 array, smoothness is the number of interpolations. 
    public static Vector3[] MakeSmoothCurve(Vector3[] arrayToCurve, float smoothness)
    {
        List<Vector3> points;
        List<Vector3> curvedPoints;
        int pointsLength = 0;
        int curvedLength = 0;

        if (smoothness < 1.0f) smoothness = 1.0f;

        pointsLength = arrayToCurve.Length;

        curvedLength = (pointsLength * Mathf.RoundToInt(smoothness)) - 1;
        curvedPoints = new List<Vector3>(curvedLength);

        float t = 0.0f;
        for (int pointInTimeOnCurve = 0; pointInTimeOnCurve < curvedLength + 1; pointInTimeOnCurve++)
        {
            t = Mathf.InverseLerp(0, curvedLength, pointInTimeOnCurve);

            points = new List<Vector3>(arrayToCurve);

            for (int j = pointsLength - 1; j > 0; j--)
            {
                for (int i = 0; i < j; i++)
                {
                    points[i] = (1 - t) * points[i] + t * points[i + 1];
                }
            }

            curvedPoints.Add(points[0]);
        }

        return (curvedPoints.ToArray());
    }

    public static Vector2 CalculateMiddlePoint(Vector2 aStart, Vector2 aEnd, float aTime, Vector2 aPoint)
    {
        float t = aTime;
        float rt = 1f - t;
        return 0.5f * (aPoint - rt * rt * aStart - t * t * aEnd) / (t * rt);
    }

    public static Vector3 VectorMiddlePoint(Vector3 start, Vector3 end)
    {
        return start + (end - start) / 2;
    }

    public static Vector3 PerpendicularVector(Vector3 v, float magnitude, int side)
    {
        
        if(side == 0)
        {
            return new Vector2(-v.y,v.x) / Mathf.Sqrt(Mathf.Pow(v.x, 2) + Mathf.Pow(v.y, 2)) * magnitude;
        }
        else
        {
            return new Vector2(-v.y, v.x) / Mathf.Sqrt(Mathf.Pow(v.x, 2) + Mathf.Pow(v.y, 2)) * - magnitude;
        }
    }

    //This returns a negative number if B is left of A, positive if right of A, or 0 if they are perfectly aligned.
    public static float AngleDir(Vector2 A, Vector2 B)
    {
        return -A.x * B.y + A.y * B.x;
    }
}
