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

    private RoomStreamer roomStreamer;

    void Start()
    {
        roomStreamer = FindObjectOfType<RoomStreamer>();
        Invoke(nameof(GetObjects), 0.1f);
        InvokeRepeating(nameof(CheckForCurrentRoom), 0.3f, 0.2f);
    }

    void CheckForCurrentRoom()
    {
        if (roomStreamer != null && roomStreamer.currentRoom != null)
        {
            currentRoom = roomStreamer.currentRoom;
            HighlightCurrentRoom(currentRoom.roomId);
        }
    }

    private void GetObjects()
    {
        mapSprites = FindObjectsOfType<MapSpriteSelector>(true);

        foreach (MapSpriteSelector sprites in mapSprites)
        {
            placeholderSprites.Add(sprites.gameObject);
        }

        roominstances = FindObjectsOfType<RoomInstance>();
    }

    private void HighlightCurrentRoom(int roomIndex)
    {
        for (int i = 0; i < placeholderSprites.Count; i++)
        {
            SpriteRenderer spriteRenderer = placeholderSprites[i].GetComponent<SpriteRenderer>();
            MapSpriteSelector sprite = placeholderSprites[i].GetComponent<MapSpriteSelector>();

            if (currentRoom.roomId == sprite.mapId)
            {
                if (!spriteRenderer.enabled)
                    spriteRenderer.enabled = true;

                spriteRenderer.color = sprite.currentColor;
            }
            else
                spriteRenderer.color = sprite.currentColor + new Color(-0.5f, -0.5f, -0.5f);
        }
    }
}
