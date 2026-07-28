using System;
using System.Collections.Generic;
using UnityEngine;

public class Soldier : MonoBehaviour
{
    private static readonly int DeathHash = Animator.StringToHash("death");
    [SerializeField] private Animator animator;
    [SerializeField] private bool isAlive;

    void Awake()
    {
        Events.BULLET_SHOT_EVENT.AddListener(OnKilled);
        Events.WORM_EATEN_EVENT.AddListener(OnKilled);
        Events.MINE_BLAST_EVENT.AddListener(OnTroopKilledByMine);
        Events.CANON_SHOT_EVENT.AddListener(OnTroopKilled);
        isAlive = true;
    }

    private void OnTroopKilledByMine(HashSet<EntityId> idSet)
    {
        if (!idSet.Contains(gameObject.GetEntityId())) return;
        Events.TROOP_KILLED_EVENT.Invoke(transform.parent.gameObject.GetEntityId());
    }

    private void OnTroopKilled(EntityId id)
    {
        if (!gameObject.GetEntityId().Equals(id)) return;
        Events.TROOP_KILLED_EVENT.Invoke(transform.parent.gameObject.GetEntityId());
    }

    private void OnKilled(EntityId id)
    {
        if (gameObject.GetEntityId().Equals(id) && isAlive)
        {
            animator.SetTrigger(DeathHash);
            gameObject.transform.SetParent(null);
            isAlive = false;
            Events.PLAY_SOUND_EVENT.Invoke(new PlaySoundEventData(Sounds.DEATH, "Soldier"));
            Events.KILLED_EVENT.Invoke(new KilledEventData(KilledType.SOLDIER, 1));
        }
    }
}
