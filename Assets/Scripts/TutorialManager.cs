using System;
using TMPro;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tutText;

    void Awake()
    {
        Events.TRIGGER_TUTORIAL_EVENT.AddListener(OnTriggerTutorial);
    }

    private void OnTriggerTutorial(string text)
    {
        tutText.text = text;
    }
}
