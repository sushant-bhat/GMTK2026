using System;
using System.Collections.Generic;
using UnityEngine;

public class Mine : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Collider2D blastRadius;
    [SerializeField] private LayerMask targetLayers;
    [SerializeField] private int maxTargetsCount = 20;

    void Awake()
    {
        Events.BULLET_SHOT_EVENT.AddListener(OnBulletShot);
    }

    private void OnBulletShot(EntityId id)
    {
        if (transform.parent.gameObject.GetEntityId().Equals(id))
        {
            Debug.Log("Mine shot");
            ExplodeAndDetect();
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        ExplodeAndDetect();
    }
    public void ExplodeAndDetect()
    {

        // Create a pre-allocated array to avoid Garbage Collection (GC) spikes at runtime
        Collider2D[] hitResults = new Collider2D[maxTargetsCount];

        // Filter parameters to only find specific layers and include triggers if needed
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(targetLayers);
        filter.useLayerMask = true;
        filter.useTriggers = true; // Set false if you don't want to detect other mines

        // Unity 6 optimized overlap query directly derived from your specific trigger's shape
        int totalCollidersFound = blastRadius.Overlap(filter, hitResults);

        Debug.Log($"💥 Blast triggered! Found {totalCollidersFound} colliders inside the zone.");

        HashSet<EntityId> blastVictims = new(20);
        for (int i = 0; i < totalCollidersFound; i++)
        {
            Collider2D currentCollider = hitResults[i];
            
            // Log the unique Unity 6 Entity ID and GameObject name
            Debug.Log(currentCollider.transform.parent.gameObject.name);
            EntityId eId = currentCollider.transform.parent.gameObject.GetEntityId();

            blastVictims.Add(eId);
        }

        Events.MINE_BLAST_EVENT.Invoke(blastVictims);
        Destroy(gameObject);
    }
}
