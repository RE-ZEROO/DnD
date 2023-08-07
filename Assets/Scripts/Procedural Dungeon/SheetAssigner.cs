using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheetAssigner : MonoBehaviour
{
    [SerializeField] private Texture2D[] sheetsNormal;
    [SerializeField] private Texture2D startSheet;
    [SerializeField] private Texture2D[] sheetsEnd;
    [SerializeField] private GameObject RoomObj;

    public Vector2 roomDimensions = new Vector2(16 * 7, 16 * 9);
    public Vector2 gutterSize = new Vector2(16 * 9, 16 * 4);

    public void Assign(Room[,] rooms)
    {
        foreach(Room room in rooms)
        {
            //Skip point where there is no room
            if (room == null)
                continue;

            //Start room only has the basic template
            if(room.type == RoomType.START)
            {
                Vector3 startPos = new Vector3(room.gridPos.x * (roomDimensions.x + gutterSize.x), room.gridPos.y * (roomDimensions.y + gutterSize.y));
                RoomInstance startRoom = Instantiate(RoomObj, startPos, Quaternion.identity).GetComponent<RoomInstance>();
                startRoom.Setup(startSheet, room.gridPos, room.type, room.doorTop, room.doorBottom, room.doorLeft, room.doorRight);

                continue;
            }

            if (room.type == RoomType.END)
            {
                int _index = Mathf.RoundToInt(Random.value * (sheetsEnd.Length - 1));

                Vector3 endPos = new Vector3(room.gridPos.x * (roomDimensions.x + gutterSize.x), room.gridPos.y * (roomDimensions.y + gutterSize.y));
                RoomInstance endRoom = Instantiate(RoomObj, endPos, Quaternion.identity).GetComponent<RoomInstance>();
                endRoom.Setup(sheetsEnd[_index], room.gridPos, room.type, room.doorTop, room.doorBottom, room.doorLeft, room.doorRight);

                continue;
            }


            //Pick a random index for the array
            int index = Mathf.RoundToInt(Random.value * (sheetsNormal.Length - 1));

            //Find position to place room
            Vector3 pos = new Vector3(room.gridPos.x * (roomDimensions.x + gutterSize.x), room.gridPos.y * (roomDimensions.y + gutterSize.y));

            RoomInstance currentRoom = Instantiate(RoomObj, pos, Quaternion.identity).GetComponent<RoomInstance>();
            currentRoom.Setup(sheetsNormal[index], room.gridPos, room.type, room.doorTop, room.doorBottom, room.doorLeft, room.doorRight);
        }
    }
}
