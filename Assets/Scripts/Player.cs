using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; // Required to read InputAction.CallbackContext

public class Player : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform weaponPivot;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float detectionRadius = 25f;
    [SerializeField] private Transform gun;
    [SerializeField] private LayerMask targetLayers;

    [Header("Visual Effects")]
    [SerializeField] private LineRenderer tracerLineRenderer; // Assign a LineRenderer here
    [SerializeField] private float tracerDuration = 0.1f;    // How long the flash stays on screen

    private Camera mainCamera;
    private Vector2 mouseScreenPos;
    private HashSet<string> canBeShotTags = new();

    void Awake()
    {
        mainCamera = Camera.main;
        canBeShotTags.Add("Soldier");
        canBeShotTags.Add("Sniper");
        canBeShotTags.Add("Worm");
    }

    // 1. Called via Unity Event whenever the mouse moves
    public void OnAimInput(InputAction.CallbackContext context)
    {
        // Read the screen position vector directly from the event context
        mouseScreenPos = context.ReadValue<Vector2>();
    }

    // 2. Called via Unity Event when the fire button changes state
    public void OnFireInput(InputAction.CallbackContext context)
    {
        // Only trigger the weapon discharge when the button is first pressed down
        if (context.started)
        {
            Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, transform.position.z));
            
            // Fire using instant raycast detection instead of spawning a projectile prefab
            ShootRaycast(mouseWorldPos);
        }
    }

    void Update()
    {
        // Rotation math remains in Update to ensure smooth visual tracking
        HandleWeaponRotation();
    }

    void HandleWeaponRotation()
    {
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, transform.position.z));
        Vector2 aimDirection = (mouseWorldPos - weaponPivot.position).normalized;
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;

        weaponPivot.rotation = Quaternion.Euler(0, 0, angle + 180);
    }

    private void ShootRaycast(Vector3 targetPosition)
    {
        // Use the firePoint position if assigned, otherwise fall back to the gun's position
        Vector2 origin = firePoint != null ? (Vector2)firePoint.position : (Vector2) gun.position;
        
        // Calculate the direction from the origin to the target position
        Vector2 fireDirection = ((Vector2)targetPosition - origin).normalized;

        // Determine where the visual tracer line should end
        Vector2 endPoint;

        // Perform the single instant hit Raycast
        RaycastHit2D hit = Physics2D.Raycast(origin, fireDirection, detectionRadius, targetLayers);

        // Draws a yellow debug ray in the Unity Scene view for testing
        Debug.DrawRay(origin, fireDirection * detectionRadius, Color.yellow, 0.1f);

        if (hit.collider != null)
        {
            endPoint = hit.point;

            if (hit.collider.CompareTag("WeakPoint"))
            {
                Debug.Log("Sniper directly hit the Weak Point!");
            } else if (canBeShotTags.Contains(hit.collider.gameObject.tag))
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

    void Shoot()
    {
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    }
}
