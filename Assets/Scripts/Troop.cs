using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Troop : MonoBehaviour
{
    private static readonly int DeathHash = Animator.StringToHash("death");
    [SerializeField] private int id = 1;
    [SerializeField] private float speed = 10f;
    [SerializeField] private float dir = -1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponent<Rigidbody2D>().linearVelocity =  dir * speed * transform.right;
        Events.MINE_BLAST_EVENT.AddListener(OnMineBlast);
    }

    private void OnMineBlast(HashSet<EntityId> idSet)
    {
        if(idSet.Contains(gameObject.GetEntityId()))
        {
            Debug.Log("Troop " + id + " got blasted");
            Animator[] soldierAnimators = GetComponentsInChildren<Animator>();
            
            for (int i = 0; i < soldierAnimators.Length; i++)
            {
                soldierAnimators[i].SetTrigger(DeathHash);
            }
            GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        }
    }
}
