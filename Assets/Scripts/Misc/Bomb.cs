using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] private float timeUntillExplosion;
    [SerializeField] private int BombDamage;
    [SerializeField] private float explosionRadius;

    private void Start()
    {
        StartCoroutine(SetOffExplosion());
    }

    private IEnumerator SetOffExplosion()
    {
        yield return new WaitForSeconds(timeUntillExplosion);
        GetComponent<Animator>().SetTrigger("Explode");
    }

    private void Explode()
    {
        var hitColliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach(var hit in hitColliders)
        {
            var enemy = hit.GetComponent<EnemyController>();
            if (enemy)
                enemy.DamageEnemy(BombDamage);

            var stoneObstacle = hit.CompareTag("Rock");
            if (stoneObstacle)
                hit.GetComponent<Animator>().SetTrigger("destroyed");
        }
    }
}
