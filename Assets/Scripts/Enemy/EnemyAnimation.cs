using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimation : MonoBehaviour
{
    private EnemyState currentState = EnemyState.IDLE;

    [Header("Animation")]
    [SerializeField] private float attackAnimTime = 0.75f;

    private Animator animator;
    private int currentAnimationState;
    private float lockedTill;

    //Hashing animation states to improve performance
    private static readonly int EnemyIdleAnimation = Animator.StringToHash("Enemy_Idle");
    private static readonly int EnemyRunAnimation = Animator.StringToHash("Enemy_Run");
    private static readonly int EnemyAttackAnimation = Animator.StringToHash("Enemy_Attack");
    private static readonly int EnemyHitAnimation = Animator.StringToHash("Enemy_Hit");
    private static readonly int EnemyDieAnimation = Animator.StringToHash("Enemy_Die");



    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        SwitchAnimation();
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
        else if (currentState == EnemyState.DEAD)
            return EnemyDieAnimation;
        else if (currentState == EnemyState.IDLE)
            return EnemyIdleAnimation;

        int LockState(int s, float t)
        {
            lockedTill = Time.time + t;
            return s;
        }

        return EnemyIdleAnimation;
    }
    #endregion
}
