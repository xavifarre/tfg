using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class SoundCollection
{
    public static Dictionary<string, AudioClip> sounds = new Dictionary<string, AudioClip>
    {
        {"player_dash", Resources.Load<AudioClip>("Audio/Player/player_dash")},
        {"player_slash01", Resources.Load<AudioClip>("Audio/Player/player_slash01")},
        {"player_slash02", Resources.Load<AudioClip>("Audio/Player/player_slash02")},
        {"player_slash03", Resources.Load<AudioClip>("Audio/Player/player_slash03")},
        {"player_slash04", Resources.Load<AudioClip>("Audio/Player/player_slash04")},

    };
}
