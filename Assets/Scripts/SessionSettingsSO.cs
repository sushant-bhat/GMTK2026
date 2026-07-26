using UnityEngine;

[CreateAssetMenu(fileName = "SessionSettings", menuName = "Settings/Session Settings")]
public class SessionSettingsSO : ScriptableObject
{
    [Header("Reset Defaults (On Refresh/New Window)")]
    [SerializeField] private float defaultSound = 0.75f;
    [SerializeField] private float defaultMusic = 0.75f;
    [SerializeField] private string defaultLevel = "TutorialScene";

    [Header("Live Session Data (Cleared on Close/Refresh)")]
    public float soundLevel;
    public float musicLevel;
    public string lastLevelPlayed;

    // Automatically runs once when the game first boots in the browser
    private void OnEnable()
    {
        ResetToSessionDefaults();
    }

    public void ResetToSessionDefaults()
    {
        soundLevel = defaultSound;
        musicLevel = defaultMusic;
        lastLevelPlayed = defaultLevel;
    }
}
