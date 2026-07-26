using UnityEngine;

public class FinishPoint : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D col)
    {
        Events.LEVEL_OVER_EVENT.Invoke(LevelOverReason.ENEMIES_KILLED);
    }
}
