
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Patroller : MonoBehaviour
{
    private static readonly int RoamStateHash = Animator.StringToHash("Base Layer.TankerRoam");
    
    [Header("Movement Settings")]
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float arrivalDistance = 0.1f;
    [SerializeField] private bool loop = true;
    [SerializeField] private Animator tankerAnimator;

    private Rigidbody2D rb;
    [SerializeField] private int currentWaypointIndex = 0;
    [SerializeField] private bool isMoving = true;

    void Start()
    {
        // Cache the Rigidbody2D component
        rb = GetComponent<Rigidbody2D>();

        // Validation check
        if (waypoints == null || waypoints.Length == 0)
        {
            Debug.LogWarning($"No waypoints assigned to {gameObject.name}.", this);
            isMoving = false;
        }
    }

    void Update()
    {
        // 1. Get info about the current state on Layer 0 (Base Layer)
        AnimatorStateInfo stateInfo = tankerAnimator.GetCurrentAnimatorStateInfo(0);

        isMoving = stateInfo.fullPathHash == RoamStateHash;
    }

    void FixedUpdate()
    {
        if (!isMoving) return;

        MoveTowardsWaypoint();
    }

    private void MoveTowardsWaypoint()
    {
        // Get target position
        Vector2 targetPosition = waypoints[currentWaypointIndex].position;
        
        // Calculate step based on fixed time to keep physics consistent
        Vector2 newPosition = Vector2.MoveTowards(rb.position, targetPosition, speed * Time.fixedDeltaTime);
        
        // Move the Rigidbody2D physically
        rb.MovePosition(newPosition);

        // Check if the object has arrived at the current waypoint
        if (Vector2.Distance(rb.position, targetPosition) <= arrivalDistance)
            UpdateToNextWaypoint();
    }

    private void UpdateToNextWaypoint()
    {
        Debug.Log("Update to the next waypoint");
        currentWaypointIndex++;

        // Handle loop logic when reaching the end of the array
        if (currentWaypointIndex >= waypoints.Length)
        {
            if (loop)
            {
                currentWaypointIndex = 0;
            }
            else
            {
                isMoving = false; // Stop moving if looping is disabled
            }
        }
    }
}

