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

}

// public struct MineBlastEventData
// {


//     public SceneChangeEventData(SCENES scene = SCENES.CURR, MENUS menu = MENUS.NONE) {
//         this.scene = scene;
//         this.menu = menu;
//     }
// }

