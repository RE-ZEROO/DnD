using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    [SerializeField] private float lifeTime;

    public bool isEnemyBullet;

    private Vector2 lastPosition;
    private Vector2 currentPosition;
    private Vector2 playerPosition;


    void Start()
    {
        StartCoroutine(DeathDelay());

        //Set bullet size to player bullet size
        if (!isEnemyBullet)
        {
            transform.localScale = new Vector2(GameController.BulletSize, GameController.BulletSize);
        }
    }

    void Update()
    {
        if (isEnemyBullet)
        {
            //Shoot towards player
            currentPosition = transform.position;
            transform.position = Vector2.MoveTowards(transform.position, playerPosition, 90f * Time.deltaTime);

            if(currentPosition == lastPosition)
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
        if(collision.CompareTag("Enemy") && !isEnemyBullet)
        {
            collision.gameObject.GetComponentInParent<EnemyController>().Damage();
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
}
