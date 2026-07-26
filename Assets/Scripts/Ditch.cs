using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Ditch : MonoBehaviour
{
    private static readonly int DeathHash = Animator.StringToHash("death");
    [SerializeField] private Animator[] wormAnimators;
    private HashSet<EntityId> soldiersKilled;
    private HashSet<Animator> liveWormAnimators;
    [SerializeField] private int currentKillCounter = 0;
    [SerializeField] private int soldiersKillThreshold = 25;
    [SerializeField] private int liveWormsThreshold = 5;
    [SerializeField] private float[] soldierDeathBroadcastDelay;

    void Awake()
    {
        soldiersKilled = new(soldiersKillThreshold);
        liveWormAnimators = new(liveWormsThreshold);
        foreach (var anim in wormAnimators)
        {
            liveWormAnimators.Add(anim);
        }
        Events.BULLET_SHOT_EVENT.AddListener(OnBulletShot);
        Events.MINE_BLAST_EVENT.AddListener(OnMineBlast);
        Events.CANON_SHOT_EVENT.AddListener(OnCanonBlast);
    }

    private void OnCanonBlast(EntityId id)
    {
        if (transform.parent.gameObject.GetEntityId().Equals(id) && liveWormAnimators.Count > 0)
        {
            Debug.Log("Ditch canon shot");
            foreach (var worm in liveWormAnimators)
            {
                worm.SetTrigger(DeathHash);
            }
            Events.KILLED_EVENT.Invoke(new KilledEventData(KilledType.ENEMY, liveWormAnimators.Count));
            liveWormAnimators.Clear();
        }
    }

    private void OnMineBlast(HashSet<EntityId> idSet)
    {
        if (idSet.Contains(transform.parent.gameObject.GetEntityId()) && liveWormAnimators.Count > 0)
        {
            Debug.Log("Ditch blasted");
            foreach (var worm in liveWormAnimators)
            {
                worm.SetTrigger(DeathHash);
            }
            Events.KILLED_EVENT.Invoke(new KilledEventData(KilledType.ENEMY, liveWormAnimators.Count));
            liveWormAnimators.Clear();
        }
    }

    private void OnBulletShot(EntityId id)
    {
        if (transform.parent.gameObject.GetEntityId().Equals(id) && liveWormAnimators.Count > 0)
        {
            Debug.Log("Ditch shot");
            Animator anim = liveWormAnimators.First();
            liveWormAnimators.Remove(anim);
            anim.SetTrigger(DeathHash);
            Events.KILLED_EVENT.Invoke(new KilledEventData(KilledType.ENEMY, 1));
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Soldier") && liveWormAnimators.Count > 0)
        {
            EntityId soldierId = col.transform.parent.gameObject.GetEntityId();
            if (soldiersKilled.Count >= soldiersKillThreshold || soldiersKilled.Contains(soldierId))
            {
                return;
            }
            soldiersKilled.Add(soldierId);
            currentKillCounter++;
            if (currentKillCounter >= 5)
            {
                currentKillCounter = 0;
                Animator anim = liveWormAnimators.First();
                liveWormAnimators.Remove(anim);
                anim.SetTrigger(DeathHash);
                Events.KILLED_EVENT.Invoke(new KilledEventData(KilledType.ENEMY, 1));
            }
            StartCoroutine(BroadcastSoldierDeath(soldierId));
        }
    }
    
    private IEnumerator BroadcastSoldierDeath(EntityId soldierId)
    {
        float randomDelay = UnityEngine.Random.Range(soldierDeathBroadcastDelay[0], soldierDeathBroadcastDelay[1]);

        // Keep a delay in soldier's death to resemble worms slowly eating them
        yield return new WaitForSeconds(randomDelay);

        Events.WORM_EATEN_EVENT.Invoke(soldierId);
    }
}
