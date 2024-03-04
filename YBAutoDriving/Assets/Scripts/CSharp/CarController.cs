using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class CarController : MonoBehaviour
{
    public Camera cam;
    public NavMeshAgent agent;

    public TMP_Text speed_indicator;
    public int Speed => (int)(3f * agent.velocity.magnitude);

    private NavMeshPath path;
    private int currentCorner = 0;
    public float speed = 10f;
    public float turnSpeed = 5f;
    public float timescale = 2f;

    public GameObject car; // Reference to the car's Rigidbody



    private LineRenderer lineRenderer;
    private Vector3[] pathPoints = new Vector3[0];


    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = timescale;

        agent.speed = speed;
        path = new NavMeshPath();
    }

    public Vector3 delta;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // Move agent
                //agent.SetDestination(hit.point);
                if (path != null)
                    path.ClearCorners();
                else
                    path = new NavMeshPath();
                currentCorner = 0;


                NavMesh.CalculatePath(transform.position, hit.point, NavMesh.AllAreas, path);
                //DrawCarPath(path.corners);
            }
        }

        if (path == null || path.corners.Length == 0)
            return;

        // Move towards the next corner
        Vector3 direction = (path.corners[currentCorner] - transform.position).normalized;
        delta = direction * speed * Time.deltaTime;
        transform.position += delta;

        // Rotate towards the next corner
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * turnSpeed);

        // Check if close to the current corner
        if (Vector3.Distance(transform.position, path.corners[currentCorner]) < 1f)
        {
            currentCorner++; // Move to the next corner
            if (currentCorner >= path.corners.Length)
            {
                // Arrived at destination
                path = null;
            }
        }

        speed_indicator.text = (100f * timescale * delta.magnitude).ToString("0");
    }

    private Vector3 lineoffset = new Vector3(0f, 100f, 0f);
    void DrawCarPath(Vector3[] points)
    {
        if (lineRenderer != null)
        {
            Destroy(lineRenderer);
        }
        lineRenderer = this.AddComponent<LineRenderer>();
        //lineRenderer.positionCount = 0;
        //lineRenderer.SetPositions(new Vector3[0]);


        foreach (Vector3 newPoint in points)
        {
            // Add the new point to the array of path points
            System.Array.Resize(ref pathPoints, pathPoints.Length + 1);
            pathPoints[pathPoints.Length - 1] = newPoint + lineoffset;
        }
        // Update the LineRenderer
        lineRenderer.positionCount = pathPoints.Length;
        lineRenderer.SetPositions(pathPoints);
    }


    // This function is called when the GameObject starts colliding with another GameObject
    private void OnCollisionEnter(Collision collision)
    {
        // Print the name of the object the GameObject starts colliding with
        //Debug.Log(gameObject.name + " has collided with " + collision.gameObject.name);
    }


}
