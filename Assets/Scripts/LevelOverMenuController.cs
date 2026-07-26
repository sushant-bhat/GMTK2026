using TMPro;
using UnityEngine;

public class LevelOverMenuController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI result;
    [SerializeField] private GameObject levelOverMenu;
    [SerializeField] private GameObject tryAgainButton;
    [SerializeField] private GameObject nextLevelButton;

    void Awake()
    {
        Events.LEVEL_OVER_EVENT.AddListener(OnLevelOver);
    }

    private void OnLevelOver(LevelOverReason reason)
    {
        levelOverMenu.SetActive(true);
        if (reason.Equals(LevelOverReason.ENEMIES_KILLED))
        {
            result.text = "Your troop lives another day!";
            nextLevelButton.SetActive(true);
            tryAgainButton.SetActive(false);
        } else
        {
            result.text = "Ouch! Wanna try again?";
            nextLevelButton.SetActive(false);
            tryAgainButton.SetActive(true);
        }
    }
}
