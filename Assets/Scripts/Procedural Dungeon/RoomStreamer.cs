using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomStreamer : MonoBehaviour
{
    [Header("References")]
    private List<RoomInstance> loadedRooms = new List<RoomInstance>();
    private Coroutine updateRoomStatesCoroutine;
    [HideInInspector] public RoomInstance[] allRooms { get; private set; }

    [SerializeField] public RoomInstance currentRoom { get; private set; }

    [Header("Debug")]
    [SerializeField] private bool showDebugGizmos = true;
    [SerializeField] private Color startRoomColor = Color.green;
    [SerializeField] private Color BossRoomColor = Color.red;
    [SerializeField] private Color loadedRoomColor = Color.yellow;
    [SerializeField] private Color unloadedRoomColor = Color.white;

    void Start()
    {
        allRooms = FindObjectsOfType<RoomInstance>(true);

        foreach (RoomInstance room in allRooms)
        {
            if (room.roomType == RoomType.START)
            {
                currentRoom = room;
                currentRoom.isCurrentRoom = true;
                currentRoom.gameObject.SetActive(true);
                loadedRooms.Add(currentRoom);
                break;
            }
        }

        updateRoomStatesCoroutine = StartCoroutine(UpdateRoomStates());
    }

    public void SetCurrentRoom(RoomInstance newCurrentRoom)
    {
        if (currentRoom != null)
            currentRoom.isCurrentRoom = false;

        currentRoom = newCurrentRoom;
        currentRoom.isCurrentRoom = true;

        if (updateRoomStatesCoroutine != null)
            StopCoroutine(updateRoomStatesCoroutine);

        updateRoomStatesCoroutine = StartCoroutine(UpdateRoomStates());
    }

    private IEnumerator UpdateRoomStates()
    {
        //Keep track of which rooms should be loaded
        List<RoomInstance> shouldBeLoaded = new List<RoomInstance>();
        shouldBeLoaded.Add(currentRoom);

        foreach (RoomInstance room in allRooms)
        {
            if (IsRoomConnectedToCurrentRoom(room))
            {
                shouldBeLoaded.Add(room);
            }
        }

        //Unload rooms that shouldn't be loaded
        List<RoomInstance> roomsToRemove = new List<RoomInstance>();
        foreach (RoomInstance room in loadedRooms)
        {
            if (!shouldBeLoaded.Contains(room))
            {
                room.gameObject.SetActive(false);
                roomsToRemove.Add(room);
                yield return null;
            }
        }

        foreach (RoomInstance room in roomsToRemove)
        {
            loadedRooms.Remove(room);
        }

        //Load rooms that should be loaded
        foreach (RoomInstance room in shouldBeLoaded)
        {
            if (!loadedRooms.Contains(room))
            {
                room.gameObject.SetActive(true);
                loadedRooms.Add(room);
                yield return null;
            }
        }
    }

    private bool IsRoomConnectedToCurrentRoom(RoomInstance room)
    {
        if (room == currentRoom)
            return false;

        bool adjacentX = Mathf.Abs(room.gridPos.x - currentRoom.gridPos.x) == 1 && room.gridPos.y == currentRoom.gridPos.y;
        bool adjacentY = Mathf.Abs(room.gridPos.y - currentRoom.gridPos.y) == 1 && room.gridPos.x == currentRoom.gridPos.x;

        if (!adjacentX && !adjacentY)
            return false;

        if (adjacentX)
        {
            if (room.gridPos.x > currentRoom.gridPos.x)
                return currentRoom.doorRight && room.doorLeft;
            else
                return currentRoom.doorLeft && room.doorRight;
        }
        else
        {
            if (room.gridPos.y > currentRoom.gridPos.y)
                return currentRoom.doorTop && room.doorBottom;
            else
                return currentRoom.doorBottom && room.doorTop;
        }
    }

    public RoomInstance GetRoomAtGridPosition(Vector2 gridPos)
    {
        foreach (RoomInstance room in allRooms)
        {
            if (room.gridPos == gridPos)
                return room;
        }
        return null;
    }

    private void OnDrawGizmos()
    {
        if (!showDebugGizmos)
            return;

        if (allRooms == null)
            allRooms = FindObjectsOfType<RoomInstance>();

        foreach (RoomInstance room in allRooms)
        {
            if (room == null)
                continue;

            if (room.roomType == RoomType.START)
                Gizmos.color = startRoomColor;
            else if (room.roomType == RoomType.END)
                Gizmos.color = BossRoomColor;
            else if (loadedRooms.Contains(room))
                Gizmos.color = loadedRoomColor;
            else
                Gizmos.color = unloadedRoomColor;

            Vector3 center = room.transform.position;
            Vector3 size = new Vector3(room.roomSizeInTiles.y * 16, room.roomSizeInTiles.x * 16, 1);
            Gizmos.DrawWireCube(center, size);
        }
    }
}
