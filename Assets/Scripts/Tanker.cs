using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tanker : MonoBehaviour
{
    private static readonly int ShootHash = Animator.StringToHash("shoot");
    private static readonly int DeathHash = Animator.StringToHash("death");
    private static readonly int DestroyHash = Animator.StringToHash("destroy");
    [Header("Animation")]
    [SerializeField] private Animator tankerAnimator;
    
    [Header("Targets")]
    [SerializeField] private Transform gun;
    [SerializeField] private float detectionRadius = 15f;
    [SerializeField] private LayerMask targetLayers;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject canonPrefab;
    [SerializeField] private ParticleSystem particleSystem;


    // Animation parameter hashes for performance

    [SerializeField] private float canonSpeed = 15f;
    [SerializeField] private float soldierTimer = 10f;
    [SerializeField] private float soldierTimerThreshold = 12f;
    [SerializeField] private bool isAlive;

    void Awake()
    {
        isAlive = true;
        Events.MINE_BLAST_EVENT.AddListener(OnMineBlast);
        Events.BULLET_SHOT_EVENT.AddListener(OnBulletShot);
    }

    private void OnBulletShot(EntityId id)
    {
        if (gameObject.GetEntityId().Equals(id))
        {
            tankerAnimator.SetTrigger(DeathHash);
            isAlive = false;
            Events.KILLED_EVENT.Invoke(new KilledEventData(KilledType.ENEMY, 1));
        }
    }

    private void OnMineBlast(HashSet<EntityId> idSet)
    {
        if (idSet.Contains(gameObject.GetEntityId()))
        {
            tankerAnimator.SetTrigger(DestroyHash);
            isAlive = false;
            Events.KILLED_EVENT.Invoke(new KilledEventData(KilledType.ENEMY, 1));
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
        
        if (soldierTimer >= soldierTimerThreshold)
        {
            soldierTimer = 0f;
            // 2. Trigger shoot animation
            tankerAnimator.SetTrigger(ShootHash);
        }
    }

    public void ShootClosestSoldier()
    {
        Transform closestSoldier = FindClosestSoldier();

        if (closestSoldier != null)
        {
            Debug.Log("Closest soldier for tanker: " + closestSoldier.position);
            FireWeapon(closestSoldier.position);
        }
    }

    private Transform FindClosestSoldier()
    {
        // Find all 2D colliders within radius matching the soldier layer
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius, targetLayers);
        Transform bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        foreach (Collider2D collider in hitColliders)
        {
            if (!collider.CompareTag("Troop")) continue; 

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

    private void FireWeapon(Vector3 targetPosition)
    {
        Events.PLAY_SOUND_EVENT.Invoke(new PlaySoundEventData(Sounds.SHOOT, "Tanker"));
        particleSystem.Play();

        // 1. Rotate the gun toward the target from its pivot point
        Vector3 pivotDirection = targetPosition - gun.position;
        float angle = Mathf.Atan2(pivotDirection.y, pivotDirection.x) * Mathf.Rad2Deg;
        gun.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        // Execute the shot logic
        GameObject canon = Instantiate(canonPrefab, firePoint.position, firePoint.rotation);
        
        // FIX: Calculate direction from the actual SPAWN POINT (firePoint) to the target
        Vector3 canonDirection = (targetPosition - firePoint.position).normalized;
        
        // Assign the corrected trajectory vector to the Rigidbody2D
        // (Note: generalized to Vector2 to prevent any Z-axis calculation interference)
        canon.GetComponent<Rigidbody2D>().linearVelocity = (Vector2) canonDirection * canonSpeed;
    }

    // Visualizes the 2D detection circle in the Scene View
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
