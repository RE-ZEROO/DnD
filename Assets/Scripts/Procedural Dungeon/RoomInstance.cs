using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ColorToGameObject
{
    public Color color;
    public GameObject[] prefabs;
}

public class RoomInstance : MonoBehaviour
{
    public Texture2D tex;
    [SerializeField] private RoomType type;

    [HideInInspector] public Vector2 gridPos;
    [HideInInspector] public bool doorTop, doorBottom, doorLeft, doorRight;
    [HideInInspector] public bool doorBossTop, doorBossBottom, doorBossLeft, doorBossRight;

    [HideInInspector] public bool isCurrentRoom;

    [SerializeField] private GameObject doorU, doorD, doorL, doorR;
    [SerializeField] private GameObject doorWallT, doorWallB, doorWallL, doorWallR;
    [SerializeField] private GameObject doorBU, doorBD, doorBL, doorBR;
    [Space]
    [SerializeField] private GameObject[] groundTiles;
    [Space]
    [SerializeField] private ColorToGameObject[] mappings;

    private float tileSize = 16;
    public Vector2 roomSizeInTiles = new Vector2(9, 17);
    private Vector2 roomCenterTilePos = Vector2.zero;

    private float checkBossRayDistanceX = 145f;
    private float checkBossRayDistanceY = 290f;


    public void Setup(Texture2D _texture, Vector2 _gridPos, RoomType _type,
                        bool _doorTop, bool _doorBottom, bool _doorLeft, bool _doorRight,
                        bool _doorBossTop, bool _doorBossBottom, bool _doorBossLeft, bool _doorBossRight)
    {
        tex = _texture;
        gridPos = _gridPos;
        type = _type;

        doorTop = _doorTop;
        doorBottom = _doorBottom;
        doorLeft = _doorLeft;
        doorRight = _doorRight;

        doorBossTop = _doorBossTop;
        doorBossBottom = _doorBossBottom;
        doorBossLeft = _doorBossLeft;
        doorBossRight = _doorBossRight;

        MakeDoors();
        GenerateRoomTiles();
        //CheckForBossRoomNeighbour();

        gameObject.name = gameObject.name + " - " + gridPos;
    }

    public Vector2 RoomCenter()
    {
        roomCenterTilePos = transform.position;
        return roomCenterTilePos;
    }

    private void CheckForBossRoomNeighbour()
    {
        if (Physics2D.Raycast(transform.position += new Vector3(roomSizeInTiles.x, 0), Vector2.up, checkBossRayDistanceX).collider.CompareTag("BossDoor"))
            Debug.Log("BossDoorTop" + gridPos);
        else if (Physics2D.Raycast(transform.position, Vector2.down, checkBossRayDistanceX).collider.CompareTag("BossDoor"))
            doorBossBottom = true;
        else if (Physics2D.Raycast(transform.position, Vector2.left, checkBossRayDistanceY).collider.CompareTag("BossDoor"))
            doorBossLeft = true;
        else if (Physics2D.Raycast(transform.position, Vector2.right, checkBossRayDistanceY).collider.CompareTag("BossDoor"))
            doorBossRight = true;


        /*if (hit.collider.GetComponent<RoomInstance>().type == RoomType.END)
            doorBossTop = true;
        else if (hit.collider.GetComponent<RoomInstance>().type == RoomType.END)*/
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //Gizmos.DrawRay(transform.position, Vector2.up * checkBossRayDistanceX);
        /*Gizmos.DrawRay(transform.position, Vector2.down * checkBossRayDistanceX);
        Gizmos.DrawRay(transform.position, Vector2.left * checkBossRayDistanceY);
        Gizmos.DrawRay(transform.position, Vector2.right * checkBossRayDistanceY);*/
    }


    private void PlaceDoor(Vector3 spawnPos, bool door, GameObject doorSpawn)
    {
        //Spawn either a door or a wall
        if (door)
            Instantiate(doorSpawn, spawnPos, Quaternion.identity).transform.parent = transform;
        else
        {
            if(spawnPos.y > RoomCenter().y)
                Instantiate(doorWallT, spawnPos, Quaternion.identity).transform.parent = transform;
            else if(spawnPos.y < RoomCenter().y)
                Instantiate(doorWallB, spawnPos, Quaternion.identity).transform.parent = transform;
            else if (spawnPos.x > RoomCenter().x)
                Instantiate(doorWallR, spawnPos, Quaternion.identity).transform.parent = transform;
            else if (spawnPos.x < RoomCenter().x)
                Instantiate(doorWallL, spawnPos, Quaternion.identity).transform.parent = transform;
        }
            //Instantiate(doorWallT, spawnPos, Quaternion.identity).transform.parent = transform;
    }

    private void MakeDoors()
    {
        //Top door, get position then spawn
        Vector3 spawnPos = transform.position + Vector3.up * (roomSizeInTiles.y / 4 * tileSize) - Vector3.up * (tileSize / 4);
        if (type == RoomType.END)
            PlaceDoor(spawnPos, doorBossTop, doorBU);
        else
            PlaceDoor(spawnPos, doorTop, doorU);

        //Bottom door
        spawnPos = transform.position + Vector3.down * (roomSizeInTiles.y / 4 * tileSize) - Vector3.down * (tileSize / 4);
        if (type == RoomType.END)
            PlaceDoor(spawnPos, doorBossBottom, doorBD);
        else
            PlaceDoor(spawnPos, doorBottom, doorD);

        //Left door
        spawnPos = transform.position + Vector3.left * (roomSizeInTiles.x * tileSize) - Vector3.left * tileSize;
        if (type == RoomType.END)
            PlaceDoor(spawnPos, doorBossLeft, doorBL);
        else
            PlaceDoor(spawnPos, doorLeft, doorL);

        //Right door
        spawnPos = transform.position + Vector3.right * (roomSizeInTiles.x * tileSize) - Vector3.right * tileSize;
        if (type == RoomType.END)
            PlaceDoor(spawnPos, doorBossRight, doorBR);
        else
            PlaceDoor(spawnPos, doorRight, doorR);
    }

    
    private void GenerateRoomTiles()
    {
        //Nested loop allows for checking every pixel in the template
        for(int x = 0; x < tex.width; x++)
        {
            for(int y = 0; y < tex.height; y++)
            {
                GenerateTile(x, y);
            }
        }
    }

    private int SelectGroundTiles()
    {
        int randomIndex = Random.Range(0, groundTiles.Length); //if null exception: length -1

        return randomIndex;
    }

    private void GenerateTile(int x, int y)
    {
        Color pixelColor = tex.GetPixel(x, y);

        //Place ground tile when pixel color is transparent
        if (pixelColor.a == 0)
        {
            Vector3 spawnPos = PositionFromTileGrid(x, y);
            Instantiate(groundTiles[SelectGroundTiles()], spawnPos, Quaternion.identity).transform.parent = transform;
        }

        //Find the color to match the pixel
        foreach (ColorToGameObject mapping in mappings)
        {
            if (mapping.color.Equals(pixelColor))
            {
                if (pixelColor.a == 0) { return; }

                int randomIndex = Random.Range(0, mapping.prefabs.Length);

                Vector3 spawnPos = PositionFromTileGrid(x, y);
                Instantiate(mapping.prefabs[randomIndex], spawnPos, Quaternion.identity).transform.parent = transform;

                //Update the A* grid when the tile is an obstacle
                //if(mapping.prefabs[randomIndex].GetComponent<Collider2D>() == null) { return; }
                //AstarPath.active.UpdateGraphs(mapping.prefabs[randomIndex].GetComponent<Collider2D>().bounds);
            }
        }
    }

    private Vector3 PositionFromTileGrid(int x, int y)
    {
        Vector3 ret;

        //Find difference between the corner of the texture and the center of this object
        Vector3 offset = new Vector3((-roomSizeInTiles.x + 1) * tileSize, (roomSizeInTiles.y / 4) * tileSize - (tileSize / 4), 0);

        //Find scaled up position at the offset
        ret = new Vector3(tileSize * (float)x, -tileSize * (float)y, 0) + offset + transform.position;

        return ret;
    }
}
