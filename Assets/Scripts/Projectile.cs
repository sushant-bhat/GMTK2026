using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float lifetime = 3f;
    [SerializeField] private ParticleSystem particleSystem;
    private HashSet<string> canBeShotTags = new();

    private bool alreadyShot;

    void Start()
    {
        canBeShotTags.Add("Soldier");
        canBeShotTags.Add("Worm");
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        // Handle impact logic here (e.g., damaging enemies)
        Debug.Log("Canon hit " + col.gameObject.tag);
        particleSystem.Play();
        if (canBeShotTags.Contains(col.gameObject.tag) && !alreadyShot)
        {
            Events.CANON_SHOT_EVENT.Invoke(col.transform.parent.gameObject.GetEntityId());
            alreadyShot = true;
        }
        Destroy(gameObject);
    }
}
