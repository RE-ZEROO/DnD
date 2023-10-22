using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RoomType{
    NORMAL,
    START,
    END
}

public class Room
{
    public Vector2 gridPos;
    public RoomType type;

    public int roomId;

    public bool doorTop;
    public bool doorBottom;
    public bool doorLeft;
    public bool doorRight;

    public Room (Vector2 _gridPos, RoomType _type, int roomId)
    {
        gridPos = _gridPos;
        type = _type;
        this.roomId = roomId;
    }
}
