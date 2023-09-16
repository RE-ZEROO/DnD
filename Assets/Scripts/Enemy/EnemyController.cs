using Pathfinding;
using System.Collections;
using UnityEngine;

public enum EnemyType
{
    MELEE,
    RANGED,
    STATIONARY,
    BOSS
};

public enum EnemyState
{
    IDLE,
    WANDER,
    FOLLOW,
    DEAD,
    ATTACK,
    HIT,
    RUSH
};

public class EnemyController : MonoBehaviour
{
    protected GameObject player;
    protected Rigidbody2D rb;
    protected BoxCollider2D enemyCollider; 
    protected Animator animator;
    private SpriteRenderer spriteRenderer;
    protected RoomInstance roomInstance;

    [SerializeField] public EnemyState currentState = EnemyState.IDLE;
    [SerializeField] public EnemyType enemyType;

    [SerializeField] private GameObject bulletPrefab;

    private Vector3 directionToPlayer;

    protected float distanceToPlayer;
    protected bool isOnCooldownAttack = false;
    protected bool inRoom = false;
    public bool isInvincible = false;
    private readonly bool flip;


    //[Header("Base Stats")]
    [field: SerializeField] public float health { get; private set; }
    public float maxHealth { get; private set; }
    [SerializeField] protected float speed;

    [SerializeField] protected float detectionRange;
    [SerializeField] protected float attackRange;

    [SerializeField] protected float bulletSpeed;
    [SerializeField] protected float cooldown;

    [Header("References")]
    [SerializeField] protected Transform bulletSpawnPos;


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

    //Damage flash
    [SerializeField] private Material flashMaterial;
    private float flashDuration = 0.1f;
    private Material originalMaterial;
    private Coroutine flashRoutine;



    protected virtual void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody2D>();
        enemyCollider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        seeker = GetComponent<Seeker>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        roomInstance = GetComponentInParent<RoomInstance>();

        originalMaterial = spriteRenderer.material;
        maxHealth = health;
        bulletSpeed *= GameController.EnemyBulletSpeedMultiplyer;

        //Pathfinding follow player
        InvokeRepeating(nameof(UpdatePath), 0f, 0.5f);
    }

    protected virtual void Update()
    {
        if (health <= 0f)
            currentState = EnemyState.DEAD;

        if (roomInstance.isCurrentRoom)
            inRoom = true;
        else
            inRoom = false;

        distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
        directionToPlayer = (player.transform.position - transform.position).normalized;
        
        SwitchStates();
        Flip();
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
        RaycastHit2D playerHit;

        if(enemyType == EnemyType.RANGED)
            playerHit = Physics2D.BoxCast(bulletSpawnPos.position, enemyCollider.size, 0, player.transform.position - bulletSpawnPos.position, distanceToPlayer, LayerMask.GetMask(layers));
        else
            playerHit = Physics2D.Raycast(transform.position, player.transform.position - transform.position, distanceToPlayer, LayerMask.GetMask(layers));

        //Debug.Log(playerHit.collider);

        if (playerHit && playerHit.collider.CompareTag("Player"))
            return true;
        else
            return false;
    }

    private void Flip()
    {
        Vector3 scale = transform.localScale;

        if ((currentState == EnemyState.WANDER || currentState == EnemyState.FOLLOW) && rb.velocity.x <= 0.01f)
            scale.x = Mathf.Abs(scale.x) * -1 * (flip ? -1 : 1);
        else if ((currentState == EnemyState.WANDER || currentState == EnemyState.FOLLOW) && rb.velocity.x >= 0.01f)
            scale.x = Mathf.Abs(scale.x) * (flip ? -1 : 1);
        else if (player.transform.position.x < transform.position.x)
            scale.x = Mathf.Abs(scale.x) * -1 * (flip ? -1 : 1);
        else if(player.transform.position.x > transform.position.x)
            scale.x = Mathf.Abs(scale.x) * (flip ? -1 : 1);

        transform.localScale = scale;
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
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPos.transform.position, Quaternion.identity);
        bullet.GetComponent<BulletController>().isEnemyBullet = true;
        bullet.AddComponent<Rigidbody2D>().gravityScale = 0;
        bullet.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;

        //bullet.transform.position = new Vector3(bullet.transform.position.x, bullet.transform.position.y, -1f);

        //Rotate to player
        var relativePos = player.transform.position - transform.position;
        var angle = Mathf.Atan2(relativePos.y, relativePos.x) * Mathf.Rad2Deg;
        var rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        bullet.transform.rotation = rotation;

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

    public void DamageEnemy(int damage)
    {
        if(isInvincible) { return; }

        //currentState = EnemyState.HIT;
        AudioManager.Instance.PlaySFX("Hit");
        health -= damage;
        StartCoroutine(DamageFlash());

        if (flashRoutine != null)
            StopCoroutine(flashRoutine);

        flashRoutine = StartCoroutine(DamageFlash());
    }

    private IEnumerator DamageFlash()
    {
        spriteRenderer.material = flashMaterial;

        yield return new WaitForSeconds(flashDuration);

        spriteRenderer.material = originalMaterial;
        flashRoutine = null;
    }

    private void EnemyDeath() => Destroy(gameObject);

    protected void SwitchStates()
    {
        //If the enemy is in the room
        if (inRoom)
        {
            //Set current states
            if (IsPlayerInRange(detectionRange) && currentState != EnemyState.DEAD && enemyType != EnemyType.STATIONARY)// && currentState != EnemyState.ATTACK
                currentState = EnemyState.FOLLOW;
            else if (!IsPlayerInRange(detectionRange) && currentState != EnemyState.DEAD && enemyType != EnemyType.STATIONARY && enemyType != EnemyType.BOSS)
                currentState = EnemyState.WANDER;

            //!!!!!!!!!!Boss Behavour Teporary!!!!!!!!!!
            if (!isOnCooldownAttack && distanceToPlayer <= attackRange && enemyType == EnemyType.BOSS && currentState != EnemyState.DEAD && currentState != EnemyState.IDLE)
                currentState = EnemyState.ATTACK;
            else if (!isOnCooldownAttack && distanceToPlayer <= attackRange && PlayerInSightLine() && currentState != EnemyState.DEAD && currentState != EnemyState.IDLE)
                currentState = EnemyState.ATTACK;
            else if(isOnCooldownAttack && distanceToPlayer <= attackRange && PlayerInSightLine() && currentState != EnemyState.DEAD)
                currentState = EnemyState.IDLE;
            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

            if (health <= 0)
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
        }
    }
}