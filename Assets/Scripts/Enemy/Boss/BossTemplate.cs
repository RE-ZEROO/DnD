using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BossTemplate : EnemyController
{
    public static Action OnHealthBar;

    [Header("Phase 2 Multiplier")]
    [SerializeField] private float speedMultiplier;
    [SerializeField] private float cooldownReduced;


    protected override void Start()
    {
        base.Start();
        enemyType = EnemyType.BOSS;
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
        if (health <= (maxHealth / 4))
            animator.SetBool("phase2", true);
    }

    private void SetPhase2Stats()
    {
        speed *= speedMultiplier;
        cooldown += cooldownReduced;
    }

    private void OnDestroy()
    {
        if (!gameObject.scene.isLoaded) { return; }

        AudioManager.Instance.inBossRoom = false;
        AudioManager.Instance.PlayBGMusic();
        Instantiate(roomInstance.portal, roomInstance.RoomCenter(), Quaternion.identity);
    }

    private void StartBossMusic()
    {
        AudioManager.Instance.inBossRoom = true;
        AudioManager.Instance.PlayBossMusic();
    }

    private void SpawnHealthBar() => OnHealthBar?.Invoke();
}
