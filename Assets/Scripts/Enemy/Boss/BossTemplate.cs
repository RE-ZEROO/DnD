using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;


public class BossTemplate : EnemyController
{
    private int bossPhase = 2;
    private bool phaseChange;
    private Vector2 lastPosition;
    private Vector2 currentPosition;
    private Animator animator;

    private static readonly int GoblinKingJumpAnimation = Animator.StringToHash("GoblinKing_Jump");
    private static readonly int GoblinKingFallAnimation = Animator.StringToHash("GoblinKing_Fall");

    protected override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
        enemyType = EnemyType.Boss;
    }


    protected override void Update()
    {
        base.Update();

        //CheckForPhase();

        /*if (bossPhase == 2)
            BossPhase2();*/

        /*if (currentState == EnemyState.ATTACK)
            animator.CrossFade(GoblinKingJumpAnimation, 0, 0);*/
    }

    private void CheckForPhase()
    {
        if (health == (maxHealth / 3))
            bossPhase++;

        /*if (currentState == EnemyState.ATTACK)
            MeeleAttack();*/
    }

    private void BossPhase2()
    {
        
    }

    private void GoblinKingAttack()
    {
        currentPosition = transform.position;
        transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);

        if (currentPosition == lastPosition)
        {
            currentState = EnemyState.RUSH;
            return;
        }
            //animator.CrossFade(GoblinKingFallAnimation, 0, 0);

        lastPosition = currentPosition;
    }

    private void OnDestroy()
    {
        if (!gameObject.scene.isLoaded) { return; }

        Instantiate(roomInstance.portal, transform.position, Quaternion.identity);
    }
}
