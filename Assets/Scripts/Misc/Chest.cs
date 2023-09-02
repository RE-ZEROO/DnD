using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    private Animator animator;

    [SerializeField] private int health = 3;
    [SerializeField] private bool needsKey;

    private static readonly int ChestHitAnimation = Animator.StringToHash("Chest_Hit");
    private static readonly int ChestOpenAnimation = Animator.StringToHash("Chest_Open");
    private static readonly int ItemSpawnAnimation = Animator.StringToHash("Item_Spawn");


    void Start()
    {
        animator = GetComponent<Animator>();

        //if(needsKey)
        //    GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (needsKey && collision.GetComponentInParent<PlayerController>() != null && GameController.KeyCount > 0)
        {
            GameController.KeyCount--;
            Key.OnKeyCollected?.Invoke();
            AudioManager.Instance.PlaySFX("ChestUnlock");
            OpenChest();
        }
        else if (!needsKey && collision.GetComponent<BulletController>()?.isEnemyBullet == false)
        {
            animator.CrossFade(ChestHitAnimation, 0, 0);
            AudioManager.Instance.PlaySFX("ChestHit");
            health--;

            if (health <= 0)
                OpenChest();
        }
        else if (collision.GetComponent<BulletController>() != null)
        {
            AudioManager.Instance.PlaySFX("ChestHit");
        }
    }

    private void OpenChest()
    {
        animator.CrossFade(ChestOpenAnimation, 0, 0);
        GetComponent<Collider2D>().enabled = false;
    }

    private void SpawnItem()
    {
        animator.CrossFade(ItemSpawnAnimation, 0, 0);
        AudioManager.Instance.PlaySFX("ItemSpawn");
        GetComponent<ItemGenerator>().GenerateItem();
    }
}
