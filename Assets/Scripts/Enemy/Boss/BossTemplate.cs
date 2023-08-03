using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BossTemplate : EnemyController
{
    public int bossPhase = 1;
    private bool phaseChange;

    protected override void Start()
    {
        base.Start();
    }


    protected override void Update()
    {
        base.Update();

        //CheckForPhase();

        if (bossPhase == 2)
            BossPhase2();
    }

    private void CheckForPhase()
    {
        if (health == (maxHealth / 3))
            bossPhase++;
    }

    private void BossPhase2()
    {

    }

    private void OnDestroy()
    {
        if (!gameObject.scene.isLoaded) { return; }

        Instantiate(roomInstance.portal, transform.position, Quaternion.identity);
    }
}
