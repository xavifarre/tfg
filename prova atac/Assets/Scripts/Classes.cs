using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Enumerador d'estats genèric utilitzat pel player i els minions.
public enum State {
    Idle,
    Attack,
    Dash,
    KnockBack,
    Fall,
    Dead
};