using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGenerator : MonoBehaviour
{
    [SerializeField] private LootTable lootTable;
    [SerializeField] private Transform itemHolderPos;

    public void GenerateLoot()
    {
        ItemData item = lootTable.GetRandomItem();
        Instantiate(item.itemGO, itemHolderPos.position, Quaternion.identity).transform.parent = itemHolderPos;
    }
}