using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMelee : EnemyController
{
    [Header("Melee Stats")]
    [SerializeField] private float dashMultiplier;
    private float dashSpeed;

    protected override void Start()
    {
        base.Start();
        enemyType = EnemyType.MELEE;
        dashSpeed = speed * dashMultiplier;
    }

    protected override void Update()
    {
        base.Update();

        if (currentState == EnemyState.ATTACK)
            DashAttack();
    }


    private void DashAttack()
    {
        Vector2 target = player.transform.position;
        Vector2 direction = (target - rb.position).normalized;

        Vector2 force = direction * dashSpeed * Time.deltaTime;
        rb.AddForce(force);
    }


    /*private IEnumerator Dash()
    {
        Vector2 target = player.transform.position;
        Vector2 direction = (target - rb.position).normalized;

        Vector2 force = direction * dashSpeed * Time.deltaTime;
        rb.AddForce(force);
        yield return new WaitForSeconds(dashTime);
    }*/
}
