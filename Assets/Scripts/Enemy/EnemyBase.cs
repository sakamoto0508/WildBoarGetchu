using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerContainer;

public class EnemyBase : MonoBehaviour
{
    public EnemyState enemyState;

    [SerializeField] int _attackPower;
    public int AttackPower { get => _attackPower; }
    public enum EnemyState
    {
        Wander,
        ChasePlayer,
        ChaseCage,
        Attack,
        Freeze,
        Getchued
    };
}


