using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneGolem : BossTemplate
{
    [Header("Stone Golem")]
    //[SerializeField] private float defaultLaserDistance = 300;
    [SerializeField] private Transform laserSpawnPos;
    private LineRenderer lineRenderer;

    //Particles
    [SerializeField] private GameObject startVFX;
    [SerializeField] private GameObject endVFX;
    private List<ParticleSystem> particleList = new List<ParticleSystem>();

    [Space]

    //Rock spawning
    [SerializeField] private int searchForRockRadius = 130;
    private List<Rock> rocksInRoomList = new List<Rock>();
    Collider2D[] hitColliders;


    protected override void Start()
    {
        base.Start();

        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = true;

        hitColliders = Physics2D.OverlapCircleAll(transform.position, searchForRockRadius);
        foreach (Collider2D col in hitColliders)
        {
            Rock rock = col.GetComponent<Rock>();

            if (rock)
            {
                rocksInRoomList.Add(rock);
                rock.gameObject.SetActive(false);
            }
        }

        FillParticleList();
        DisableLaser();
    }

    protected override void Update()
    {
        base.Update();
    }


    #region Laser
    private void ShootLaser()
    {
        lineRenderer.enabled = true;

        string[] layers = { "Player", "Obstacle" };
        Vector2 direction = (player.transform.position - laserSpawnPos.position).normalized;

        RaycastHit2D hit = Physics2D.Linecast(laserSpawnPos.position, player.transform.position, LayerMask.GetMask(layers));
        Draw2DRay(laserSpawnPos.position, hit.point);

        if (hit && hit.collider.GetComponent<PlayerController>())
            GameController.DamagePlayer();

        if (hit && hit.collider.GetComponent<Rock>())
            hit.collider.gameObject.SetActive(false);

        for (int i = 0; i < particleList.Count; i++)
            particleList[i].Play();

        StartCoroutine(Cooldown());
    }

    private void DisableLaser()
    {
        lineRenderer.enabled = false;
        
        for (int i = 0; i < particleList.Count; i++)
            particleList[i].Stop();
    }

    private void Draw2DRay(Vector2 startPos, Vector2 endPos)
    {
        lineRenderer.SetPosition(0, startPos);
        startVFX.transform.position = startPos;

        lineRenderer.SetPosition(1, endPos);
        endVFX.transform.position = lineRenderer.GetPosition(1);
    }
    #endregion

    private void FillParticleList()
    {
        for (int i = 0; i < startVFX.transform.childCount; i++)
        {
            var ps = startVFX.transform.GetChild(i).GetComponent<ParticleSystem>();
            if (ps)
                particleList.Add(ps);
        }

        for (int i = 0; i < endVFX.transform.childCount; i++)
        {
            var ps = endVFX.transform.GetChild(i).GetComponent<ParticleSystem>();
            if (ps)
                particleList.Add(ps);
        }
    }

    private void SpawnRocks()
    {
        foreach(Rock rock in rocksInRoomList)
            rock.gameObject.SetActive(true);

    }
}
