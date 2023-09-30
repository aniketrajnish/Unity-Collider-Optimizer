// ref - https://www.codeproject.com/Articles/18936/A-C-Implementation-of-Douglas-Peucker-Line-Appro

using System.Collections.Generic;
using UnityEngine;

public class DouglasPeuckerReduction : MonoBehaviour
{
    public static List<Vector2> DouglasPeuckerReductionPoints(List<Vector2> points, float tolerance)
    {
        if (points == null || points.Count < 3)
            return points;

        int firstPoint = 0;
        int lastPoint = points.Count - 1;
        List<int> pointIndicesToKeep = new List<int> { firstPoint, lastPoint };
       
        while (points[firstPoint].Equals(points[lastPoint]))        
            lastPoint--;        

        DouglasPeuckerReductionRecursive(points, firstPoint, lastPoint, tolerance, ref pointIndicesToKeep);
        pointIndicesToKeep.Sort();

        List<Vector2> returnPoints = new List<Vector2>();
        
        foreach (int index in pointIndicesToKeep)
        {
            returnPoints.Add(points[index]);
        }

        return returnPoints;
    }

    private static void DouglasPeuckerReductionRecursive(List<Vector2> points, int firstPoint, int lastPoint, float tolerance, ref List<int> pointIndicesToKeep)
    {
        float maxDistance = 0;
        int indexFarthest = 0;

        for (int index = firstPoint; index < lastPoint; index++)
        {
            float distance = PerpendicularDistance(points[firstPoint], points[lastPoint], points[index]);
            if (distance > maxDistance)
            {
                maxDistance = distance;
                indexFarthest = index;
            }
        }

        if (maxDistance > tolerance && indexFarthest != 0)
        {
            pointIndicesToKeep.Add(indexFarthest);

            DouglasPeuckerReductionRecursive(points, firstPoint, indexFarthest, tolerance, ref pointIndicesToKeep);
            DouglasPeuckerReductionRecursive(points, indexFarthest, lastPoint, tolerance, ref pointIndicesToKeep);
        }
    }

    public static float PerpendicularDistance(Vector2 point1, Vector2 point2, Vector2 point)
    {
        float area = Vector3.Cross(point1 - point, point2 - point).magnitude * 0.5f;
        float bottom = Vector3.Distance(point1, point2);
        float height = area / bottom * 2f;

        return height;
    }

}

