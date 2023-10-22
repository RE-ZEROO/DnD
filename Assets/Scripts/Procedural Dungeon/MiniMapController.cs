using System.Collections.Generic;
using UnityEngine;

public class MiniMapController : MonoBehaviour
{
    [Header("Placeholder Sprites")]
    [SerializeField] private List<GameObject> placeholderSprites = new List<GameObject>();
    MapSpriteSelector[] mapSprites;

    [Header("Current Room")]
    [SerializeField] private RoomInstance currentRoom;
    RoomInstance[] roominstances;
    

    void Start()
    {
        Invoke(nameof(GetObjects), 0.1f);
        InvokeRepeating(nameof(CheckForCurrentRoom), 0.3f, 0.2f);
    }

    void CheckForCurrentRoom()
    {
        foreach (var room in roominstances)
        {
            if (room.isCurrentRoom)
                currentRoom = room;
        }

        HighlightCurrentRoom(currentRoom.roomId);
    }

    private void GetObjects()
    {
        mapSprites = FindObjectsOfType<MapSpriteSelector>();

        foreach (MapSpriteSelector sprites in mapSprites)
        {
            placeholderSprites.Add(sprites.gameObject);
        }

        roominstances = FindObjectsOfType<RoomInstance>();
    }

    public void HighlightCurrentRoom(int roomIndex)
    {
        for (int i = 0; i < placeholderSprites.Count; i++)
        {
            SpriteRenderer spriteRenderer = placeholderSprites[i].GetComponent<SpriteRenderer>();
            MapSpriteSelector sprite = placeholderSprites[i].GetComponent<MapSpriteSelector>();

            if (currentRoom.roomId == sprite.id)
            {
                spriteRenderer.color = sprite.mainColor;

                if(placeholderSprites[i].transform.childCount > 0)
                    Destroy(placeholderSprites[i].transform.GetChild(0).gameObject);
            }
            else
                spriteRenderer.color = sprite.mainColor + new Color(-0.5f, -0.5f, -0.5f);
        }
    }
}
