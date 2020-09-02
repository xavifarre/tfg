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

        //tp
        {"player_tp", Resources.Load<AudioClip>("Audio/Player/player_tp")},
        {"player_tp2", Resources.Load<AudioClip>("Audio/Player/player_tp2")},

        //Voice attack
        {"player_attack01", Resources.Load<AudioClip>("Audio/Player/player_voice_attack01")},
        {"player_attack02", Resources.Load<AudioClip>("Audio/Player/player_voice_attack02")},
        {"player_attack03", Resources.Load<AudioClip>("Audio/Player/player_voice_attack03")},
        {"player_attack04", Resources.Load<AudioClip>("Audio/Player/player_voice_attack04")},
        {"player_attack05", Resources.Load<AudioClip>("Audio/Player/player_voice_attack05")},
        {"player_attack07", Resources.Load<AudioClip>("Audio/Player/player_voice_attack06")},
        {"player_attack06", Resources.Load<AudioClip>("Audio/Player/player_voice_attack07")},

        //Voice damage
        {"player_die", Resources.Load<AudioClip>("Audio/Player/player_voice_die")},
        {"player_damage01", Resources.Load<AudioClip>("Audio/Player/player_voice_damage01")},
        {"player_damage02", Resources.Load<AudioClip>("Audio/Player/player_voice_damage02")},
        
        
        {"crystal_recall", Resources.Load<AudioClip>("Audio/Player/crystal_recall")},
        {"creepy_die", Resources.Load<AudioClip>("Audio/Player/die_creepy_sound")},
        



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


        #region Bosses

        {"dead_particles", Resources.Load<AudioClip>("Audio/Enemies/boss_dead_particles")},

        //Summoner
        {"spawn_sound", Resources.Load<AudioClip>("Audio/Enemies/Summoner/spawn_sound")},
        {"summoner_slash", Resources.Load<AudioClip>("Audio/Enemies/Summoner/slash")},
        {"summoner_lunge", Resources.Load<AudioClip>("Audio/Enemies/Summoner/lunge")},
        {"summoner_summon", Resources.Load<AudioClip>("Audio/Enemies/Summoner/summon")},
        {"summoner_dash", Resources.Load<AudioClip>("Audio/Enemies/Summoner/dash")},
        {"summoner_damage", Resources.Load<AudioClip>("Audio/Enemies/Summoner/damage")},
        {"summoner_die", Resources.Load<AudioClip>("Audio/Enemies/Summoner/die")},


        //Preserver
        {"preserver_spin", Resources.Load<AudioClip>("Audio/Enemies/Preserver/spin")},
        {"preserver_expanding_spin", Resources.Load<AudioClip>("Audio/Enemies/Preserver/expanding_spin")},
        {"preserver_undodgeable_spin", Resources.Load<AudioClip>("Audio/Enemies/Preserver/undodgeable_spin")},
        {"preserver_double_slash_charge", Resources.Load<AudioClip>("Audio/Enemies/Preserver/double_slash_charge")},
        {"preserver_double_slash_throw", Resources.Load<AudioClip>("Audio/Enemies/Preserver/double_slash_throw")},
        {"preserver_barrel_pop", Resources.Load<AudioClip>("Audio/Enemies/Preserver/barrel_pop")},
        {"preserver_barrel_throw", Resources.Load<AudioClip>("Audio/Enemies/Preserver/barrel_throw")},
        {"preserver_launch_barrel", Resources.Load<AudioClip>("Audio/Enemies/Preserver/launch_barrel")},
        {"preserver_powder_drop", Resources.Load<AudioClip>("Audio/Enemies/Preserver/powder_drop")},
        {"preserver_die", Resources.Load<AudioClip>("Audio/Enemies/Preserver/preserver_die")},
        {"preserver_heal", Resources.Load<AudioClip>("Audio/Enemies/Preserver/heal")},

        {"blade_move", Resources.Load<AudioClip>("Audio/Enemies/Preserver/blade_rotation")},
        {"barrel_active", Resources.Load<AudioClip>("Audio/Enemies/Preserver/barrel_active")},


        //Destroyer

        {"destroyer_slash01", Resources.Load<AudioClip>("Audio/Enemies/Destroyer/slash01")},
        {"destroyer_slash02", Resources.Load<AudioClip>("Audio/Enemies/Destroyer/slash02")},
        {"destroyer_slamUp", Resources.Load<AudioClip>("Audio/Enemies/Destroyer/slamUp")},
        {"destroyer_slamGround", Resources.Load<AudioClip>("Audio/Enemies/Destroyer/slamGround")},
        {"destroyer_slamDown", Resources.Load<AudioClip>("Audio/Enemies/Destroyer/slamDown")},
        {"destroyer_prepSlash", Resources.Load<AudioClip>("Audio/Enemies/Destroyer/prepSlash")},
        {"destroyer_dashPierce", Resources.Load<AudioClip>("Audio/Enemies/Destroyer/dashPierce")},
        {"destroyer_pierce", Resources.Load<AudioClip>("Audio/Enemies/Destroyer/pierce")},
        {"destroyer_circleCharge", Resources.Load<AudioClip>("Audio/Enemies/Destroyer/circleCharge")},
        {"destroyer_charge", Resources.Load<AudioClip>("Audio/Enemies/Destroyer/charge")},
        {"destroyer_dash", Resources.Load<AudioClip>("Audio/Enemies/Destroyer/dash")},
        {"destroyer_die", Resources.Load<AudioClip>("Audio/Enemies/Destroyer/die")},

        {"destroyer_crystal", Resources.Load<AudioClip>("Audio/Enemies/Destroyer/crystal_spawn")},
        {"destroyer_crystal_throw", Resources.Load<AudioClip>("Audio/Enemies/Destroyer/crystal_throw")},
        {"destroyer_crystal_recall", Resources.Load<AudioClip>("Audio/Enemies/Destroyer/crystal_recall")},
        {"orbital_charge", Resources.Load<AudioClip>("Audio/Enemies/Destroyer/orbital_charge")},
        {"orbital_strike", Resources.Load<AudioClip>("Audio/Enemies/Destroyer/orbital_strike")},

        #endregion

        #region Enviroment

        {"rain", Resources.Load<AudioClip>("Audio/Enviroment/rain")},
        {"thunder01", Resources.Load<AudioClip>("Audio/Enviroment/thunder01")},
        {"thunder02", Resources.Load<AudioClip>("Audio/Enviroment/thunder02")},
        {"thunder03", Resources.Load<AudioClip>("Audio/Enviroment/thunder03")},
        {"enter_torii_level", Resources.Load<AudioClip>("Audio/Enviroment/enter_torii_level")},
        {"birds", Resources.Load<AudioClip>("Audio/Enviroment/birds")},
        {"sword_explosion", Resources.Load<AudioClip>("Audio/Enviroment/sword_explosion")},
        {"sword_swing", Resources.Load<AudioClip>("Audio/Enviroment/sword_swing")},
        {"seal_disappear", Resources.Load<AudioClip>("Audio/Enviroment/seal_disappear")},
        {"eq", Resources.Load<AudioClip>("Audio/Enviroment/eq")},

        #endregion


        #region Music

        {"music_intro_summoner", Resources.Load<AudioClip>("Audio/Music/OST_Boss01_Intro")},
        {"music_loop_summoner", Resources.Load<AudioClip>("Audio/Music/OST_Boss01_Main")},
        {"music_intro_preserver", Resources.Load<AudioClip>("Audio/Music/OST_Boss02_Intro")},
        {"music_loop_preserver", Resources.Load<AudioClip>("Audio/Music/OST_Boss02_Main")},
        {"music_intro_destroyer", Resources.Load<AudioClip>("Audio/Music/OST_Boss03_Intro")},
        {"music_loop_destroyer", Resources.Load<AudioClip>("Audio/Music/OST_Boss03_Main")},

        #endregion

        #region Footsteps

        {"grass1", Resources.Load<AudioClip>("Audio/Player/Footsteps/Grass/grass01")},
        {"grass2", Resources.Load<AudioClip>("Audio/Player/Footsteps/Grass/grass02")},
        {"grass3", Resources.Load<AudioClip>("Audio/Player/Footsteps/Grass/grass03")},
        {"grass4", Resources.Load<AudioClip>("Audio/Player/Footsteps/Grass/grass04")},
        {"grass5", Resources.Load<AudioClip>("Audio/Player/Footsteps/Grass/grass05")},
        {"grass6", Resources.Load<AudioClip>("Audio/Player/Footsteps/Grass/grass06")},

        {"concrete1", Resources.Load<AudioClip>("Audio/Player/Footsteps/Concrete/concrete01")},
        {"concrete2", Resources.Load<AudioClip>("Audio/Player/Footsteps/Concrete/concrete02")},
        {"concrete3", Resources.Load<AudioClip>("Audio/Player/Footsteps/Concrete/concrete03")},
        {"concrete4", Resources.Load<AudioClip>("Audio/Player/Footsteps/Concrete/concrete04")},
        {"concrete5", Resources.Load<AudioClip>("Audio/Player/Footsteps/Concrete/concrete05")},
        {"concrete6", Resources.Load<AudioClip>("Audio/Player/Footsteps/Concrete/concrete06")},



        #endregion
    };
}
