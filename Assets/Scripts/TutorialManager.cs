using System;
using TMPro;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tutText;

    void Awake()
    {
        Events.TRIGGER_TUTORIAL_EVENT.AddListener(OnTriggerTutorial);
        Events.LEVEL_OVER_EVENT.AddListener(OnLevelOver);
    }

    private void OnLevelOver(LevelOverReason reason)
    {
        tutText.enabled = false;
    }

    private void OnTriggerTutorial(string text)
    {
        tutText.text = text;
    }
}
