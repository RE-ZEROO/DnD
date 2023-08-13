using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Loot Table")]
public class LootTable : ScriptableObject
{
    [SerializeField] private List<ItemData> itemList;

    [System.NonSerialized] private bool isInitialized = false;

    private float totalWeight;

    private void InitializeDropWeight()
    {
        if (isInitialized) {  return; }

        totalWeight = 0;
        foreach (var item in itemList)
        {
            totalWeight += item.dropWeight;
        }

        isInitialized = true;
    }

    public ItemData GetRandomItem()
    {
        InitializeDropWeight();

        float randomnumber = Random.Range(0f, totalWeight);

        foreach (var item in itemList)
        {
            // If item.weight is greater (or equal) than our randomNumber, we take that item and return
            if (item.dropWeight >= randomnumber)
                return item;

            // If we didn't return, we substract the weight to our randomNumber and cycle to the next item
            randomnumber -= item.dropWeight;
        }

        throw new System.Exception("Item generation failed!");
    }
}