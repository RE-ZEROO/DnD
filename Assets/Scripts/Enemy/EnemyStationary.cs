using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStationary : EnemyController
{
    [Header("Stationary Stats")]
    [SerializeField] private bool isInvincible = true;

    protected override void Start()
    {
        base.Start();

        enemyType = EnemyType.STATIONARY;
        attackRange = detectionRange;
    }

    protected override void Update()
    {
        base.Update();

        if (PlayerInSightLine())
            isInvincible = false;
        else
            isInvincible = true;

        //=======Health can't decline while being invincible==========

        if (currentState == EnemyState.ATTACK && !isInvincible)
            RangeAttack();
    }

    private void RangeAttack()
    {
        if (isOnCooldownAttack && currentState == EnemyState.IDLE && currentState == EnemyState.HIT) { return; }
        
        Shoot();
    }
}