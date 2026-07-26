using TMPro;
using UnityEngine;

public class LevelDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI troopCountDisplay;
    [SerializeField] private TextMeshProUGUI enemyCountDisplay;
    [SerializeField] private int troopCount;
    [SerializeField] private int enemyCount;
    [SerializeField] private int enemyFightingCount;
    [SerializeField] private bool levelOver;
    [SerializeField] private float speedUpRate;

    void Awake()
    {
        Events.KILLED_EVENT.AddListener(OnKilled);
        troopCountDisplay.text = troopCount.ToString();
        enemyCountDisplay.text = enemyCount.ToString();
        levelOver = false;
    }

    private void OnKilled(KilledEventData data)
    {
        if (levelOver) return;
        if (data.killedType.Equals(KilledType.SOLDIER))
        {
            troopCount -= data.number;
            if (troopCount <= 0) {
                troopCount = 0;
                Events.LEVEL_OVER_EVENT.Invoke(LevelOverReason.SOLDIERS_KILLED);
                levelOver = true;
            }
            troopCountDisplay.text = troopCount.ToString();
        } else {
            enemyCount -= data.number;
            enemyFightingCount -= data.number;
            if (enemyFightingCount <= 0)
            {
                Events.FIGHTING_ENEMIES_KILLED_EVENT.Invoke();
                Time.timeScale = speedUpRate;
            }
            if (enemyCount < 0) {
                enemyCount = 0;
                Events.LEVEL_OVER_EVENT.Invoke(LevelOverReason.ENEMIES_KILLED);
                levelOver = true;
            }
            enemyCountDisplay.text = enemyCount.ToString();
        }
    }
}
