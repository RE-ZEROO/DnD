using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneGolem : BossTemplate
{
    [Header("Stone Golem")]
    [SerializeField] private float defaultLaserDistance = 300;
    [SerializeField] private Transform laserSpawnPos;
    private LineRenderer lineRenderer;

    protected override void Start()
    {
        base.Start();
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void ShootLaser()
    {
        if(Physics2D.Raycast(laserSpawnPos.position, transform.right))
        {
            RaycastHit2D hit = Physics2D.Raycast(laserSpawnPos.position, transform.right);
            Draw2DRay(laserSpawnPos.position, hit.point);
        }
        else
            Draw2DRay(laserSpawnPos.position, laserSpawnPos.transform.right * defaultLaserDistance);

        StartCoroutine(Cooldown());
    }

    private void Draw2DRay(Vector2 startPos, Vector2 endPos)
    {
        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, endPos);
    }
}
