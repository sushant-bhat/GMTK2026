using System;
using UnityEngine;

public class Soldier : MonoBehaviour
{
    private static readonly int DeathHash = Animator.StringToHash("death");
    [SerializeField] private Animator animator;

    void Awake()
    {
        Events.BULLET_SHOT_EVENT.AddListener(OnKilled);
        Events.WORM_EATEN_EVENT.AddListener(OnKilled);
    }

    private void OnKilled(EntityId id)
    {
        if (gameObject.GetEntityId().Equals(id))
        {
            Debug.Log("Soldier killed: " + id.GetHashCode());
            animator.SetTrigger(DeathHash);
            gameObject.transform.SetParent(null);
        }
    }
}
