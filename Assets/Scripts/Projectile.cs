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
        canBeShotTags.Add("Troop");
        canBeShotTags.Add("Worm");
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        // Handle impact logic here (e.g., damaging enemies)
        Debug.Log("Canon hit " + col.gameObject.tag);
        if (canBeShotTags.Contains(col.gameObject.tag) && !alreadyShot)
        {
            Events.CANON_SHOT_EVENT.Invoke(col.transform.parent.gameObject.GetEntityId());
            alreadyShot = true;
        }
        Destroy(gameObject);
    }
}
