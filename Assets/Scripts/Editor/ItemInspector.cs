using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ItemController))]
public class ItemInspector : Editor
{
    override public void OnInspectorGUI()
    {
        var itemC = target as ItemController;

        itemC.item.itemType = (ItemType)EditorGUILayout.EnumPopup("Item Type", itemC.item.itemType);
        itemC.item.itemName = EditorGUILayout.TextField("Item Name", itemC.item.itemName);
        itemC.item.itemDescription = EditorGUILayout.TextField("Item Description", itemC.item.itemDescription);
        itemC.item.itemImage = (Sprite)EditorGUILayout.ObjectField("Item Image", itemC.item.itemImage, typeof(Sprite), false, GUILayout.Height(EditorGUIUtility.singleLineHeight));

        EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);

        switch (itemC.item.itemType)
        {
            case ItemType.REDHEART or ItemType.BLUEHEART:
                EditorGUILayout.LabelField("Player gets healed by 1 or gets another max heart");
                break;
            case ItemType.SPEED:
                itemC.moveSpeedChange = EditorGUILayout.FloatField("Change Movement Speed", itemC.moveSpeedChange);
                break;
            case ItemType.FIRERATE:
                itemC.firerateChange = EditorGUILayout.FloatField("Change Firerate", itemC.firerateChange);
                break;
            case ItemType.BULLETSIZE:
                itemC.bulletSizeChange = EditorGUILayout.FloatField("Change Bullet Size", itemC.bulletSizeChange);
                itemC.firerateChange = EditorGUILayout.FloatField("Change Firerate", itemC.firerateChange);
                break;
            case ItemType.TRIPLESHOT:
                EditorGUILayout.LabelField("Player now shoots 3 bullets");
                break;
        }
    }
}
