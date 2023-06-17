using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct SegmentData
{
    public Vector3 start;
    public Vector3 end;
    public float distance;

    public float weight;

    public float segmentLengthStart;
    public float segmentLengthEnd;

    public override string ToString()
    {
        return "Start: " + start + " / End: " + end + " / Distance:" + distance + " / Weight: " + weight + " / Segment Length Start: " + segmentLengthStart + " / Segment Length End: " + segmentLengthEnd;
    }
}

public class SegmentedCurve
{
    private List<Vector3> nodes;
    private List<SegmentData> data;
    private float segmentedCurveLength;


    public SegmentedCurve(List<Vector3> nodes)
    {
        this.nodes = nodes;
        GenerateSegmentData();
    }

    private void GenerateSegmentData()
    {
        //Debug.Log("GenerateSegmentData");

        segmentedCurveLength = 0.0f;
        data = new List<SegmentData>();

        //data.Add(newSegmentData);

        for (int n = 1; n < nodes.Count; n++)
        {
            SegmentData newSegmentData = new SegmentData();
            newSegmentData = new SegmentData();

            float distance = Vector3.Distance(nodes[n - 1], nodes[n]);

            newSegmentData.start = nodes[n - 1];
            newSegmentData.end = nodes[n];
            newSegmentData.distance = distance;

            newSegmentData.segmentLengthStart = segmentedCurveLength;

            segmentedCurveLength += distance;

            newSegmentData.segmentLengthEnd = segmentedCurveLength;

            data.Add(newSegmentData);
        }

        for (int n = 0;  n < data.Count; n++)
        {
            SegmentData segmentData = data[n];

            segmentData.segmentLengthStart = segmentData.segmentLengthStart / segmentedCurveLength;
            segmentData.segmentLengthEnd = segmentData.segmentLengthEnd / segmentedCurveLength;

            data[n] = segmentData;

            //Debug.Log(data[n]);
        }

        //Debug.Log("segmentedCurveLength : " + segmentedCurveLength);

    }


    public Vector3 PointAt(float t)
    {
        Vector3 point = Vector3.zero;

        if (t < 0.0f || t > 1.0f)
        {
            return point;
        }

        int n;
        //  Find the segment for t
        for (n = 0; n < data.Count; n++)
        {
            if (t >= data[n].segmentLengthStart && t <= data[n].segmentLengthEnd)
            {

                float segmentLerp = t - data[n].segmentLengthStart;
                segmentLerp = segmentLerp / (data[n].segmentLengthEnd - data[n].segmentLengthStart) ;

                //Debug.Log("PointAt : " + t + " at element : " + n + " segmentLerp: " + segmentLerp);

                point = Vector3.Lerp(data[n].start, data[n].end, segmentLerp);

                return point;
            }
        }

        return point;
    }

}
