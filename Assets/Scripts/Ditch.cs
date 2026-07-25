using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Ditch : MonoBehaviour
{
    private static readonly int DeathHash = Animator.StringToHash("death");
    [SerializeField] private Animator[] wormAnimators;
    private readonly HashSet<EntityId> soldiersKilled = new(25);
    private HashSet<Animator> liveWormAnimators = new(5);
    private int currentKillCounter = 0;

    void Awake()
    {
        foreach (var anim in wormAnimators)
        {
            liveWormAnimators.Add(anim);
        }
        Events.BULLET_SHOT_EVENT.AddListener(OnBulletShot);
        Events.MINE_BLAST_EVENT.AddListener(OnMineBlast);
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
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Soldier") && liveWormAnimators.Count > 0)
        {
            EntityId soldierId = col.transform.parent.gameObject.GetEntityId();
            if (soldiersKilled.Count >= 25 || soldiersKilled.Contains(soldierId))
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
            }
            StartCoroutine(BroadcastSoldierDeath(soldierId));
        }
    }
    
    private IEnumerator BroadcastSoldierDeath(EntityId soldierId)
    {
        float randomDelay = Random.Range(1f, 3f);

        // Keep a delay in soldier's death to resemble worms slowly eating them
        yield return new WaitForSeconds(randomDelay);

        Events.WORM_EATEN_EVENT.Invoke(soldierId);
    }
}
