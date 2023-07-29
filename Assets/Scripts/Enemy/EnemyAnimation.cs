using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public class EnemyAnimation : MonoBehaviour
{
    [Header("Animation")]
    private EnemyController enemyController;
    private EnemyState currentState;

    [SerializeField] private float attackAnimTime;
    [SerializeField] private float hitAnimTime;

    private Animator animator;
    private int currentAnimationState;
    private float lockedTill;

    //Hashing animation states to improve performance
    private static readonly int EnemyIdleAnimation = Animator.StringToHash("Enemy_Idle");
    private static readonly int EnemyRunAnimation = Animator.StringToHash("Enemy_Run");
    private static readonly int EnemyAttackAnimation = Animator.StringToHash("Enemy_Attack");
    private static readonly int EnemyHitAnimation = Animator.StringToHash("Enemy_Hit");
    private static readonly int EnemyDieAnimation = Animator.StringToHash("Enemy_Death");



    void Start()
    {
        enemyController = GetComponentInParent<EnemyController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        currentState = enemyController.currentState;

        SwitchAnimation();
        SetAnimTime();
    }

    #region Animation
    

    private void SwitchAnimation()
    {
        int animationState = GetAnimationState();

        if (animationState == currentAnimationState) { return; }
        animator.CrossFade(animationState, 0, 0);
        currentAnimationState = animationState;
    }

    private int GetAnimationState()
    {
        if (Time.time < lockedTill) return currentAnimationState;

        //Set animation based of the enemy state
        if (currentState == EnemyState.WANDER || currentState == EnemyState.FOLLOW)
            return EnemyRunAnimation;
        else if (currentState == EnemyState.ATTACK)
            return LockState(EnemyAttackAnimation, attackAnimTime);
        else if (currentState == EnemyState.HIT)
            return LockState(EnemyHitAnimation, hitAnimTime);
        else if (currentState == EnemyState.DEAD)
            return EnemyDieAnimation;
        else if (currentState == EnemyState.IDLE)
            return EnemyIdleAnimation;
        

        int LockState(int animation, float time)
        {
            lockedTill = Time.time + time;
            return animation;
        }

        return EnemyIdleAnimation;
    }

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
