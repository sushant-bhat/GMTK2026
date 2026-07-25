using System;
using System.Collections;
using UnityEngine;

public class Sniper : MonoBehaviour
{
    private static readonly int DeathHash = Animator.StringToHash("death");
    private static readonly WaitForSeconds _waitForSeconds1 = new(1f);
    
    [Header("Animation")]
    [SerializeField] private Animator sniperAnimator;
    
    [Header("Targets")]
    [SerializeField] private Transform weakPoint;
    [SerializeField] private Transform gun;
    [SerializeField] private float detectionRadius = 25f;
    [SerializeField] private LayerMask soldierLayer; // Ensure this mask covers both your Soldiers and the WeakPoint layers
    [SerializeField] private Transform firePoint;
    
    [Header("Visual Effects")]
    [SerializeField] private LineRenderer tracerLineRenderer; // Assign a LineRenderer here
    [SerializeField] private float tracerDuration = 0.1f;    // How long the flash stays on screen


    // Animation parameter hashes for performance
    private static readonly int IsShootingHash = Animator.StringToHash("isShooting");

    private float soldierTimer = 0f;
    private float weakPointTimer = 0f;
    private bool isAlive;
    private bool isAlternateWeakPointTime = true; // Tracks 5th, 15th, 25th vs 10th, 20th, 30th
    
    // Track the coroutine to safely stop it if the sniper dies mid-shot
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
            Debug.Log("Sniper shot");
            
            // Interrupt the shot sequence immediately if it's currently running
            if (activeFireRoutine != null)
            {
                StopCoroutine(activeFireRoutine);
                activeFireRoutine = null;
            }

            sniperAnimator.SetBool(IsShootingHash, false);
            sniperAnimator.SetTrigger(DeathHash);
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
        weakPointTimer += Time.deltaTime;

        // Check weak point timer first (every 5 seconds)
        if (weakPointTimer >= 5f)
        {
            weakPointTimer = 0f; // Reset loop

            if (isAlternateWeakPointTime)
            {
                ShootWeakPoint();
            }
            
            // Toggle so it only fires on the 5th, 15th, 25th, etc.
            isAlternateWeakPointTime = !isAlternateWeakPointTime;
        }
        // If it's not time to shoot the weak point, check the soldier timer (every 2 seconds)
        else if (soldierTimer >= 2f)
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

    private void ShootWeakPoint()
    {
        if (weakPoint != null)
        {
            activeFireRoutine = StartCoroutine(FireWeaponRoutine(weakPoint.position));
        }
    }

    private Transform FindClosestSoldier()
    {
        // Find all 2D colliders within radius matching the soldier layer
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius, soldierLayer);
        Transform bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        foreach (Collider2D collider in hitColliders) {
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
        Debug.Log("Set shooting true");
        sniperAnimator.SetBool(IsShootingHash, true);
        
        // 3. Fire the single raycast shot
        ShootRaycast(targetPosition);

        // 4. Delay to let animation play out (adjust as needed)
        yield return _waitForSeconds1;

        // 5. Reset to crouch state
        Debug.Log("Set shooting false");
        sniperAnimator.SetBool(IsShootingHash, false);
        
        activeFireRoutine = null;
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

        if (hit.collider != null)
        {
            endPoint = hit.point;

            if (hit.collider.CompareTag("WeakPoint"))
            {
                Debug.Log("Sniper directly hit the Weak Point!");
            } else if (hit.collider.CompareTag("Soldier"))
            {
                Events.BULLET_SHOT_EVENT.Invoke(hit.collider.transform.parent.gameObject.GetEntityId());
            }
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
