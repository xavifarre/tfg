using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class SoundCollection
{
    public static Dictionary<string, AudioClip> sounds = new Dictionary<string, AudioClip>
    {
        #region Player

        //Player
        {"player_dash", Resources.Load<AudioClip>("Audio/Player/player_dash")},
        {"player_slash01", Resources.Load<AudioClip>("Audio/Player/player_slash01")},
        {"player_slash02", Resources.Load<AudioClip>("Audio/Player/player_slash02")},
        {"player_slash03", Resources.Load<AudioClip>("Audio/Player/player_slash03")},
        {"player_slash04", Resources.Load<AudioClip>("Audio/Player/player_slash04")},

        #endregion

        #region Minions

        //Biter
        {"biter_attack01", Resources.Load<AudioClip>("Audio/Enemies/Biter/biter_attack01")},
        {"biter_attack02", Resources.Load<AudioClip>("Audio/Enemies/Biter/biter_attack02")},
        {"biter_spawn", Resources.Load<AudioClip>("Audio/Enemies/Biter/biter_spawn")},
        {"biter_hit", Resources.Load<AudioClip>("Audio/Enemies/Biter/biter_hit")},
        {"biter_die", Resources.Load<AudioClip>("Audio/Enemies/Biter/biter_die")},

        #endregion
    };
}
