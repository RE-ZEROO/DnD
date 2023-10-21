using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGenerator : MonoBehaviour
{
    [SerializeField] private LootTable lootTable;
    [SerializeField] private Transform itemHolderPos;

    public void GenerateItem()
    {
        ItemData item = lootTable.GetRandomItem();

        if(itemHolderPos)
            Instantiate(item.itemGO, itemHolderPos.position, Quaternion.identity).transform.parent = itemHolderPos;
        else
            Instantiate(item.itemGO, transform.position, Quaternion.identity);
    }
}