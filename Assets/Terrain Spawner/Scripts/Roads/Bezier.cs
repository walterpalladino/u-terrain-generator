using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bezier
{

    private List<Vector3> nodes;


    public Bezier(List<Vector3> nodes)
    {
        this.nodes = nodes;
    }

    
    public Vector3 PointAt(float t)
    {
        return PointAt(nodes[0], nodes[1], nodes[2], nodes[3], t);
    }
    
    /*
    public Vector3 PointAt(float t)
    {
        Vector3 result = Vector3.zero;

        if (nodes.Count < 3)
        {
            return Vector3.zero;
        }

        result = nodes[0];
        for (int n = 1; n < nodes.Count - 1; n ++)
        {
            result = Vector3.Lerp(result, PointAt(nodes[n], nodes[n + 1], t), t);
        }

        return result;
    }
    */

    public Vector3 PointAt(Vector3 a, Vector3 b, float t)
    {
        return Vector3.Lerp(a, b, t);
    }

    public Vector3 PointAt(Vector3 a, Vector3 b, Vector3 c, float t)
    {
        return Vector3.Lerp(PointAt(a, b, t), PointAt(b, c, t), t);
    }

    Vector3 PointAt(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float t)
    {
        return Vector3.Lerp(PointAt(a, b, c, t), PointAt(b, c, d, t), t);
    }

}
