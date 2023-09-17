using System.Collections;
using UnityEngine;

public class EnemyAnimation : MonoBehaviour
{
    [Header("Animation")]
    private EnemyController enemyController;
    private EnemyState currentState;

    private bool isInIntro;
    
    [SerializeField] private float introAnimTime;
    [SerializeField] private float attackAnimTime = 0.5f;
    //[SerializeField] private float invincibilityAnimTime;

    private Animator animator;
    private int currentAnimationState;
    private float lockedTill;

    //Hashing animation states to improve performance
    //private static readonly int EnemyIntroAnimation = Animator.StringToHash("Enemy_Intro");
    private static readonly int EnemyIdleAnimation = Animator.StringToHash("Enemy_Idle");
    private static readonly int EnemyRunAnimation = Animator.StringToHash("Enemy_Run");
    private static readonly int EnemyAttackAnimation = Animator.StringToHash("Enemy_Attack");
    private static readonly int EnemyDieAnimation = Animator.StringToHash("Enemy_Death");

    private static readonly int EnemyInvincibleAnimation = Animator.StringToHash("Enemy_Invincible");
    private static readonly int EnemyNotInvincibleAnimation = Animator.StringToHash("Enemy_NotInvincible");



    void Start()
    {
        enemyController = GetComponent<EnemyController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        currentState = enemyController.currentState;

        SwitchAnimation();
    }

    /*private IEnumerator IntroTimout()
    {
        isInIntro = true;
        yield return new WaitForSeconds(1);

        if(enemyController.inRoom)
            animator.SetBool("inRoom", true);

        isInIntro = false;
    }*/

    private void StartIntro()
    {
        enemyController.Invincibile();
        isInIntro = true;
    }

    private void EndIntro() 
    {
        enemyController.NotInvincibile();
        isInIntro = false;

        if (enemyController.inRoom)
            animator.SetBool("inRoom", true);
    } 
    

    #region Animation
    private int GetAnimationState()
    {
        if (Time.time < lockedTill) return currentAnimationState;

        //if (enemyController.inRoom)
        //    return LockState(EnemyIntroAnimation, introAnimTime);

        //Set animation based of the enemy state
        if (currentState == EnemyState.WANDER || currentState == EnemyState.FOLLOW)
            return EnemyRunAnimation;
        else if (currentState == EnemyState.ATTACK)
            return LockState(EnemyAttackAnimation, attackAnimTime); //animator.HasState(EnemyAttackAnimation, 0) && 
        else if (currentState == EnemyState.DEAD)
            return EnemyDieAnimation;
        else if (currentState == EnemyState.IDLE)
            return EnemyIdleAnimation;

        /*if (enemyController.isInvincible)
            return LockState(EnemyInvincibleAnimation, invincibilityAnimTime);
        else if (!enemyController.isInvincible)
            return LockState(EnemyNotInvincibleAnimation, invincibilityAnimTime);*/
        

        int LockState(int animation, float time)
        {
            lockedTill = Time.time + time;
            return animation;
        }

        return EnemyIdleAnimation;
    }

    private void SwitchAnimation()
    {
        if (isInIntro) { return; }

        int animationState = GetAnimationState();

        if (animationState == currentAnimationState) { return; }
        animator.CrossFade(animationState, 0, 0);
        currentAnimationState = animationState;
    }

    private float GetCurrentAnimationTime(Animator targetAnim, int layer = 0)
    {
        AnimatorStateInfo animState = targetAnim.GetCurrentAnimatorStateInfo(layer);
        float currentTime = animState.length;
        return currentTime;
    }
    #endregion
}
