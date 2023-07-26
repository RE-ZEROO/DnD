using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMelee : EnemyController
{
    [Header("Melee Stats")]
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashTime;

    protected override void Start()
    {
        base.Start();
        enemyType = EnemyType.MELEE;
    }

    protected override void Update()
    {
        base.Update();

        if (currentState == EnemyState.ATTACK)
            StartCoroutine(Dash());
    }


    private void DashAttack()
    {
        Vector2 target = player.transform.position;
        Vector2 direction = (target - rb.position).normalized;

        Vector2 force = direction * dashSpeed * Time.deltaTime;
        rb.AddForce(force);
    }


    private IEnumerator Dash()
    {
        Vector2 target = player.transform.position;
        Vector2 direction = (target - rb.position).normalized;

        Vector2 force = direction * dashSpeed * Time.deltaTime;
        rb.AddForce(force);
        yield return new WaitForSeconds(dashTime);
    }
}
