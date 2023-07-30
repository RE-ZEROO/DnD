using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] PlayerController player;
    [SerializeField] RoomInstance room;
    [SerializeField] Camera mainCamera;

    SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite doorOpen;
    [SerializeField] private Sprite doorClosed;

    Vector2 moveJump = Vector2.zero;
    Vector3 camJump;
    Vector3 camTempPos;

    [SerializeField] private bool doorTop, doorBottom, doorLeft, doorRight;
    [SerializeField] private bool isOpen = false;
    private bool camSwitchingRoom = false;

    private float camSpeedX = 2000f;
    private float camSpeedY = 1200f;

    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        room = GetComponentInParent<RoomInstance>();
        spriteRenderer = GetComponentInParent<SpriteRenderer>();
        mainCamera = Camera.main;

        if (transform.position.y > room.RoomCenter().y)
            doorTop = true;
        else if (transform.position.y < room.RoomCenter().y)
            doorBottom = true;
        else if (transform.position.x < room.RoomCenter().x)
            doorLeft = true;
        else if (transform.position.x > room.RoomCenter().x)
            doorRight = true;

        SheetAssigner SA = FindObjectOfType<SheetAssigner>();
        Vector2 tempJump = SA.roomDimensions + SA.gutterSize;
        camJump = new Vector3(tempJump.x, tempJump.y, 0);

        //Numbers based on trial and error testing
        moveJump = new Vector2(172, 98);
    }

    void Update()
    {
        if (RoomInCamera.enemiesInRoom)
            isOpen = false;
        else
            isOpen = true;

        if(isOpen)
            spriteRenderer.sprite = doorOpen;
        else if (!isOpen)
            spriteRenderer.sprite = doorClosed;

        if (camSwitchingRoom)
        {
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

       // Debug.Log(player.canMove);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!isOpen) { return; }

        //Teleport Player
        if (collision.CompareTag("Player"))
        {
            Vector2 moveTempPos = transform.position;
            camTempPos = mainCamera.transform.position;

            if (doorTop)
            {
                moveTempPos += Vector2.up * moveJump.y;
                camTempPos.y += Vector3.up.y * camJump.y;
            }
            else if (doorBottom)
            {
                moveTempPos += Vector2.down * moveJump.y;
                camTempPos.y += Vector3.down.y * camJump.y;
            }
            else if (doorLeft)
            {
                moveTempPos += Vector2.left * moveJump.x;
                camTempPos.x += Vector3.left.x * camJump.x;
            }
            else if (doorRight)
            {
                moveTempPos += Vector2.right * moveJump.x;
                camTempPos.x += Vector3.right.x * camJump.x;
            }

            player.transform.position = moveTempPos;
            player.canMove = false;
            camSwitchingRoom = true;
        }
    }
}
