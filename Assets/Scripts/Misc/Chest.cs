using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    private Animator animator;

    [SerializeField] private int health = 3;

    private static readonly int ChestHitAnimation = Animator.StringToHash("Chest_Hit");
    private static readonly int ChestOpenAnimation = Animator.StringToHash("Chest_Open");
    private static readonly int ItemSpawnAnimation = Animator.StringToHash("Item_Spawn");


    void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<BulletController>()?.isEnemyBullet == false)
        {
            animator.CrossFade(ChestHitAnimation, 0, 0);
            AudioManager.Instance.PlaySFX("ChestHit");
            health--;

            if(health <= 0)
            {
                animator.CrossFade(ChestOpenAnimation, 0, 0);
                GetComponent<Collider2D>().enabled = false;
            }
        }
    }

    private void SpawnItem()
    {
        animator.CrossFade(ItemSpawnAnimation, 0, 0);
        AudioManager.Instance.PlaySFX("ItemSpawn");
        GetComponent<ItemGenerator>().GenerateItem();
    }
}
