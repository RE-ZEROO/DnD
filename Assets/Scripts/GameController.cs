using System;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using Unity.VisualScripting;

public class GameController : MonoBehaviour
{
    public static GameController Instance;
    public static Action OnInitializeStats;
    public static Action OnPlayerDamaged;
    public static Action OnPlayerHeal;
    public static Action OnBombSpawn;

    [SerializeField] private GameObject worldLevelText;

    //Internal stats
    private static float health;
    private static int maxHealth;

    private static float moveSpeed;
    private static float playerBulletSpeed;
    private static int playerDamage;

    private static float fireRate;
    private static float bulletSize;

    private static float invincibilityTime = 1.5f;
    private static bool isInvincible = false;

    //Enemy Stats
    private static float enemyDamage = 1f;
    private static float enemyBulletSpeedMultiplyer = 1f;

    //Item Stats
    private static int bombCount = 1;
    private static int coinCount;
    private static int keyCount;
    //public List<string> collectedItems = new List<string>();

    //World / Level
    private static int worldNumber = 1;
    private static int levelNumber = 1;

    //For public access
    public static float Health { get => health; set => health = value; }
    public static int MaxHealth { get => maxHealth; set => maxHealth = value; }
    public static float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }
    public static int PlayerDamage { get => playerDamage; set => playerDamage = value; }

    public static float PlayerInvicibilityTime { get => invincibilityTime; }
    public static bool PlayerInvicibility { get => isInvincible; set => isInvincible = value; }

    public static float FireRate { get => fireRate; set => fireRate = value; }
    public static float BulletSize { get => bulletSize; set => bulletSize = value; }
    public static float PlayerBulletSpeed { get => playerBulletSpeed; set => playerBulletSpeed = value; }

    public static float EnemyBulletSpeedMultiplyer { get => enemyBulletSpeedMultiplyer; set => enemyBulletSpeedMultiplyer = value; }

    public static int BombCount { get => bombCount; set => bombCount = value; }
    public static int CoinCount { get => coinCount; set => coinCount = value; }
    public static int KeyCount { get => keyCount; set => keyCount = value; }

    public static int WorldNumber { get => worldNumber; set => worldNumber = value; }
    public static int LevelNumber { get => levelNumber; set => levelNumber = value; }


    private void InitializeStats()
    {
        health = 6f;
        maxHealth = 6;
        moveSpeed = 70f;
        playerBulletSpeed = 90f;
        fireRate = 0.5f;
        bulletSize = 0.7f;
        playerDamage = 100;

        bombCount = 1;
        coinCount = 0;
        keyCount = 0;

        worldNumber = 1;
        levelNumber = 1;

        PlayerController.isTripleshot = false;

        OnInitializeStats?.Invoke();
    }

    void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeStats();
        }
        else
            Destroy(gameObject);
    }


    void Update()
    {
        StatsMinCap();
    }


    public static void DamagePlayer()
    {
        if (isInvincible) { return; }
        
        health -= enemyDamage;
        OnPlayerDamaged?.Invoke();
    }

    public void OnSpawnBomb(InputValue value) => OnBombSpawn.Invoke();


    /*public void UpdateCollectedItems(ItemController collectedItem)
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
        }


        //Item Synergies
        //if (bootCollected && screwCollected)
        //    FireRateChange(0.25f);
    }*/

    private void StatsMinCap()
    {
        if (fireRate < 0.2f)
            fireRate = 0.2f;
        else if (bulletSize < 0.2f)
            bulletSize = 0.2f;
    }

    public static void GameOver()
    {
        Instance.InitializeStats();
        SceneController.Instance.ToggleGameOverCanvas(true);
    }

    public static void NextLevel()
    {
        LevelNumber++;

        /*if(LevelNumber == 5)
        {
            LevelNumber = 1;
            WorldNumber++;
        }*/

        Instance.worldLevelText.GetComponent<TextMeshProUGUI>().text = $"{worldNumber} - {levelNumber}";

        SceneController.Instance.LoadScene("MainScene");
    }
}
