using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimation : MonoBehaviour
{
    [Header("Animation")]
    private EnemyController enemyController;
    private BossTemplate bossTemplate;
    private EnemyState currentState;

    [SerializeField] private float attackAnimTime;
    [SerializeField] private float hitAnimTime;
    [SerializeField] private float invincibilityAnimTime;

    private Animator animator;
    private int currentAnimationState;
    private float lockedTill;

    //Hashing animation states to improve performance
    private static readonly int EnemyIdleAnimation = Animator.StringToHash("Enemy_Idle");
    private static readonly int EnemyRunAnimation = Animator.StringToHash("Enemy_Run");
    private static readonly int EnemyAttackAnimation = Animator.StringToHash("Enemy_Attack");
    private static readonly int EnemyAttackAnimation2 = Animator.StringToHash("Enemy_Attack2");
    private static readonly int EnemyHitAnimation = Animator.StringToHash("Enemy_Hit");
    private static readonly int EnemyDieAnimation = Animator.StringToHash("Enemy_Death");

    private static readonly int EnemyInvincibleAnimation = Animator.StringToHash("Enemy_Invincible");
    private static readonly int EnemyNotInvincibleAnimation = Animator.StringToHash("Enemy_NotInvincible");



    void Start()
    {
        enemyController = GetComponent<EnemyController>();
        bossTemplate = GetComponent<BossTemplate>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        currentState = enemyController.currentState;

        SwitchAnimation();
        //SwitchBossanimation();
        SetAnimTime();
    }

    #region Animation
    private int GetAnimationState()
    {
        if (Time.time < lockedTill) return currentAnimationState;

        //Set animation based of the enemy state
        if (currentState == EnemyState.WANDER || currentState == EnemyState.FOLLOW)
            return EnemyRunAnimation;
        else if (currentState == EnemyState.ATTACK)
            return LockState(EnemyAttackAnimation, attackAnimTime); //animator.HasState(EnemyAttackAnimation, 0) && 
        /*else if (animator.HasState(EnemyAttackAnimation2, 0) && currentState == EnemyState.ATTACK)
            return LockState(EnemyAttackAnimation2, attackAnimTime);*/
        else if (currentState == EnemyState.HIT)
            return LockState(EnemyHitAnimation, hitAnimTime);
        else if (currentState == EnemyState.DEAD)
            return EnemyDieAnimation;
        else if (currentState == EnemyState.IDLE)
            return EnemyIdleAnimation;

        if (enemyController.isInvincible)
            return LockState(EnemyInvincibleAnimation, invincibilityAnimTime);
        else if (!enemyController.isInvincible)
            return LockState(EnemyNotInvincibleAnimation, invincibilityAnimTime);
        

        int LockState(int animation, float time)
        {
            lockedTill = Time.time + time;
            return animation;
        }

        return EnemyIdleAnimation;
    }

    private void SwitchAnimation()
    {
        int animationState = GetAnimationState();

        if (animationState == currentAnimationState) { return; }
        animator.CrossFade(animationState, 0, 0);
        currentAnimationState = animationState;
    }

    /*private void SwitchBossanimation()
    {
        if (bossTemplate != null && bossTemplate.bossPhase == 2)
            EnemyAttackAnimation = Animator.StringToHash("Enemy_Attack2");
    }*/

    private void SetAnimTime()
    {
        if (attackAnimTime != 0f && hitAnimTime != 0f) { return; }

        if (GetAnimationState() == EnemyAttackAnimation)
            attackAnimTime = GetCurrentAnimationTime(animator);
        else if (GetAnimationState() == EnemyHitAnimation)
            hitAnimTime = GetCurrentAnimationTime(animator);
    }

    private float GetCurrentAnimationTime(Animator targetAnim, int layer = 0)
    {
        AnimatorStateInfo animState = targetAnim.GetCurrentAnimatorStateInfo(layer);
        float currentTime = animState.length;
        return currentTime;
    }
    #endregion
}
