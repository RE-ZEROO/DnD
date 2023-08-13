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
    [SerializeField] public RoomType roomType;

    [HideInInspector] public Vector2 gridPos;
    [HideInInspector] public bool doorTop, doorBottom, doorLeft, doorRight;
    [HideInInspector] public bool doorBossTop, doorBossBottom, doorBossLeft, doorBossRight;

    [HideInInspector] public bool isCurrentRoom;

    [SerializeField] private GameObject doorU, doorD, doorL, doorR;
    [SerializeField] private GameObject doorWallT, doorWallB, doorWallL, doorWallR;
    //[SerializeField] public GameObject doorBU, doorBD, doorBL, doorBR;

    [Header("Boss Monsters")]
    [SerializeField] private GameObject[] bossMonsterPrefab;
    public GameObject portal;

    [Header("Ground Tiles")]
    [SerializeField] private GameObject[] groundTiles;

    [Header("Color Maping")]
    [SerializeField] private ColorToGameObject[] mappings;

    private float tileSize = 16;
    public Vector2 roomSizeInTiles = new Vector2(9, 17);
    private Vector2 roomCenterTilePos = Vector2.zero;



    public void Setup(Texture2D _texture, Vector2 _gridPos, RoomType _type,
                        bool _doorTop, bool _doorBottom, bool _doorLeft, bool _doorRight)
    {
        tex = _texture;
        gridPos = _gridPos;
        roomType = _type;

        doorTop = _doorTop;
        doorBottom = _doorBottom;
        doorLeft = _doorLeft;
        doorRight = _doorRight;

        MakeDoors();
        GenerateRoomTiles();
        SpawnBoss();

        gameObject.name = gameObject.name + " - " + gridPos;
    }

    public Vector2 RoomCenter()
    {
        roomCenterTilePos = transform.position;
        return roomCenterTilePos;
    }

    private void SpawnBoss()
    {
        if(roomType != RoomType.END) { return; }

        int randomBossIndex = Random.Range(0, bossMonsterPrefab.Length);

        Instantiate(bossMonsterPrefab[randomBossIndex], RoomCenter(), Quaternion.identity).transform.parent = transform;
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
        PlaceDoor(spawnPos, doorTop, doorU);

        //Bottom door
        spawnPos = transform.position + Vector3.down * (roomSizeInTiles.y / 4 * tileSize) - Vector3.down * (tileSize / 4);
        PlaceDoor(spawnPos, doorBottom, doorD);

        //Left door
        spawnPos = transform.position + Vector3.left * (roomSizeInTiles.x * tileSize) - Vector3.left * tileSize;
        PlaceDoor(spawnPos, doorLeft, doorL);

        //Right door
        spawnPos = transform.position + Vector3.right * (roomSizeInTiles.x * tileSize) - Vector3.right * tileSize;
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

        if(pixelColor.a == 0) { return; }

        //Place ground tile when pixel color is transparent
        /*if (pixelColor.a == 0)
        {
            Vector3 spawnPos = PositionFromTileGrid(x, y);
            Instantiate(groundTiles[SelectGroundTiles()], spawnPos, Quaternion.identity).transform.parent = transform;
        }*/

        //Find the color to match the pixel
        foreach (ColorToGameObject mapping in mappings)
        {
            if (mapping.color.Equals(pixelColor))
            {
                if (pixelColor.a == 0) { return; }

                int randomIndex = Random.Range(0, mapping.prefabs.Length);

                Vector3 spawnPos = PositionFromTileGrid(x, y);
                Instantiate(mapping.prefabs[randomIndex], spawnPos, Quaternion.identity).transform.parent = transform;
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
