using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerContainer;

public class EnemyBase : MonoBehaviour
{
    public EnemyState enemyState;

    public enum EnemyState
    {
        Wander,
        RunUP,
        ChasePlayer,
        ChaseCage,
        Attack,
        Freeze,
        Getchued,
        RunFromPlayer
    };
}


