using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRanged : EnemyController
{
    protected override void Start()
    {
        base.Start();
        enemyType = EnemyType.RANGED;
    }

    protected override void Update()
    {
        base.Update();

        if (currentState == EnemyState.ATTACK)
            RangeAttack();
    }

    private void RangeAttack()
    {
        if(!isOnCooldownAttack)
            Shoot();
    }
}