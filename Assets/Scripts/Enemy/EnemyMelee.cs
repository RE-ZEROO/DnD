using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMelee : EnemyController
{
    [SerializeField] private float dashSpeed;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        if(enemyType == EnemyType.MELEE  && currentState == EnemyState.ATTACK)
            DashAttack();
    }


    private void DashAttack()
    {
        //Vector2 playerTargetPos = player.transform.position;
        //transform.position = Vector2.MoveTowards(transform.position, playerTargetPos, dashSpeed * Time.deltaTime);
    }
}
