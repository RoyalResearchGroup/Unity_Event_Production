using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class LineDrawer : MonoBehaviour
{
    private LineRenderer lineRenderer;

    void Awake()
    {
        //Initialize the LineRenderer component.
        lineRenderer = gameObject.GetComponent<LineRenderer>();
        //Set up the LineRenderer properties
        lineRenderer.widthMultiplier = 0.02f;
    }

    public void DrawLinesToSuccessors(List<GameObject> successors)
    {
        List<Vector3> points = new List<Vector3>();

        //Add the predecessor's position as the starting point.
        points.Add(transform.position);

        //Loop through each successor to add points.
        foreach (GameObject successor in successors)
        {
            if (successor != null)
            {
                points.Add(transform.position);
                points.Add(successor.transform.position + new Vector3(0,0,transform.position.z));
            }
        }

        //Set the points to the LineRenderer.
        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPositions(points.ToArray());
    }
}
