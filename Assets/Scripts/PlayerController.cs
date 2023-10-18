using System;
using System.Collections;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem;

public enum PlayerState
{
    IDLE,
    RUN,
    ATTACK,
    TELEPORTING_OUT,
    TELEPORTING_IN,
    Invincible,
    DEAD
}

public class PlayerController : MonoBehaviour
{
    public PlayerState currentState = PlayerState.IDLE;
    private Rigidbody2D rb;
    private Renderer playerRenderer;
    private Collider2D playerCollider;
    private Color playerColor = Color.white;

    private Collider2D[] enemyCollider;
    private Collider2D[] bulletCollider;
    private GameObject[] enemyArray;
    private GameObject[] bulletArray;

    [SerializeField] private GameObject bombGO;

    [Header("Input")]
    [SerializeField] private PlayerActionsInput playerInput;
    private InputAction move;
    private InputAction shoot;
    private InputAction spawnBombPressed;

    [Header("Movement")]
    [SerializeField] private float speed;
    private Vector2 moveDirection = Vector2.zero;

    [HideInInspector] public bool canMove = true;
    private bool facingRight = true;
    private bool isTouchingEnemy;

    [Header("Shooting")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletSpawnPos;
    [SerializeField] private float fireDelay;
    private float bulletSpeed;
    private float lastFire;
    private float xShootValue;
    private float yShootValue;

    public static bool isTripleshot = false;
    private Vector2 shootDirection = Vector2.zero;

    //Animation
    [Header("Aniamtion")]
    [SerializeField] private float attackAnimTime;
    [SerializeField] private float teleportAnimTime;
    [SerializeField] private int trippleNum;

    private Animator animator;
    private int currentAnimationState;
    private float lockedTill;

    //Hashing animation states to improve performance
    private static readonly int PlayerIdleAnimation = Animator.StringToHash("Player_Idle");
    private static readonly int PlayerRunAnimation = Animator.StringToHash("Player_Run");
    private static readonly int PlayerAttackAnimation = Animator.StringToHash("Player_Attack");
    private static readonly int PlayerTeleportOutAnimation = Animator.StringToHash("Player_Teleport_Out");
    private static readonly int PlayerTeleportInAnimation = Animator.StringToHash("Player_Teleport_In");
    private static readonly int PlayerInvincibleAnimation = Animator.StringToHash("Player_Invincible");
    private static readonly int PlayerDieAnimation = Animator.StringToHash("Player_Die");


    #region Enable / Disable Inputs
    public void OnEnable()
    {
        move = playerInput.Player.Move;
        move.Enable();

        shoot = playerInput.Player.Shoot;
        shoot.Enable();

        spawnBombPressed = playerInput.Player.SpawnBomb;
        spawnBombPressed.Enable();

        GameController.OnPlayerDamaged += StartInvincibility;
        GameController.OnBombSpawn += SpawnBomb;
    }

    public void OnDisable()
    {
        move.Disable();
        shoot.Disable();
        spawnBombPressed.Disable();

        GameController.OnPlayerDamaged -= StartInvincibility;
        GameController.OnBombSpawn -= SpawnBomb;
    }
    #endregion

    private void Awake() => playerInput = new PlayerActionsInput();

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerCollider = GetComponent<Collider2D>();
        playerRenderer = GetComponent<SpriteRenderer>();

        GameController.PlayerInvicibility = false;
        GameController.OnPlayerHeal?.Invoke();
    }

    void Update()
    {
        if(!canMove || currentState == PlayerState.DEAD) { return; }

        //Read input values
        moveDirection = move.ReadValue<Vector2>().normalized;
        shootDirection = shoot.ReadValue<Vector2>();

        //Check colliders
        enemyArray = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemyArray) { enemyCollider = GetComponents<Collider2D>(); }

        bulletArray = GameObject.FindGameObjectsWithTag("Bullet");
        foreach (GameObject bullet in bulletArray) { bulletCollider = GetComponents<Collider2D>(); }

        //Set stats to current game stats
        fireDelay = GameController.FireRate;
        speed = GameController.MoveSpeed;
        bulletSpeed = GameController.PlayerBulletSpeed;

        //Set player state
        currentState = PlayerState.IDLE;
        if(moveDirection.x != 0  || moveDirection.y != 0)
            currentState = PlayerState.RUN;

        //Only shoot horizontally or vertically
        if (shootDirection.x != 0)
            shootDirection.y = 0;
        else if (shootDirection.y != 0)
            shootDirection.x = 0;
            
        //Only shoot when cooldown is down 
        if ((shootDirection.x != 0 || shootDirection.y != 0) && Time.time > lastFire + fireDelay && !isTouchingEnemy)
        {
            xShootValue = shootDirection.x;
            yShootValue = shootDirection.y;
            currentState = PlayerState.ATTACK;
            lastFire = Time.time;
        }

        //Check to flip
        if (facingRight == false && (moveDirection.x > 0 || shootDirection.x > 0))
            Flip();
        else if (facingRight == true && (moveDirection.x < 0 || shootDirection.x < 0))
            Flip();

        //Animation
        var animationState = GetAnimationState();
        if (animationState != currentAnimationState)
        {
            animator.CrossFade(animationState, 0, 0);
            currentAnimationState = animationState;
        }

        //Kill player
        if (GameController.Health <= 0)
            PlayerDeathState();
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(moveDirection.x * speed, moveDirection.y * speed);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") || collision.CompareTag("Boss"))
        {
            isTouchingEnemy = true;
            GameController.DamagePlayer();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") || collision.CompareTag("Boss"))
            isTouchingEnemy = false;
    }

    #region Shooting
    private void Shoot()
    {
        if (isTripleshot)
            TripleShot();
        else
            InstantiateStraightBullet(xShootValue, yShootValue);
    }

    private void TripleShot()
    {
        Quaternion upperMoveAngle = Quaternion.Euler(0, 0, 25);
        Quaternion lowerMoveAngle = Quaternion.Euler(0, 0, -25);

        InstantiateStraightBullet(xShootValue, yShootValue);
        InstantiateAngledBullet(xShootValue, yShootValue, upperMoveAngle);
        InstantiateAngledBullet(xShootValue, yShootValue, lowerMoveAngle);

        #region Trippleshot upgrade for later
        /*Quaternion lowerMoveAngle2 = Quaternion.Euler(0, 0, -90);
        Quaternion lowerMoveAngle3 = Quaternion.Euler(0, 0, -65);
        Quaternion lowerMoveAngle4 = Quaternion.Euler(0, 0, -115);

        Quaternion upperMoveAngle2 = Quaternion.Euler(0, 0, 90);
        Quaternion upperMoveAngle3 = Quaternion.Euler(0, 0, 65);
        Quaternion upperMoveAngle4 = Quaternion.Euler(0, 0, 115);


        if(trippleNum == 1)
        {
            InstantiateStraightBullet(xShootValue, yShootValue);
            InstantiateAngledBullet(xShootValue, yShootValue, upperMoveAngle);
            InstantiateAngledBullet(xShootValue, yShootValue, lowerMoveAngle);
        }

        if (trippleNum == 2)
        {
            InstantiateStraightBullet(-xShootValue, -yShootValue);
            InstantiateAngledBullet(-xShootValue, -yShootValue, upperMoveAngle);
            InstantiateAngledBullet(-xShootValue, -yShootValue, lowerMoveAngle);
        }

        if (trippleNum == 3)
        {
            InstantiateAngledBullet(xShootValue, yShootValue, lowerMoveAngle2);
            InstantiateAngledBullet(xShootValue, yShootValue, lowerMoveAngle3);
            InstantiateAngledBullet(xShootValue, yShootValue, lowerMoveAngle4);
        }

        if (trippleNum == 4)
        {
            InstantiateAngledBullet(xShootValue, yShootValue, upperMoveAngle2);
            InstantiateAngledBullet(xShootValue, yShootValue, upperMoveAngle3);
            InstantiateAngledBullet(xShootValue, yShootValue, upperMoveAngle4);
        }*/
        #endregion
    }

    private void InstantiateStraightBullet(float x, float y)
    {
        GameObject bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);

        bullet.AddComponent<Rigidbody2D>().gravityScale = 0;
        bullet.GetComponent<Rigidbody2D>().velocity = new Vector2(
            (x < 0) ? Mathf.Floor(x) * bulletSpeed : Mathf.Ceil(x) * bulletSpeed,
            (y < 0) ? Mathf.Floor(y) * bulletSpeed : Mathf.Ceil(y) * bulletSpeed);

        //Rotate bullet
        var relativePos = bullet.GetComponent<Rigidbody2D>().velocity;
        var angle = Mathf.Atan2(relativePos.y, relativePos.x) * Mathf.Rad2Deg;
        var rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        bullet.transform.rotation = rotation;
    }

    private void InstantiateAngledBullet(float x, float y, Quaternion rotationAngle)
    {
        GameObject upperBullet = Instantiate(bulletPrefab, transform.position, rotationAngle);
        upperBullet.AddComponent<Rigidbody2D>().gravityScale = 0;

        Vector2 direction = (Vector2)(rotationAngle * new Vector3(
        (x < 0) ? Mathf.Floor(x) : Mathf.Ceil(x),
        (y < 0) ? Mathf.Floor(y) : Mathf.Ceil(y), 0));
        upperBullet.GetComponent<Rigidbody2D>().velocity = direction * bulletSpeed;

        //Rotate bullet
        var relativePos = upperBullet.GetComponent<Rigidbody2D>().velocity;
        var angle = Mathf.Atan2(relativePos.y, relativePos.x) * Mathf.Rad2Deg;
        var rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        upperBullet.transform.rotation = rotation;
    }
    #endregion

    private void SpawnBomb()
    {
        if(GameController.BombCount <= 0) {  return; }

        Instantiate(bombGO, transform.position, Quaternion.identity);
        GameController.BombCount--;
        BombItem.OnBombCollected?.Invoke();
    }

    private void Flip()
    {
        //Flip player sprite
        facingRight = !facingRight;
        Vector3 Scaler = transform.localScale;
        Scaler.x *= -1;
        transform.localScale = Scaler;
    }

    public void PlayerDeathState()
    {
        currentState = PlayerState.DEAD;
        animator.CrossFade(PlayerDieAnimation, 0, 0);
    }

    private void Death() => GameController.GameOver();
    private void PlayerNextLevel() => GameController.NextLevel();

    #region Invincibility
    private void StartInvincibility() => StartCoroutine(Invincibility());

    private IEnumerator Invincibility()
    {
        GameController.PlayerInvicibility = true;
        //Set a flashing animation later when there is a player sprite
        //playerController.currentState = PlayerState.Invincible;
        playerColor.a = 0.5f;
        playerRenderer.material.color = playerColor;

        if (enemyCollider != null)
            for (int e = 0; e < enemyCollider.Length; e++) { Physics2D.IgnoreCollision(playerCollider, enemyCollider[e], true); }

        if (bulletCollider != null)
            for (int b = 0; b < bulletCollider.Length; b++) { Physics2D.IgnoreCollision(playerCollider, bulletCollider[b], true); }

        yield return new WaitForSeconds(GameController.PlayerInvicibilityTime);

        playerColor.a = 1f;
        //playerController.currentState = PlayerState.IDLE;
        playerRenderer.material.color = playerColor;

        if (enemyCollider != null)
            for (int e = 0; e < enemyCollider.Length; e++) { Physics2D.IgnoreCollision(playerCollider, enemyCollider[e], false); }

        if (bulletCollider != null)
            for (int b = 0; b < bulletCollider.Length; b++) { Physics2D.IgnoreCollision(playerCollider, bulletCollider[b], false); }

        GameController.PlayerInvicibility = false;
    }
    #endregion

    #region Animation
    private int GetAnimationState()
    {
        if (Time.time < lockedTill) return currentAnimationState;

        //Set animation based of the player state
        if (currentState == PlayerState.RUN)
            return PlayerRunAnimation;
        else if (currentState == PlayerState.ATTACK)
            return LockState(PlayerAttackAnimation, attackAnimTime);
        else if (currentState == PlayerState.TELEPORTING_OUT)
            return LockState(PlayerTeleportOutAnimation, teleportAnimTime);
        else if (currentState == PlayerState.TELEPORTING_IN)
            return LockState(PlayerTeleportInAnimation, teleportAnimTime);
        else if (currentState == PlayerState.Invincible)
            return LockState(PlayerInvincibleAnimation, GameController.PlayerInvicibilityTime);

        return PlayerIdleAnimation;

        int LockState(int s, float t)
        {
            lockedTill = Time.time + t;
            return s;
        }
    }
    #endregion
}