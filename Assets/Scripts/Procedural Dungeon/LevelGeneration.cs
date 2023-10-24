using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LevelGeneration : MonoBehaviour
{
    private static Room[,] rooms;
    //MapSpriteSelector currentRoomSprite;
    //private RoomType type;

    Vector2 worldSize = new Vector2(4, 4); //Actual size is doubled
    List<Vector2> takenPositions = new List<Vector2>();

    private int gridSizeX;
    private int gridSizeY;
    private int numberOfRooms = 20;

    private bool isThereABossRoom = false;

    public GameObject roomWhiteObj;
    public GameObject roomWhiteObjCover;
    public Transform mapRoot;


    void Start()
    {
        //Make sure that there are not more rooms than could fit in the grid
        if (numberOfRooms >= (worldSize.x * 2) * (worldSize.y * 2))
            numberOfRooms = Mathf.RoundToInt((worldSize.x * 2) * (worldSize.y * 2));

        gridSizeX = Mathf.RoundToInt(worldSize.x);
        gridSizeY = Mathf.RoundToInt(worldSize.y);


        CreateRooms();
        SetRoomDoors();
        DrawMap();
        GetComponent<SheetAssigner>().Assign(rooms);
        AstarPath.active.Scan();
    }

    private void CreateRooms()
    {
        //Setup
        rooms = new Room[gridSizeX * 2, gridSizeY * 2];
        rooms[gridSizeX, gridSizeY] = new Room(Vector2.zero, RoomType.START, 0);
        takenPositions.Insert(0, Vector2.zero);
        Vector2 checkPos = Vector2.zero;

        //Magic numbers
        float randomCompare = 0.2f;
        float randomCompareStart = 0.2f;
        float randomCompareEnd = 0.01f;

        //Add rooms
        for (int i = 0; i < numberOfRooms -1; i++)
        {
            float randomPercentage = ((float)i) / ((float)numberOfRooms - 1);
            randomCompare = Mathf.Lerp(randomCompareStart, randomCompareEnd, randomPercentage); //The farther in the loop, the less chance to branch out

            //Grab new Position
            checkPos = NewPosition();

            //Branch out rooms more based on the magic numbers
            if (NumberOfNeighbors(checkPos, takenPositions) > 1 && Random.value > randomCompare)
            {
                int iterations = 0;

                //Find a position that has only one neighbor
                do{
                    checkPos = SelectiveNewPosition();
                    iterations++;
                } while (NumberOfNeighbors(checkPos, takenPositions) > 1 && iterations < 100);

                if (iterations >= 50)
                    Debug.Log("Error: could not create with fewer neighbors than: " + NumberOfNeighbors(checkPos, takenPositions));
            }

            //Add position to array and list
            rooms[(int)checkPos.x + gridSizeX, (int)checkPos.y + gridSizeY] = new Room(checkPos, RoomType.NORMAL, i + 1); //Calculating offset when added to array
            takenPositions.Insert(0, checkPos);
        }

        //Check for rooms with one neighbor and set it as a boss room
        for (int i = 0; i < numberOfRooms - 1; i++)
        {
            if (isThereABossRoom) { return; }

            if (NumberOfNeighbors(checkPos, takenPositions) == 1) //&& isThereABossRoom == false
            {
                takenPositions.RemoveAt(i);

                var bossRoom = rooms[(int)checkPos.x + gridSizeX, (int)checkPos.y + gridSizeY] = new Room(checkPos, RoomType.END, numberOfRooms - 1);
                takenPositions.Insert(i, checkPos);

                isThereABossRoom = true;
            }
        }
    }

    //Get a valid position that is neighboring to a room and already exits
    private Vector2 NewPosition()
    {
        Vector2 checkingPos = Vector2.zero;
        int x = 0;
        int y = 0;

        do{
            //Get a random taken position from the list
            int index = Mathf.RoundToInt(Random.value * (takenPositions.Count - 1));
            x = (int)takenPositions[index].x;
            y = (int)takenPositions[index].y;

            //Randomly select up, down, left, right
            bool UpDown = (Random.value < 0.5f); //Randomly select X or Y-Axis
            bool positive = (Random.value < 0.5f); //Randomly select the positive or negative direction

            if (UpDown)
            {
                if (positive)
                    y++;
                else
                    y--;
            }
            else
            {
                if (positive)
                    x++;
                else
                    x--;
            }

            checkingPos = new Vector2(x, y);

        } while (takenPositions.Contains(checkingPos) || x >= gridSizeX || x < -gridSizeX || y >= gridSizeY || y < -gridSizeY);

        return checkingPos;
    }


    //Adds to the branching effect
    private Vector2 SelectiveNewPosition()
    {
        Vector2 checkingPos = Vector2.zero;
        int index = 0;
        int increment = 0;
        int x = 0;
        int y = 0;

        do{
            increment = 0;

            //Only grab a room that has one neighbour => Makes it more likely to branch out
            do{
                index = Mathf.RoundToInt(Random.value * (takenPositions.Count - 1));
                increment++;
            } while (NumberOfNeighbors(takenPositions[index], takenPositions) > 1 && increment < 100);
            
            x = (int)takenPositions[index].x;
            y = (int)takenPositions[index].y;

            //Randomly select up, down, left, right
            bool UpDown = (Random.value < 0.5f);
            bool positive = (Random.value < 0.5f);

            if (UpDown)
            {
                if (positive)
                    y++;
                else
                    y--;
            }
            else
            {
                if (positive)
                    x++;
                else
                    x--;
            }

            checkingPos = new Vector2(x, y);

        } while (takenPositions.Contains(checkingPos) || x >= gridSizeX || x < -gridSizeX || y >= gridSizeY || y < -gridSizeY);

        if (increment >= 100)
            Debug.Log("Error: could not find position with only one neighbour");

        return checkingPos;
    }

    //Each neighbour of a room, increments the roomNeighborNumber
    private int NumberOfNeighbors(Vector2 checkingPos, List<Vector2> usedPositions)
    {
        int roomNeighbourNumber = 0;

        //Right
        if (usedPositions.Contains(checkingPos + Vector2.right))
            roomNeighbourNumber++;

        //Left
        if (usedPositions.Contains(checkingPos + Vector2.left))
            roomNeighbourNumber++;

        //Up
        if (usedPositions.Contains(checkingPos + Vector2.up))
            roomNeighbourNumber++;

        //Down
        if (usedPositions.Contains(checkingPos + Vector2.down))
            roomNeighbourNumber++;

        return roomNeighbourNumber;
    }


    private void DrawMap()
    {
        foreach (Room room in rooms)
        {
            if (room == null)
                continue;

            
            //Grab the position of a room and multiply by size of map sprite
            Vector2 drawPos = room.gridPos;
            drawPos.x *= 16;
            drawPos.y *= 8;

            //Instantiate and set variables to the room it represents
            MapSpriteSelector mapper = Instantiate(roomWhiteObj, drawPos, Quaternion.identity).GetComponent<MapSpriteSelector>();
            mapper.type = room.type;
            mapper.mapId = room.roomId;

            mapper.up = room.doorTop;
            mapper.down = room.doorBottom;
            mapper.left = room.doorLeft;
            mapper.right = room.doorRight;

            mapper.gameObject.transform.parent = mapRoot;


            //Instantiate MiniMap cover
            GameObject mapperCover = Instantiate(roomWhiteObjCover, drawPos, Quaternion.identity, mapper.transform);
            mapperCover.transform.position = mapper.transform.position;
            mapperCover.GetComponent<SpriteRenderer>().sprite = mapper.GetComponent<SpriteRenderer>().sprite;
            mapperCover.GetComponent<SpriteRenderer>().color = mapper.backgroundColor;
        }
    }

    void SetRoomDoors()
    {
        //Nested foor-loops allow to check every position in a 2D Array
        for (int x = 0; x < ((gridSizeX * 2)); x++)
        {
            for (int y = 0; y < ((gridSizeY * 2)); y++)
            {
                //If there is no room, continue to next check in the loop
                if (rooms[x, y] == null)
                    continue;
                
                Vector2 gridPosition = new Vector2(x, y);

                //check above
                if (y - 1 < 0)
                    rooms[x, y].doorBottom = false;
                else
                    rooms[x, y].doorBottom = (rooms[x, y - 1] != null);

                //check bellow
                if (y + 1 >= gridSizeY * 2)
                    rooms[x, y].doorTop = false;
                else
                    rooms[x, y].doorTop = (rooms[x, y + 1] != null);

                //check left
                if (x - 1 < 0)
                    rooms[x, y].doorLeft = false;
                else
                    rooms[x, y].doorLeft = (rooms[x - 1, y] != null);

                //check right
                if (x + 1 >= gridSizeX * 2)
                    rooms[x, y].doorRight = false;
                else
                    rooms[x, y].doorRight = (rooms[x + 1, y] != null);
            }
        }
    }
}