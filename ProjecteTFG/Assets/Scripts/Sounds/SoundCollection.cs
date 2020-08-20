using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class SoundCollection
{
    public static Dictionary<string, AudioClip> sounds = new Dictionary<string, AudioClip>
    {
        #region Player

        //Dash
        {"player_dash", Resources.Load<AudioClip>("Audio/Player/player_dash")},
        {"player_voice_dash01", Resources.Load<AudioClip>("Audio/Player/player_voice_dash01")},
        {"player_voice_dash02", Resources.Load<AudioClip>("Audio/Player/player_voice_dash02")},

        //Slash
        {"player_slash01", Resources.Load<AudioClip>("Audio/Player/player_slash01")},
        {"player_slash02", Resources.Load<AudioClip>("Audio/Player/player_slash02")},
        {"player_slash03", Resources.Load<AudioClip>("Audio/Player/player_slash03")},
        {"player_slash04", Resources.Load<AudioClip>("Audio/Player/player_slash04")},

        //Voice attack
        {"player_attack01", Resources.Load<AudioClip>("Audio/Player/player_voice_attack01")},
        {"player_attack02", Resources.Load<AudioClip>("Audio/Player/player_voice_attack02")},
        {"player_attack03", Resources.Load<AudioClip>("Audio/Player/player_voice_attack03")},
        {"player_attack04", Resources.Load<AudioClip>("Audio/Player/player_voice_attack04")},
        {"player_attack05", Resources.Load<AudioClip>("Audio/Player/player_voice_attack05")},

        //Voice damage
        {"player_die", Resources.Load<AudioClip>("Audio/Player/player_voice_die")},
        {"player_damage01", Resources.Load<AudioClip>("Audio/Player/player_voice_damage01")},
        {"player_damage02", Resources.Load<AudioClip>("Audio/Player/player_voice_damage02")},
        



        #endregion

        #region Minions

        //Biter
        {"biter_attack01", Resources.Load<AudioClip>("Audio/Enemies/Biter/biter_attack01")},
        {"biter_attack02", Resources.Load<AudioClip>("Audio/Enemies/Biter/biter_attack02")},
        {"biter_spawn", Resources.Load<AudioClip>("Audio/Enemies/Biter/biter_spawn")},
        {"biter_hit", Resources.Load<AudioClip>("Audio/Enemies/Biter/biter_hit")},
        {"biter_die", Resources.Load<AudioClip>("Audio/Enemies/Biter/biter_die")},

        //Bomb
        {"bomb_spawn", Resources.Load<AudioClip>("Audio/Enemies/Bomb/bomb_spawn")},
        {"bomb_charge", Resources.Load<AudioClip>("Audio/Enemies/Bomb/bomb_charge")},
        {"bomb_explosion", Resources.Load<AudioClip>("Audio/Enemies/Bomb/bomb_explosion")},
        {"bomb_move", Resources.Load<AudioClip>("Audio/Enemies/Bomb/bomb_move")},

        //Slasher
        {"slasher_spawn", Resources.Load<AudioClip>("Audio/Enemies/Slasher/slasher_spawn")},
        {"slasher_attack", Resources.Load<AudioClip>("Audio/Enemies/Slasher/slasher_attack")},
        {"slasher_die", Resources.Load<AudioClip>("Audio/Enemies/Slasher/slasher_die")},
        {"slasher_hit", Resources.Load<AudioClip>("Audio/Enemies/Slasher/slasher_hit")},
        
        //Sentinel
        {"sentinel_spawn", Resources.Load<AudioClip>("Audio/Enemies/Sentinel/sentinel_spawn")},
        {"sentinel_hit", Resources.Load<AudioClip>("Audio/Enemies/Sentinel/sentinel_hit")},
        {"sentinel_shoot", Resources.Load<AudioClip>("Audio/Enemies/Sentinel/sentinel_shoot")},
        {"sentinel_die", Resources.Load<AudioClip>("Audio/Enemies/Sentinel/sentinel_die")},


        #endregion

        #region Enviroment

        {"rain", Resources.Load<AudioClip>("Audio/Enviroment/rain")},
        {"thunder01", Resources.Load<AudioClip>("Audio/Enviroment/thunder01")},
        {"thunder02", Resources.Load<AudioClip>("Audio/Enviroment/thunder02")},
        {"thunder03", Resources.Load<AudioClip>("Audio/Enviroment/thunder03")},
        {"enter_torii_level", Resources.Load<AudioClip>("Audio/Enviroment/enter_torii_level")},

        #endregion
    };
}
