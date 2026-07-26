using System;
using UnityEngine;

public class TroopManager : MonoBehaviour
{
    [SerializeField] private string offsetParamName = "MarchOffset";
    [SerializeField] private float delayStep = 0.1f; // The delay gap between each soldier
    [SerializeField] private float offSetCycle = 1.0f; // The delay gap between each soldier
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private SessionSettingsSO sessionSettings;

    void Awake()
    {
        Events.LEVEL_OVER_EVENT.AddListener(OnLevelOver);
        audioSource.volume = sessionSettings.musicLevel;
    }

    private void OnLevelOver(LevelOverReason reason)
    {
        StopBattleCry();
    }

    void Start()
    {
        // Get all individual animators from the child soldier objects
        Animator[] soldierAnimators = GetComponentsInChildren<Animator>();

        int offsetHash = Animator.StringToHash(offsetParamName);

        for (int i = 0; i < soldierAnimators.Length; i++)
        {
            // Calculate a normalized offset value between 0.0 and 1.0
            // Multiplying by delayStep staggers them incrementally (e.g., 0.0, 0.1, 0.2...)
            float calculatedOffset = (i * delayStep) % offSetCycle;

            // Apply the offset directly to this specific soldier's instance
            soldierAnimators[i].SetFloat(offsetHash, calculatedOffset);
        }
    }

    private void StopBattleCry()
    {
        audioSource.Stop();
        Debug.Log("All soldiers are dead. Battle cry stopped.");
    }
}
