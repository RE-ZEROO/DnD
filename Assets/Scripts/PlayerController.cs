using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using System.Collections;
using static UnityEditor.Experimental.GraphView.GraphView;
using System.Collections.Generic;
using UnityEngine.InputSystem.EnhancedTouch;

public enum PlayerState
{
    IDLE,
    RUN,
    ATTACK,
    TELEPORTING_OUT,
    TELEPORTING_IN,
    DEAD
}

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;
    [SerializeField] public PlayerState currentState = PlayerState.IDLE;

    [Header("Input")]
    public PlayerActionsInput playerInput;
    public InputAction move;
    public InputAction shoot;

    [Header("Movement")]
    public float speed;
    Vector2 moveDirection = Vector2.zero;
    private bool facingRight = true;

    [Header("Shooting")]
    public GameObject bulletPrefab;
    public float bulletSpeed;
    public float fireDelay;
    private float lastFire;

    public static bool isTripleshot = false;
    Vector2 shootDirection = Vector2.zero;

    //Animation
    private Animator animator;

    [Header("Aniamtion")]
    [SerializeField] private float attackAnimTime = 0.75f;

    private int currentAnimationState;
    private float lockedTill;

    //Hashing animation states to improve performance
    private static readonly int Idle = Animator.StringToHash("Player_Idle");
    private static readonly int Run = Animator.StringToHash("Player_Run");
    private static readonly int Attack = Animator.StringToHash("Player_Attack");
    private static readonly int TeleportOut = Animator.StringToHash("Player_Teleport_Out");
    private static readonly int TeleportIn = Animator.StringToHash("Player_Teleport_In");
    private static readonly int Die = Animator.StringToHash("Player_Die");


    #region Enable / Disable Inputs
    public void OnEnable()
    {
        move = playerInput.Player.Move;
        move.Enable();

        shoot = playerInput.Player.Shoot;
        shoot.Enable();
    }

    public void OnDisable()
    {
        move.Disable();
        shoot.Disable();
    }
    #endregion

    private void Awake() => playerInput = new PlayerActionsInput();

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        //playerInput.Player.Shoot.performed += _ => Shoot(shootDirection.x, shootDirection.y);
    }

    void Update()
    {
        //Read input values
        moveDirection = move.ReadValue<Vector2>().normalized;
        shootDirection = shoot.ReadValue<Vector2>();

        //Set stats to current game stats
        fireDelay = GameController.FireRate;
        speed = GameController.MoveSpeed;

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
        if ((shootDirection.x != 0 || shootDirection.y != 0) && Time.time > lastFire + fireDelay)
        {
            Shoot(shootDirection.x, shootDirection.y);
            lastFire = Time.time;
        }

        //Check to flip
        if (facingRight == false && (moveDirection.x > 0 || shootDirection.x > 0))
            Flip();
        else if (facingRight == true && (moveDirection.x < 0 || shootDirection.x < 0))
            Flip();

        //Animation
        var animationState = GetAnimationState();

        if (animationState == currentAnimationState) { return; }
        animator.CrossFade(animationState, 0, 0);
        currentAnimationState = animationState;
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(moveDirection.x * speed, moveDirection.y * speed);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == collision.CompareTag("Enemy") || collision == collision.CompareTag("Boss"))
            GameController.DamagePlayer();
    }

    #region Shooting
    private void Shoot(float x, float y)
    {
        currentState = PlayerState.ATTACK;

        //Create, instantiate and fire Bullet
        if (isTripleshot)
            TripleShot(x, y);

        GameObject bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
        bullet.AddComponent<Rigidbody2D>().gravityScale = 0;
        bullet.GetComponent<Rigidbody2D>().velocity = new Vector2(
            (x < 0) ? Mathf.Floor(x) * bulletSpeed : Mathf.Ceil(x) * bulletSpeed,
            (y < 0) ? Mathf.Floor(y) * bulletSpeed : Mathf.Ceil(y) * bulletSpeed
        );
    }

    private void TripleShot(float x, float y)
    {
        Quaternion upperMoveAngle = Quaternion.Euler(0, 0, 25);
        Quaternion lowerMoveAngle = Quaternion.Euler(0, 0, -25);

        GameObject upperBullet = Instantiate(bulletPrefab, transform.position, upperMoveAngle);
        upperBullet.AddComponent<Rigidbody2D>().gravityScale = 0;

        Vector2 upperDirection = (Vector2)(upperMoveAngle * new Vector3(
        (x < 0) ? Mathf.Floor(x) : Mathf.Ceil(x),
        (y < 0) ? Mathf.Floor(y) : Mathf.Ceil(y), 0));
        upperBullet.GetComponent<Rigidbody2D>().velocity = upperDirection * bulletSpeed;


        GameObject lowerBullet = Instantiate(bulletPrefab, transform.position, lowerMoveAngle);
        lowerBullet.AddComponent<Rigidbody2D>().gravityScale = 0;

        Vector2 lowerDirection = (Vector2)(lowerMoveAngle * new Vector3(
        (x < 0) ? Mathf.Floor(x) : Mathf.Ceil(x),
        (y < 0) ? Mathf.Floor(y) : Mathf.Ceil(y), 0));
        lowerBullet.GetComponent<Rigidbody2D>().velocity = lowerDirection * bulletSpeed;
    }

    private IEnumerator BulletDelay(float delay)
    {
        yield return new WaitForSeconds(5);
        yield return null;
    }
    #endregion

    private void Flip()
    {
        //Flip player sprite
        facingRight = !facingRight;
        Vector3 Scaler = transform.localScale;
        Scaler.x *= -1;
        transform.localScale = Scaler;
    }

    #region Animation
    private int GetAnimationState()
    {
        if (Time.time < lockedTill) return currentAnimationState;

        //Set animation based of the player state
        if (currentState == PlayerState.RUN) 
            return Run;
        else if (currentState == PlayerState.ATTACK)
            return LockState(Attack, attackAnimTime);
        else if (currentState == PlayerState.TELEPORTING_OUT) 
            return TeleportOut;
        else if (currentState == PlayerState.TELEPORTING_IN) 
            return TeleportIn;
        else if (currentState == PlayerState.DEAD) 
            return Die;

        return Idle;

        int LockState(int s, float t)
        {
            lockedTill = Time.time + t;
            return s;
        }
    }
    #endregion
}