using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BossTemplate : EnemyController
{
    public static Action OnToggleHealthBar;

    //[SerializeField] private Slider healthBar;
    //private bool healthBarState = false;

    [Header("Phase 2 Multiplyer")]
    [SerializeField] private float speedMultiplyer;
    [SerializeField] private float cooldownReduced;

    [SerializeField] private BossHealthBar healthBar;
    private bool healthBarState = false;


    protected override void Start()
    {
        base.Start();
        enemyType = EnemyType.BOSS;
        healthBar = FindObjectOfType<BossHealthBar>();
    }


    protected override void Update()
    {
        base.Update();

        if (inRoom && animator.GetBool("inRoom") == false)
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
        if (health == (maxHealth / 4))
            animator.SetBool("phase2", true);
    }

    private void SetPhase2Stats()
    {
        speed *= speedMultiplyer;
        cooldown += cooldownReduced;
    }

    //private void ToggleHealthBar() => OnToggleHealthBar?.Invoke();

    private void Invincibile() => isInvincible = true; 
    private void NotInvincibile() => isInvincible = false; 

    private void OnDestroy()
    {
        if (!gameObject.scene.isLoaded) { return; }

        Instantiate(roomInstance.portal, roomInstance.RoomCenter(), Quaternion.identity);
    }


    

    /*private void OnEnable()
    {
        BossTemplate.OnToggleHealthBar += ToggleHealthBar;
    }

    private void OnDisable()
    {
        BossTemplate.OnToggleHealthBar -= ToggleHealthBar;
    }*/

    /*private void Awake()
    {
        healthBar = FindObjectOfType<BossHealthBar>();
    }*/

    private void ToggleHealthBar()
    {
        //healthBarState = !healthBarState;
        //healthBar.gameObject.SetActive(healthBarState);
    }
}
