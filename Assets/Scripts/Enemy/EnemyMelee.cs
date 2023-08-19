using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMelee : EnemyController
{
    [Header("Melee Stats")]
    [SerializeField] private float dashMultiplier;
    private float dashSpeed;

    [SerializeField] private bool isDasher;

    protected override void Start()
    {
        base.Start();
        enemyType = EnemyType.MELEE;
        dashSpeed = speed * dashMultiplier;
    }

    protected override void Update()
    {
        base.Update();


        if (currentState == EnemyState.ATTACK && !isDasher)
            MeeleAttack();
        else if (currentState == EnemyState.ATTACK && isDasher)
            DashAttack();
    }


    private void MeeleAttack()
    {
        if (isOnCooldownAttack) { return; }

        GameController.DamagePlayer();

        StartCoroutine(Cooldown());
    }


    private void DashAttack()
    {
        Vector2 target = player.transform.position;
        Vector2 direction = (target - rb.position).normalized;

        Vector2 force = direction * dashSpeed * Time.deltaTime;
        rb.AddForce(force);
    }
}
