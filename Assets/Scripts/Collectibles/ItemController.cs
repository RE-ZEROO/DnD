/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    REDHEART,
    BLUEHEART,
    SPEED,
    FIRERATE,
    BULLETSIZE,
    TRIPLESHOT
}

[System.Serializable]
public class Item2
{
    public ItemType itemType; 
    public Sprite itemImage;
    public string itemName;
    public string itemDescription;
    public int dropChance;
}

public class ItemController : MonoBehaviour
{
    public Item2 item;

    public float moveSpeedChange;
    public float firerateChange;
    public float bulletSizeChange;


    void Start()
    {
        GetComponent<SpriteRenderer>().sprite = item.itemImage;

        Destroy(GetComponent<PolygonCollider2D>());
        gameObject.AddComponent<PolygonCollider2D>().isTrigger = true;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!collision.CompareTag("Player")) { return; }
        
        switch (item.itemType)
        {
            case ItemType.REDHEART or ItemType.BLUEHEART:
                GameController.HealPlayer(item.itemType);
                break;
            case ItemType.SPEED:
                GameController.MoveSpeedChange(moveSpeedChange);
                break;
            case ItemType.FIRERATE:
                GameController.FireRateChange(firerateChange);
                break;
            case ItemType.BULLETSIZE:
                GameController.BulletSizeChange(bulletSizeChange);
                GameController.FireRateChange(firerateChange);
                break;
            case ItemType.TRIPLESHOT:
                GameController.TripleShot();    
                break;
        }

        //If health is full, redheart can't be picked up
        if (item.itemType == ItemType.REDHEART && GameController.Health == GameController.MaxHealth) { return; }

        GameController.Instance.UpdateCollectedItems(this);
        Debug.Log("Added Item");

        Destroy(gameObject);
    }
}*/