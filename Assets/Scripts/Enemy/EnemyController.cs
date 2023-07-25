using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public enum EnemyType
{
    MELEE,
    RANGED,
    STATIONARY
};

public enum EnemyState
{
    IDLE,
    WANDER,
    FOLLOW,
    DEAD,
    ATTACK,
    RUSH
};

public class EnemyController : MonoBehaviour
{
    private GameObject player;
    private Rigidbody2D rb;
    private Collider2D coll;
    [SerializeField] private Transform enemyGFX;
    public GameObject tester;

    [SerializeField] protected EnemyState currentState = EnemyState.IDLE;
    [SerializeField] protected EnemyType enemyType;

    [SerializeField] private float range;
    [SerializeField] private float maxDistance;
    private Vector2 wanderingWayPoint;


    [Header("Stats")]
    public float speed;
    //public float dashSpeed;
    public float retreatSpeed;

    public float detectionRange;
    //public float stoppingDistance;
    public float retreatDistance;

    public float attackRange;

    public float bulletSpeed;
    public float cooldown;

    private float distanceToPlayer;

    private bool isChoosingDirection = false;
    //private bool isDead = false;
    private bool isOnCooldownAttack = false;

    //[HideInInspector]
    protected bool inRoom = true;

    private Vector3 randomDir;
    public GameObject bulletPrefab;



    [Header("Pathfinding")]
    public float nextWaypointDistance = 3f;
    
    private Seeker seeker;
    private Path path;
    private int currentWaypoint = 0;
    private bool reachedEndOfPath = false;



    [Header("Animation")]
    [SerializeField] private float attackAnimTime = 0.75f;

    private Animator animator;
    private int currentAnimationState;
    private float lockedTill;

    //Hashing animation states to improve performance
    private static readonly int EnemyIdleAnimation = Animator.StringToHash("Enemy_Idle");
    private static readonly int EnemyRunAnimation = Animator.StringToHash("Enemy_Run");
    private static readonly int EnemyAttackAnimation = Animator.StringToHash("Enemy_Attack");
    private static readonly int EnemyHitAnimation = Animator.StringToHash("Enemy_Hit");
    private static readonly int EnemyDieAnimation = Animator.StringToHash("Enemy_Die");





    protected virtual void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        seeker = GetComponent<Seeker>();
        animator = GetComponent<Animator>();

        //Pathfinding follow player
        InvokeRepeating(nameof(UpdatePath), 0f, 0.5f);
    }

    protected virtual void Update()
    {
        distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
        
        SwitchStates();

        SwitchAnimation();

        

        //Flipping Sprite
        if (rb.velocity.x >= 0.01f)
            enemyGFX.localScale = new Vector3(1f, 1f, 1f);
        else if (rb.velocity.x <= 0.01f)
            enemyGFX.localScale = new Vector3(-1f, 1f, 1f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(currentState == EnemyState.WANDER && other.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
            SetNewWanderPosition();
    }


    #region Pathfinding
    private void UpdatePath()
    {
        if(!seeker.IsDone()) { return; }

        //seeker.StartPath(rb.position, player.transform.position, OnPathComplete);

        if (currentState == EnemyState.FOLLOW)
            seeker.StartPath(rb.position, player.transform.position, OnPathComplete);

    }

    private void OnPathComplete(Path p)
    {
        if (p.error) { return; }

        path = p;
        currentWaypoint = 0;
    }
    #endregion


    protected bool IsPlayerInRange(float range) => distanceToPlayer <= range;

    #region Move Methods
    private void Wander()
    {
        if (Vector2.Distance(transform.position, wanderingWayPoint) < range)
            SetNewWanderPosition();
            

        Vector2 direction = (wanderingWayPoint - (Vector2)transform.position).normalized;

        Vector2 force = direction * speed * Time.deltaTime;
        rb.AddForce(force);
    }

    private void SetNewWanderPosition()
    {
        wanderingWayPoint = new Vector2(transform.position.x + Random.Range(-maxDistance, maxDistance + 1), 
                                        transform.position.y + Random.Range(-maxDistance, maxDistance + 1));

        Instantiate(tester, wanderingWayPoint, Quaternion.identity);
    }


    protected void Follow()
    {
        if(path == null) { return; }

        if (currentWaypoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;
            return;
        }
        else
            reachedEndOfPath = false;

        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;

        Vector2 force = direction * speed * Time.deltaTime;
        rb.AddForce(force);

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

        if (distance < nextWaypointDistance)
            currentWaypoint++;
    }

    //Dodge player (For ranged types)
    protected void Retreat()
    {
        Vector2 direction = ((Vector2)player.transform.position + rb.position).normalized;
        Vector2 force = direction * speed * Time.deltaTime;
        rb.AddForce(force);

        //transform.position = Vector2.MoveTowards(transform.position, player.transform.position, -retreatSpeed * Time.deltaTime);
    }
    #endregion


    #region Attack Methods
    

    private void RangeAttack()
    {
        if (!isOnCooldownAttack) { Shoot(); }

        if (distanceToPlayer < attackRange && distanceToPlayer > retreatDistance) { transform.position = this.transform.position; }
        else if (distanceToPlayer < retreatDistance) { Retreat(); }
    }

    protected void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity) as GameObject;
        bullet.GetComponent<BulletController>().GetPlayer(player.transform);
        bullet.AddComponent<Rigidbody2D>().gravityScale = 0;
        bullet.GetComponent<BulletController>().isEnemyBullet = true;
        StartCoroutine(Cooldown());
    }

    protected IEnumerator Cooldown()
    {
        isOnCooldownAttack = true;
        yield return new WaitForSeconds(cooldown);
        isOnCooldownAttack = false;
    }
    #endregion

    private void Idle()
    {

    }

    public void Death()
    {
        currentState = EnemyState.DEAD;

        //=======Add Death Animation=========

        Destroy(gameObject);
    }

    protected void SwitchStates()
    {
        //If the enemy is in the room
        if (inRoom)
        {
            //Set current states
            if (IsPlayerInRange(detectionRange) && currentState != EnemyState.DEAD)
                currentState = EnemyState.FOLLOW;
            else if (!IsPlayerInRange(detectionRange) && currentState != EnemyState.DEAD)
                currentState = EnemyState.WANDER;

            if (distanceToPlayer <= attackRange)
                currentState = EnemyState.ATTACK;
        }
        else
            currentState = EnemyState.IDLE;

        //Call methods when in state
        switch (currentState)
        {
            case (EnemyState.IDLE):
                Idle();
                break;
            case (EnemyState.WANDER):
                Wander();
                break;
            case (EnemyState.FOLLOW):
                Follow();
                break;
            case (EnemyState.DEAD):
                Death();
                break;
            case (EnemyState.ATTACK):
                if (enemyType == EnemyType.MELEE) { }
                else if (enemyType == EnemyType.RANGED) { RangeAttack(); };
                break;
            /*case (EnemyState.RUSH):
                DashAttack();
                break;*/
            default:
                currentState = EnemyState.IDLE;
                break;
        }
    }

    #region Animation
    private void SwitchAnimation()
    {
        int animationState = GetAnimationState();

        if (animationState == currentAnimationState) { return; }
        animator.CrossFade(animationState, 0, 0);
        currentAnimationState = animationState;
    }

    private int GetAnimationState()
    {
        if (Time.time < lockedTill) return currentAnimationState;

        //Set animation based of the enemy state
        if (currentState == EnemyState.WANDER || currentState == EnemyState.FOLLOW)
            return EnemyRunAnimation;
        else if (currentState == EnemyState.ATTACK)
            return LockState(EnemyAttackAnimation, attackAnimTime);
        else if (currentState == EnemyState.DEAD)
            return EnemyDieAnimation;
        else if (currentState == EnemyState.IDLE)
            return EnemyIdleAnimation;

        int LockState(int s, float t)
        {
            lockedTill = Time.time + t;
            return s;
        }

        return EnemyIdleAnimation;
    }
    #endregion
}