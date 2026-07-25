using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 15f;
    [SerializeField] private float lifetime = 3f;
    [SerializeField] private float dir = -1f;
    private HashSet<string> canBeShotTags = new();

    private bool alreadyShot;

    void Start()
    {
        canBeShotTags.Add("Soldier");
        canBeShotTags.Add("Sniper");
        canBeShotTags.Add("Worm");
        // Move forward along the bullet's local right axis (X-axis)
        GetComponent<Rigidbody2D>().linearVelocity =  dir * speed * transform.right;
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        // Handle impact logic here (e.g., damaging enemies)
        Destroy(gameObject);
        if (canBeShotTags.Contains(col.gameObject.tag) && !alreadyShot)
        {
            Events.BULLET_SHOT_EVENT.Invoke(col.transform.parent.gameObject.GetEntityId());
            alreadyShot = true;
        }
    }
}
