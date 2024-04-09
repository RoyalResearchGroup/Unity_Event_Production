using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class LineDrawer : MonoBehaviour
{
    private LineRenderer lineRenderer;

    void Awake()
    {
        // Initialize the LineRenderer component.
        lineRenderer = gameObject.GetComponent<LineRenderer>();
        // Set up the LineRenderer properties, such as materials, width, etc.
        // lineRenderer.material = ...
        lineRenderer.widthMultiplier = 0.02f;
    }

    public void DrawLinesToSuccessors(List<GameObject> successors)
    {
        // Assuming that there is one predecessor and it alternates between successors.
        List<Vector3> points = new List<Vector3>();

        // Add the predecessor's position as the starting point.
        points.Add(transform.position);

        // Loop through each successor to add points.
        foreach (GameObject successor in successors)
        {
            if (successor != null)
            {
                // Add the predecessor point before every successor to make an alternating pattern.
                points.Add(transform.position);
                points.Add(successor.transform.position + new Vector3(0,0,transform.position.z));
            }
        }

        // Set the points to the LineRenderer.
        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPositions(points.ToArray());
    }
}
