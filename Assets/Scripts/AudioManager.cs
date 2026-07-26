using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    private AudioSource source;
    [SerializeField] private string who;
    [SerializeField] private AudioClip shootClip; 
    [SerializeField] private AudioClip deathClip; 
    [SerializeField] private SessionSettingsSO sessionSettings;

    void Awake()
    {
        source = GetComponent<AudioSource>();
        source.volume = sessionSettings.musicLevel;
        Events.PLAY_SOUND_EVENT.AddListener(PlaySound);
    }

    void PlaySound(PlaySoundEventData data)
    {
        if (!data.who.Equals(who)) return;

        if (data.type.Equals(Sounds.SHOOT))
        {
            source.PlayOneShot(shootClip);
        } else if (data.type.Equals(Sounds.DEATH))
        {
            source.PlayOneShot(deathClip);
        }
    }
}
