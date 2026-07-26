using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Events
{
    // Basic unity events

    public static UnityEvent<HashSet<EntityId>> MINE_BLAST_EVENT = new();
    public static UnityEvent<EntityId> BULLET_SHOT_EVENT = new();
    public static UnityEvent<EntityId> CANON_SHOT_EVENT = new();
    public static UnityEvent<EntityId> WORM_EATEN_EVENT = new();
    public static UnityEvent<string> SCENE_CHANGE_EVENT = new();
    public static UnityEvent<string> TRIGGER_TUTORIAL_EVENT = new();
    public static UnityEvent<PlaySoundEventData> PLAY_SOUND_EVENT = new();
    public static UnityEvent<KilledEventData> KILLED_EVENT = new();
    public static UnityEvent<LevelOverReason> LEVEL_OVER_EVENT = new();
    public static UnityEvent GAME_PAUSE_EVENT = new();
    public static UnityEvent GAME_RESUME_EVENT = new();
    public static UnityEvent FIGHTING_ENEMIES_KILLED_EVENT = new();

}

public enum Sounds
{
    SHOOT, DEATH, EXPLODE
}

public enum KilledType
{
    SOLDIER,
    ENEMY
}

public enum LevelOverReason
{
    SOLDIERS_KILLED,
    ENEMIES_KILLED
}

public struct PlaySoundEventData
{
    public Sounds type;
    public string who;

    public PlaySoundEventData(Sounds type, string who)
    {
        this.type = type;
        this.who = who;
    }
}

public struct KilledEventData
{
    public KilledType killedType;
    public int number;

    public KilledEventData(KilledType killedType, int number)
    {
        this.killedType = killedType;
        this.number = number;
    }
}

