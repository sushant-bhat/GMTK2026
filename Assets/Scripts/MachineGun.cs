using System.Collections;
using UnityEngine;

public class MachineGun : MonoBehaviour
{
    private static readonly int DeathHash = Animator.StringToHash("death");
    private static readonly WaitForSeconds _waitForSeconds1 = new(0.2f);
    [Header("Animation")]
    [SerializeField] private Animator mgAnimator;
    
    [Header("Targets")]
    [SerializeField] private Transform gun;
    [SerializeField] private float detectionRadius = 25f;
    [SerializeField] private LayerMask soldierLayer;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject bulletPrefab;
    
    [Header("Visual Effects")]
    [SerializeField] private LineRenderer tracerLineRenderer; // Assign a LineRenderer here
    [SerializeField] private float tracerDuration = 0.1f;    // How long the flash stays on screen


    // Animation parameter hashes for performance
    private static readonly int IsShootingHash = Animator.StringToHash("isShooting");

    private float soldierTimer = 10f;
    private bool isAlive;
    private Coroutine activeFireRoutine;

    void Awake()
    {
        Events.BULLET_SHOT_EVENT.AddListener(OnBulletShot);
        isAlive = true;
    }

    private void OnBulletShot(EntityId id)
    {
        if (gameObject.GetEntityId().Equals(id))
        {
            Debug.Log("Machine gunner shot");
            StopCoroutine(activeFireRoutine);
            mgAnimator.SetTrigger(DeathHash);
            isAlive = false;
        }
    }

    void Update()
    {
        if (!isAlive)
        {
            return;
        }
        HandleTimers();
    }

    private void HandleTimers()
    {
        soldierTimer += Time.deltaTime;
        
        if (soldierTimer >= 10f)
        {
            soldierTimer = 0f;
            ShootClosestSoldier();
        }
    }

    private void ShootClosestSoldier()
    {
        Transform closestSoldier = FindClosestSoldier();

        if (closestSoldier != null)
        {
            activeFireRoutine = StartCoroutine(FireWeaponRoutine(closestSoldier.position));
        }
    }

    private Transform FindClosestSoldier()
    {
        // Find all 2D colliders within radius matching the soldier layer
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius, soldierLayer);
        Transform bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        foreach (Collider2D collider in hitColliders)
        {
            if (!collider.CompareTag("Soldier")) continue; 

            Vector3 directionToTarget = collider.transform.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;

            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = collider.transform;
            }
        }

        return bestTarget;
    }

    private IEnumerator FireWeaponRoutine(Vector3 targetPosition)
    {
        // 1. Aim at target in 2D space (Calculates angle along the Z axis)
        Vector3 direction = targetPosition - gun.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        gun.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        // 2. Trigger shoot animation
        mgAnimator.SetBool(IsShootingHash, true);

        for (int i = 0; i < 15; i++)
        {
            // Execute the shot logic
            ShootRaycast(targetPosition);

            // Wait for the specified gap before the next bullet
            yield return _waitForSeconds1;
        }

        // 5. Reset to crouch state
        mgAnimator.SetBool(IsShootingHash, false);
    }

    private void ShootRaycast(Vector3 targetPosition)
    {
        // Use the firePoint position if assigned, otherwise fall back to the gun's position
        Vector2 origin = firePoint != null ? (Vector2)firePoint.position : (Vector2)gun.position;
        
        // Calculate the direction from the origin to the target position
        Vector2 fireDirection = ((Vector2)targetPosition - origin).normalized;

        // Determine where the visual tracer line should end
        Vector2 endPoint;

        // Perform the single instant hit Raycast
        RaycastHit2D hit = Physics2D.Raycast(origin, fireDirection, detectionRadius, soldierLayer);

        // Draws a yellow debug ray in the Unity Scene view for testing
        Debug.DrawRay(origin, fireDirection * detectionRadius, Color.yellow, 0.1f);

        if (hit.collider != null && hit.collider.CompareTag("Soldier"))
        {
            endPoint = hit.point;
            Events.BULLET_SHOT_EVENT.Invoke(hit.collider.transform.parent.gameObject.GetEntityId());
        }
        else
        {
            endPoint = origin + (fireDirection * detectionRadius);
        }

        // Trigger the visual flash trajectory
        if (tracerLineRenderer != null)
        {
            StartCoroutine(DrawTracerFlash(origin, endPoint));
        }
    }
    
    private IEnumerator DrawTracerFlash(Vector3 start, Vector3 end)
    {
        tracerLineRenderer.enabled = true;
        
        // Line renderers require array positions to form a line
        tracerLineRenderer.SetPosition(0, start);
        tracerLineRenderer.SetPosition(1, end);

        // Keep it visible for a split second (e.g., 0.1 seconds) to resemble a bullet flash
        yield return new WaitForSeconds(tracerDuration);

        tracerLineRenderer.enabled = false;
    }

    // Visualizes the 2D detection circle in the Scene View
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
