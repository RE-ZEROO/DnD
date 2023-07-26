using Pathfinding;
using System.Collections;
using UnityEngine;

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
    protected GameObject player;
    protected Rigidbody2D rb;
    protected Collider2D coll;

    [SerializeField] protected EnemyState currentState = EnemyState.IDLE;
    [SerializeField] protected EnemyType enemyType;

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform enemyGFX;

    protected float distanceToPlayer;
    protected bool isOnCooldownAttack = false;
    protected bool inRoom = true;


    [Header("Base Stats")]
    [SerializeField] protected float health;
    [SerializeField] protected float speed;

    [SerializeField] protected float detectionRange;
    [SerializeField] protected float attackRange;

    [SerializeField] protected float bulletSpeed;
    [SerializeField] protected float cooldown;


    //[Header("Pathfinding")]
    private float nextWaypointDistance = 3f;
    private Seeker seeker;
    private Path path;
    private int currentWaypoint = 0;
    private bool reachedEndOfPath = false;

    //Random Wandering
    private float wanderingRange = 1f;
    private float wanderingMaxDistance = 30f;
    private Vector2 wanderingWayPoint;


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

    protected bool IsPlayerInRange(float range) => distanceToPlayer <= range;
    protected bool PlayerInSightLine()
    {
        string[] layers = { "Player", "Obstacle" };
        RaycastHit2D hit = Physics2D.Raycast(transform.position, player.transform.position - transform.position, distanceToPlayer, LayerMask.GetMask(layers));

        if (hit.collider.CompareTag("Player"))
            return true;
        else
            return false;
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

    #region Move Methods
    private void Wander()
    {
        if (Vector2.Distance(transform.position, wanderingWayPoint) < wanderingRange)
            SetNewWanderPosition();
            

        Vector2 direction = (wanderingWayPoint - (Vector2)transform.position).normalized;

        Vector2 force = direction * speed * Time.deltaTime;
        rb.AddForce(force);
    }

    private void SetNewWanderPosition()
    {
        wanderingWayPoint = new Vector2(transform.position.x + Random.Range(-wanderingMaxDistance, wanderingMaxDistance + 1), 
                                        transform.position.y + Random.Range(-wanderingMaxDistance, wanderingMaxDistance + 1));
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


    #endregion

    #region Attack Methods
    protected void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity) as GameObject;
        bullet.GetComponent<BulletController>().GetPlayer(player.transform);
        bullet.AddComponent<Rigidbody2D>().gravityScale = 0;
        bullet.GetComponent<BulletController>().isEnemyBullet = true;
        StartCoroutine(Cooldown());
    }

    private IEnumerator Cooldown()
    {
        isOnCooldownAttack = true;
        yield return new WaitForSeconds(cooldown);
        isOnCooldownAttack = false;
    }
    #endregion


    private void Idle()
    {

    }

    public void Damage()
    {
        health -= (GameController.PlayerDamage / 2); //Enemies have two colliders => divide by 2
        Debug.Log(health);

        if(health <= 0 )
            currentState = EnemyState.DEAD;
    }

    public void Death() => Destroy(gameObject);

    protected void SwitchStates()
    {
        //If the enemy is in the room
        if (inRoom)
        {
            //Set current states
            if (IsPlayerInRange(detectionRange) && currentState != EnemyState.DEAD && enemyType != EnemyType.STATIONARY)
                currentState = EnemyState.FOLLOW;
            else if (!IsPlayerInRange(detectionRange) && currentState != EnemyState.DEAD && enemyType != EnemyType.STATIONARY)
                currentState = EnemyState.WANDER;


            if (distanceToPlayer <= attackRange && PlayerInSightLine() && currentState != EnemyState.DEAD)
                currentState = EnemyState.ATTACK;

            if(health <= 0)
                currentState = EnemyState.DEAD;
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
            /*case (EnemyState.DEAD):
                Death();
                break;*/
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