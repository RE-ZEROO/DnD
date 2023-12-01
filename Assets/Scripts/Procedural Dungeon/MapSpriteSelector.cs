using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSpriteSelector : MonoBehaviour
{
    public Sprite spU, spD, spR, spL,
            spUD, spRL, spUR, spUL, spDR, spDL,
            spULD, spRUL, spDRU, spLDR, spUDRL;

    [Header("Door Placement")]
    public bool up;
    public bool down;
    public bool left;
    public bool right;

    [Header("Specifics")]
    public RoomType type;
    public int mapId = 0;

    [Header("Colors")]
    [SerializeField] private Color normalColor;
    [SerializeField] private Color startColor;
    [SerializeField] private Color bossColor;

    public Color mainColor;
    private SpriteRenderer rend;


    void Start()
    {
        rend = GetComponent<SpriteRenderer>();
        mainColor = normalColor;

        PickSprite();
        PickColor();
    }

    void PickSprite()
    {
        int spriteIndex = 0;

        if (up)
            spriteIndex += 8;

        if (down)
            spriteIndex += 4;

        if (right)
            spriteIndex += 2;

        if (left)
            spriteIndex += 1;

        switch (spriteIndex)
        {
            case 1: //left
                rend.sprite = spL;
                break;
            case 2: //right
                rend.sprite = spR;
                break;
            case 3: //left, right
                rend.sprite = spRL;
                break;
            case 4: //down
                rend.sprite = spD;
                break;
            case 5: //down, left
                rend.sprite = spDL;
                break;
            case 6: //down, right
                rend.sprite = spDR;
                break;
            case 7: //down, left, right
                rend.sprite = spLDR;
                break;
            case 8: //up
                rend.sprite = spU;
                break;
            case 9: //up, left
                rend.sprite = spUL;
                break;
            case 10: //up, right
                rend.sprite = spUR;
                break;
            case 11: //up, left, right
                rend.sprite = spRUL;
                break;
            case 12: //up, down
                rend.sprite = spUD;
                break;
            case 13: //up, down, left
                rend.sprite = spULD;
                break;
            case 14: //up, down, right
                rend.sprite = spDRU;
                break;
            case 15: //All doors
                rend.sprite = spUDRL;
                break;
        }
    }

    private void PickColor()
    {
        if (type == RoomType.NORMAL)
            mainColor = normalColor;
        else if (type == RoomType.START)
            mainColor = startColor;
        else if (type == RoomType.END)
            mainColor = bossColor;

        rend.color = mainColor;
    }
}
