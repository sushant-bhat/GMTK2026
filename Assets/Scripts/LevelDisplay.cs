using TMPro;
using UnityEngine;

public class LevelDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI troopCountDisplay;
    [SerializeField] private TextMeshProUGUI enemyCountDisplay;
    [SerializeField] private int troopCount;
    [SerializeField] private int enemyCount;

    void Awake()
    {
        Events.KILLED_EVENT.AddListener(OnKilled);
        troopCountDisplay.text = troopCount.ToString();
        enemyCountDisplay.text = enemyCount.ToString();
    }

    private void OnKilled(KilledEventData data)
    {
        if (data.killedType.Equals(KilledType.SOLDIER))
        {
            troopCount -= data.number;
            if (troopCount < 0) troopCount = 0;
            troopCountDisplay.text = troopCount.ToString();
        } else
        {
            enemyCount -= data.number;
            if (enemyCount < 0) enemyCount = 0;
            enemyCountDisplay.text = enemyCount.ToString();
        }
    }
}
