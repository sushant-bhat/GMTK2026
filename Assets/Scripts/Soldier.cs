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
        isAlive = true;
    }

    private void OnKilled(EntityId id)
    {
        if (gameObject.GetEntityId().Equals(id) && isAlive)
        {
            Debug.Log("Soldier killed: " + id.GetHashCode());
            animator.SetTrigger(DeathHash);
            gameObject.transform.SetParent(null);
            isAlive = false;
            Events.KILLED_EVENT.Invoke(new KilledEventData(KilledType.SOLDIER, 1));
        }
    }
}
