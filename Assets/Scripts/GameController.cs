using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    //Internal stats
    private static float health;
    private static int maxHealth;
    //private float healAmount = 0.5f;

    private static float moveSpeed;
    private static float fireRate = 0.5f;
    private static float bulletSize;

    private static float enemyDamage;
    private static float playerDamage;

    [SerializeField] private static float invincibilityTime = 1.5f;
    private bool isInvincible = false;

    /*private bool bootCollected = false;
    private bool screwCollected = false;
    private bool potionCollected = false;*/


    public List<string> collectedItems = new List<string>();

    //For public access
    public static float Health { get => health; set => health = value; }
    public static int MaxHealth { get => maxHealth; set => maxHealth = value; }
    public static float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }
    public static float FireRate { get => fireRate; set => fireRate = value; }
    public static float BulletSize { get => bulletSize; set => bulletSize = value; }
    public static float PlayerDamage { get => playerDamage; set => playerDamage = value; }

    public static float PlayerInvicibilityTime { get => invincibilityTime; set => invincibilityTime = value; }

    //public Text healthText;

    private GameObject player;
    private PlayerController playerController;
    private Renderer playerRenderer;
    private Collider2D playerCollider;
    private Color playerColor;
    
    private Collider2D[] enemyCollider;
    private Collider2D[] bulletCollider;
    private GameObject[] enemyArray;
    private GameObject[] bulletArray;


    private void SetStats()
    {
        health = 6f;
        maxHealth = 6;
        moveSpeed = 90f;
        fireRate = 0.5f;
        bulletSize = 0.2f;
        enemyDamage = 1f;
        playerDamage = 1f;
    }

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Start()
    {
        SetStats();

        player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
        playerCollider = player.GetComponent<Collider2D>();
        playerRenderer = player.GetComponent<SpriteRenderer>();
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

        if (health <= 0)
            KillPlayer();
    }


    public static void DamagePlayer()
    {
        if (instance.isInvincible) { return; }
        
        health -= enemyDamage;
        instance.StartCoroutine(instance.Invincibility());
    }

    private void KillPlayer()
    {
        //Debug.Log("Player Died!");
        

        playerController.PlayerDeathState();
        SetStats();
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
        //playerController.currentState = PlayerState.Invincible;
        playerColor.a = 0.5f;
        playerRenderer.material.color = playerColor;

        if (enemyCollider != null)
            for (int e = 0; e < enemyCollider.Length; e++) { Physics2D.IgnoreCollision(playerCollider, enemyCollider[e], true); }

        if(bulletCollider != null)
            for (int b = 0; b < bulletCollider.Length; b++) { Physics2D.IgnoreCollision(playerCollider, bulletCollider[b], true); }

        yield return new WaitForSeconds(invincibilityTime);

        playerColor.a = 1f;
        //playerController.currentState = PlayerState.IDLE;
        playerRenderer.material.color = playerColor;

        if (enemyCollider != null)
            for (int e = 0; e < enemyCollider.Length; e++) { Physics2D.IgnoreCollision(playerCollider, enemyCollider[e], false); }

        if (bulletCollider != null)
            for (int b = 0; b < bulletCollider.Length; b++) { Physics2D.IgnoreCollision(playerCollider, bulletCollider[b], false); }

        isInvincible = false;
    }

    public static void GameOver()
    {
        instance.SetStats();
        //Change this to show a gameover panel

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public static void NextLevel()
    {
        instance.SetStats();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    #region Player Power-Ups
    public static void HealPlayer(ItemType itemType)
    {
        if (itemType == ItemType.BLUEHEART)
        {
            maxHealth++;
            health++;
        }
        else if (itemType == ItemType.REDHEART && health < maxHealth) //itemType == ItemType.REDHEART && health <= maxHealth - 1
            health += 1f;
        /*else if (itemType == ItemType.REDHEART && health <= maxHealth ) //itemType == ItemType.REDHEART && health <= maxHealth - 0.5f
            health += 0.5f;*/
    }

    public static void MoveSpeedChange(float speed) => MoveSpeed += speed;
    public static void FireRateChange(float rate) => fireRate += rate;
    public static void BulletSizeChange(float size) => bulletSize += size;
    public static void PlayerDamageChange(float damage) => bulletSize += damage;
    public static void TripleShot() => PlayerController.isTripleshot = true;
    #endregion
}
