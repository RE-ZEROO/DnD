using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;


public class BossTemplate : EnemyController
{
    private int bossPhase = 2;

    //private GameObject healthBar;
    private bool healthBarState = false;

    [Header("Phase 2 Multiplyer")]
    [SerializeField] private float speedMultiplyer;
    [SerializeField] private float cooldownReduced;


    protected override void Start()
    {
        base.Start();
        enemyType = EnemyType.BOSS;
    }


    protected override void Update()
    {
        base.Update();

        if (inRoom)
            animator.SetBool("inRoom", true);

        CheckForPhase();

        SetAnimationStates();
    }

    private void SetAnimationStates()
    {
        if (currentState == EnemyState.IDLE)
        {
            animator.SetBool("isRunning", false);
            animator.ResetTrigger("triggerAttack");
        }
        else if (currentState == EnemyState.FOLLOW)
            animator.SetBool("isRunning", true);
        else if (currentState == EnemyState.ATTACK)
            animator.SetTrigger("triggerAttack");
        else if (currentState == EnemyState.DEAD)
            animator.SetTrigger("triggerDeath");
    }

    private void CheckForPhase()
    {
        if (health == (maxHealth / 2))
            BossPhase2();
    }

    private void BossPhase2()
    {
        animator.SetBool("phase2", true);
        //speed *= speedMultiplyer;
        //cooldown += cooldownReduced;

    }

    private void ToggleHealthBar()
    {
        //healthBarState = !healthBarState;
        //healthBar.SetActive(healthBarState);
    }
    private void Invincibile() => isInvincible = true; 
    private void NotInvincibile() => isInvincible = false; 

    private void OnDestroy()
    {
        if (!gameObject.scene.isLoaded) { return; }

        Instantiate(roomInstance.portal, roomInstance.RoomCenter(), Quaternion.identity);
    }
}
