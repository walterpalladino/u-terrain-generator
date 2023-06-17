using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SegmentedPath : MonoBehaviour
{

    [SerializeField]
    private GameObject[] nodesGameObjects;

    [SerializeField]
    private GameObject[] pathPrefabs;

    [SerializeField]
    private GameObject segmentsParent;

    //[SerializeField]
    //int segmentCount = 32;

    [SerializeField]
    float distanceBetweenSegments = 2.0f;




    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    public void Generate()
    {
        Debug.Log("Generate Path");

        float rawPathLength = GetRawPathLength();
        Debug.Log("Raw Path Length : " + rawPathLength);


        int numberOfSegments = 1 + (int)(rawPathLength / distanceBetweenSegments);
        Debug.Log("Number of Segments : " + numberOfSegments);

        List<Vector3> segmentPoints = GetSegmentsPoints(numberOfSegments);
        //Debug.Log(segmentPoints.Count);

        for (int p = 0; p < numberOfSegments - 1; p++)
        {
            
            //            float angle = Vector3.Angle((bezierPoints[p] - bezierPoints[p - 1]).normalized, (bezierPoints[p + 1] - bezierPoints[p]).normalized);
            //float angle = Vector3.SignedAngle(bezierPoints[p].normalized, bezierPoints[p + 1].normalized, Vector3.up);
            float angle = Vector3.SignedAngle((segmentPoints[p + 1] - segmentPoints[p]), Vector3.right, Vector3.up);
            //Debug.Log("Angle : " + angle);
            Quaternion rotation = Quaternion.AngleAxis(90 - angle, Vector3.up);
            GameObject instance = Instantiate(pathPrefabs[0], segmentPoints[p], rotation);
            instance.transform.parent = segmentsParent.transform;
            
        }

    }

    public void Clear()
    {
        Debug.Log("Clear Path");


        if (Application.isPlaying)
        {

            foreach (Transform child in segmentsParent.transform)
            {
                GameObject.Destroy(child.gameObject);
            }

        }
        else
        {

            while (segmentsParent.transform.childCount != 0)
            {
                DestroyImmediate(segmentsParent.transform.GetChild(0).gameObject);
            }
        }

    }

    private void OnDrawGizmosSelected()
    {
        Color rawPathColor = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Color curvePathColor = new Color(1.0f, 1.0f, 0.0f, 0.35f);

        Gizmos.color = rawPathColor;

        for (int n = 0; n < nodesGameObjects.Length - 1; n++)
        {
            Gizmos.DrawLine(nodesGameObjects[n].transform.position, nodesGameObjects[n + 1].transform.position);
        }

        float rawPathLength = GetRawPathLength();
        int numberOfSegments = 1 + (int)(rawPathLength / distanceBetweenSegments);

        List<Vector3> points = GetSegmentsPoints(numberOfSegments);
        //DrawBezierPath(bezierPoints);

        Gizmos.color = curvePathColor;
        for (int p = 0; p < numberOfSegments - 1; p++)
        {
            Gizmos.DrawLine(points[p], points[p + 1]);
        }

    }

    private float GetRawPathLength()
    {
        float pathLength = 0.0f;
        for (int n = 0; n < nodesGameObjects.Length - 1; n++)
        {
            pathLength += Vector3.Distance(nodesGameObjects[n].transform.position, nodesGameObjects[n + 1].transform.position);
        }
        return pathLength;
    }

    /*
    private List<Vector3> GetBezierPoints(int segmentCount)
    {

        //  Create the bezier curve based on the actual path nodes
        List<Vector3> nodes = new List<Vector3>();
        foreach (GameObject go in nodesGameObjects)
        {
            nodes.Add(go.transform.position);
        }
        Bezier bezier = new Bezier(nodes);

        //  Get the bezier curve as points based on the segment count
        List<Vector3> bezierPoints = new List<Vector3>();
        int segment = 0;

        float segmentSize = 1.0f / segmentCount;

        while (segment < segmentCount)
        {
            bezierPoints.Add(bezier.PointAt(segment * segmentSize));
            segment++;
        }

        return bezierPoints;
    }
*/

    private List<Vector3> GetSegmentsPoints(int segmentCount)
    {

        //  Create the bezier curve based on the actual path nodes
        List<Vector3> nodes = new List<Vector3>();
        foreach (GameObject go in nodesGameObjects)
        {
            nodes.Add(go.transform.position);
        }

        List<Vector3> points = new List<Vector3>();

        SegmentedCurve curve = new SegmentedCurve(nodes);

        //  Get the bezier curve as points based on the segment count
        int segment = 0;

        float segmentSize = 1.0f / segmentCount;

        while (segment < segmentCount)
        {
            points.Add(curve.PointAt(segment * segmentSize));
            segment++;
        }
        
        return points;
    }

}
