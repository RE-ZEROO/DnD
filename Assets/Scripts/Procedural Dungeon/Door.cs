using UnityEngine;

public class Door : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] PlayerController player;
    [SerializeField] RoomInstance roomInstance;
    [SerializeField] Camera mainCamera;

    [Header("Doors")]
    [SerializeField] private Sprite doorOpen;
    [SerializeField] private Sprite doorClosed;
    [SerializeField] private Sprite doorBossOpen;
    [SerializeField] private Sprite doorBossClosed;

    SpriteRenderer spriteRenderer;
    private bool isBossDoor = false;

    //[Header("Bools")]
    [SerializeField] private bool doorTop, doorBottom, doorLeft, doorRight;
    [SerializeField] private bool isOpen = false;
    private bool camSwitchingRoom = false;

    Vector2 moveJump = Vector2.zero;
    Vector3 camJump;
    Vector3 camTempPos;

    private float camSpeedX = 2000f;
    private float camSpeedY = 1200f;

    private float checkBossRayDistanceX;
    private float checkBossRayDistanceY;


    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        roomInstance = GetComponentInParent<RoomInstance>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        mainCamera = Camera.main;

        //Find out where the door is
        if (transform.position.y > roomInstance.RoomCenter().y)
            doorTop = true;
        else if (transform.position.y < roomInstance.RoomCenter().y)
            doorBottom = true;
        else if (transform.position.x < roomInstance.RoomCenter().x)
            doorLeft = true;
        else if (transform.position.x > roomInstance.RoomCenter().x)
            doorRight = true;

        //Set camera jump position
        SheetAssigner SA = FindObjectOfType<SheetAssigner>();
        Vector2 tempJump = SA.roomDimensions + SA.gutterSize;
        camJump = new Vector3(tempJump.x, tempJump.y, 0);

        //Set door raycast
        checkBossRayDistanceX = SA.roomDimensions.x + SA.gutterSize.x;
        checkBossRayDistanceY = SA.roomDimensions.y + SA.gutterSize.y;

        //Numbers based on trial and error testing
        moveJump = new Vector2(172, 98);

        //Set door type to boss door
        if (roomInstance.roomType == RoomType.END)
        {
            isBossDoor = true;
            gameObject.tag = "BossDoor";
        }

        Invoke("CheckForBossRoomNeighbour", 0.1f);
    }

    void Update()
    {
        if (RoomInCamera.enemiesInRoom)
            isOpen = false;
        else
            isOpen = true;

        DecideSprite();
        CalculateCameraPosition();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!isOpen) { return; }

        //Teleport player and cam
        if (collision.CompareTag("Player"))
            TeleportPlayerAndCamera();
    }

    private void DecideSprite()
    {
        if (isOpen && !isBossDoor) //Normal door open
            spriteRenderer.sprite = doorOpen;
        else if (!isOpen && !isBossDoor) //Normal door closed
            spriteRenderer.sprite = doorClosed;
        else if (isOpen && isBossDoor) //Boss door open
            spriteRenderer.sprite = doorBossOpen;
        else if (!isOpen && isBossDoor) //Boss door closed
            spriteRenderer.sprite = doorBossClosed;
    }

    private void TeleportPlayerAndCamera()
    {
        Vector2 movePlayerTempPos = transform.position;
        camTempPos = mainCamera.transform.position;

        if (doorTop)
        {
            movePlayerTempPos += Vector2.up * moveJump.y;
            camTempPos.y += Vector3.up.y * camJump.y;
        }
        else if (doorBottom)
        {
            movePlayerTempPos += Vector2.down * moveJump.y;
            camTempPos.y += Vector3.down.y * camJump.y;
        }
        else if (doorLeft)
        {
            movePlayerTempPos += Vector2.left * moveJump.x;
            camTempPos.x += Vector3.left.x * camJump.x;
        }
        else if (doorRight)
        {
            movePlayerTempPos += Vector2.right * moveJump.x;
            camTempPos.x += Vector3.right.x * camJump.x;
        }

        player.transform.position = movePlayerTempPos;
        player.canMove = false;
        camSwitchingRoom = true;
    }

    private void CalculateCameraPosition()
    {
        if(!camSwitchingRoom) { return; }

        //Camera Y-Movement
        mainCamera.transform.position = Vector3.MoveTowards(mainCamera.transform.position,
                                        new Vector3(mainCamera.transform.position.x, camTempPos.y, mainCamera.transform.position.z), camSpeedY * Time.deltaTime);
        //Camera X-Movement
        mainCamera.transform.position = Vector3.MoveTowards(mainCamera.transform.position,
                                        new Vector3(camTempPos.x, mainCamera.transform.position.y, mainCamera.transform.position.z), camSpeedX * Time.deltaTime);

        if (mainCamera.transform.position == camTempPos)
        {
            camSwitchingRoom = false;
            player.canMove = true;
        }
    }

    private void CheckForBossRoomNeighbour()
    {
        RaycastHit2D hitUP = Physics2D.Raycast(transform.position, Vector2.up, checkBossRayDistanceY);
        RaycastHit2D hitDown = Physics2D.Raycast(transform.position, Vector2.down, checkBossRayDistanceY);
        RaycastHit2D hitRight = Physics2D.Raycast(transform.position, Vector2.right, checkBossRayDistanceX);
        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, Vector2.left, checkBossRayDistanceX);

        if ((hitUP && hitUP.collider.CompareTag("BossDoor")) || (hitDown && hitDown.collider.CompareTag("BossDoor")) ||
            (hitRight && hitRight.collider.CompareTag("BossDoor")) || (hitLeft && hitLeft.collider.CompareTag("BossDoor")))
        {
            isBossDoor = true;
            //gameObject.tag = "BossDoor";
        }
    }

    /*private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Vector2.up * checkBossRayDistanceY);
        Gizmos.DrawRay(transform.position, Vector2.down * checkBossRayDistanceY);
        Gizmos.DrawRay(transform.position, Vector2.left * checkBossRayDistanceX);
        Gizmos.DrawRay(transform.position, Vector2.right * checkBossRayDistanceX);
    }*/
}
