using UnityEngine;
using Pathfinding;

public class ClickToMover : MonoBehaviour
{
    public Animator animator;
    //The point to move to
    public Vector3 targetPosition;
    private Seeker seeker;
    private CharacterController controller;
    //The calculated path
    public Path path;
    //The AI's speed per second
    public float speed = 100;
    //The max distance from the AI to a waypoint for it to continue to the next waypoint
    public float nextWaypointDistance = 3;
    //The waypoint we are currently moving towards
    private int currentWaypoint = 0;
    public float rotationSpeed = 360;

    // Use this for initialization
    void Start ()
	{
	    seeker = GetComponent<Seeker>();
        seeker.pathCallback += PathCallback;
        controller = GetComponent<CharacterController>();
	}

    public void PathCallback(Path newPath)
    {
        Debug.Log("Yay, we got a path back. Did it have an error? " + newPath.error);
        if (!newPath.error)
        {
            //Reset the waypoint counter
            currentWaypoint = 0;

            path = newPath;
            animator.SetBool("Moving", true);
        }
    }

    // Update is called once per frame
    void Update () {
        if (path == null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit))
                {
                    seeker.StartPath(transform.position, hit.point);
                }

            }

            //We have no path to move after yet
            return;
        }

        if (currentWaypoint >= path.vectorPath.Count)
        {
            Debug.Log("End Of Path Reached");
            animator.SetBool("Moving", false);
            path = null;
            return;
        }
        //Direction to the next waypoint
        Vector3 dir = (path.vectorPath[currentWaypoint] - transform.position).normalized;
        //transform.LookAt(path.vectorPath[currentWaypoint]);
        // Rotate towards the target
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), rotationSpeed * Time.deltaTime);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);

        dir *= speed * Time.deltaTime;
        controller.SimpleMove(dir);
        //Check if we are close enough to the next waypoint
        //If we are, proceed to follow the next waypoint
        if (Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]) < nextWaypointDistance)
        {
            currentWaypoint++;
            return;
        }
    }
}
