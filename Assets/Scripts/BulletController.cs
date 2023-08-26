using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    [SerializeField] private float lifeTime;
    [SerializeField] private float enemyBulletSpeed = 90f;
    [SerializeField] private GameObject impactEffect;

    public bool isEnemyBullet;

    private Vector2 lastPosition;
    private Vector2 currentPosition;
    private Vector2 playerPosition;


    void Start()
    {
        //enemyBulletSpeed = GameController.EnemyBulletSpeed;

        StartCoroutine(DeathDelay());

        //Set bullet size to player bullet size
        if (!isEnemyBullet)
        {
            transform.localScale = new Vector2(GameController.BulletSize, GameController.BulletSize);
        }

        playerPosition = FindObjectOfType<PlayerController>().transform.position;
    }

    void Update()
    {
        if (isEnemyBullet)
        {
            //Shoot towards player
            currentPosition = transform.position;
            transform.position = Vector2.MoveTowards(transform.position, playerPosition, enemyBulletSpeed * Time.deltaTime);

            if (currentPosition == lastPosition)
                Destroy(gameObject);

            lastPosition = currentPosition;
        }
    }


    public void GetPlayer(Transform player) => playerPosition = player.position;


    IEnumerator DeathDelay()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && !isEnemyBullet)
        {
            collision.gameObject.GetComponentInParent<EnemyController>().Damage();
            //collision.gameObject.GetComponentInParent<EnemyAnimation>().AnimationHitEnemy();
            Destroy(gameObject);
        }
        else if(collision.CompareTag("Player") && isEnemyBullet)
        {
            GameController.DamagePlayer();
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Wall") || collision.CompareTag("Door") || collision.CompareTag("BossDoor"))
            Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (!gameObject.scene.isLoaded) { return; }

        if (impactEffect != null)
            Instantiate(impactEffect, transform.position, gameObject.transform.rotation);
    }
}
