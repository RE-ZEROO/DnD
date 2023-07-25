using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    //Internal stats
    private static float health = 6f;
    private static int maxHealth = 6;
    //private float healAmount = 0.5f;

    private static float moveSpeed = 90f;
    private static float fireRate = 0.5f;
    private static float bulletSize = 0.2f;

    private static float enemyDamage = 1f;
    [SerializeField] private float invincibilityTime = 1.5f;

    /*private bool bootCollected = false;
    private bool screwCollected = false;
    private bool potionCollected = false;*/

    private bool isInvincible = false;

    public List<string> collectedItems = new List<string>();

    //For public access
    public static float Health { get => health; set => health = value; }
    public static int MaxHealth { get => maxHealth; set => maxHealth = value; }
    public static float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }
    public static float FireRate { get => fireRate; set => fireRate = value; }
    public static float BulletSize { get => bulletSize; set => bulletSize = value; }

    //public Text healthText;

    private Renderer playerRenderer;
    private Collider2D playerCollider;
    private Color playerColor;
    
    private Collider2D[] enemyCollider;
    private Collider2D[] bulletCollider;
    private GameObject[] enemyArray;
    private GameObject[] bulletArray;


    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Start()
    {
        playerRenderer = GameObject.FindGameObjectWithTag("Player").GetComponent<SpriteRenderer>();
        playerCollider = GameObject.FindGameObjectWithTag("Player").GetComponent<Collider2D>();
        playerColor = playerRenderer.material.color;
    }

    

    void Update()
    {
        //healthText.text = "Health: " + health;

        enemyArray = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemyArray) { enemyCollider = GetComponents<Collider2D>(); }
        
        bulletArray = GameObject.FindGameObjectsWithTag("Bullet");
        foreach (GameObject bullet in bulletArray) { bulletCollider = GetComponents<Collider2D>(); }

        StatsMinCap();
    }


    public static void DamagePlayer()
    {
        if(health <= 0) { KillPlayer(); }

        if (!instance.isInvincible)
        {
            health -= enemyDamage;
            instance.StartCoroutine(instance.Invincibility());
        }
    }

    private static void KillPlayer()
    {
        Debug.Log("Player Died!");
    }

    public void UpdateCollectedItems(ItemController collectedItem)
    {
        collectedItems.Add(collectedItem.item.itemName);

        /*foreach(string i in collectedItems)
        {
            switch (i)
            {
                case "Boot":
                    bootCollected = true;
                    break;
                case "Screw":
                    screwCollected = true;
                    break;
                case "Potion":
                    potionCollected = true;
                    break;
            }
        }*/


        //Item Synergies
        //if (bootCollected && screwCollected)
        //    FireRateChange(0.25f);
    }

    private void StatsMinCap()
    {
        if (fireRate < 0.2f)
            fireRate = 0.2f;
        else if (bulletSize < 0.2f)
            bulletSize = 0.2f;
    }

    private IEnumerator Invincibility()
    {
        isInvincible = true;
        //Set a flashing animation later when there is a player sprite
        playerColor.a = 0.5f;
        playerRenderer.material.color = playerColor;

        if (enemyCollider != null)
            for (int e = 0; e < enemyCollider.Length; e++) { Physics2D.IgnoreCollision(playerCollider, enemyCollider[e], true); }

        if(bulletCollider != null)
            for (int b = 0; b < bulletCollider.Length; b++) { Physics2D.IgnoreCollision(playerCollider, bulletCollider[b], true); }

        yield return new WaitForSeconds(invincibilityTime);

        playerColor.a = 1f;
        playerRenderer.material.color = playerColor;

        if (enemyCollider != null)
            for (int e = 0; e < enemyCollider.Length; e++) { Physics2D.IgnoreCollision(playerCollider, enemyCollider[e], false); }

        if (bulletCollider != null)
            for (int b = 0; b < bulletCollider.Length; b++) { Physics2D.IgnoreCollision(playerCollider, bulletCollider[b], false); }

        isInvincible = false;
    }

    #region Player Power-Ups
    public static void HealPlayer(ItemType itemType)
    {
        if (itemType == ItemType.BLUEHEART)
        {
            maxHealth++;
            health++;
        }
        else if (itemType == ItemType.REDHEART && health <= maxHealth - 1)
            health += 1f;
        else if (itemType == ItemType.REDHEART && health <= maxHealth - 0.5f)
            health += 0.5f;
    }
    public static void MoveSpeedChange(float speed) => MoveSpeed += speed;

    public static void FireRateChange(float rate) => fireRate += rate;
    public static void BulletSizeChange(float size) => bulletSize += size;
    public static void TripleShot() => PlayerController.isTripleshot = true;
    #endregion
}
