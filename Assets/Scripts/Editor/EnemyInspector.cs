


//!!!!!!!!!!!!!!!!!!Values are being shown but can't be changed!!!!!!!!!!!!!!!!!!


/*using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnemyController), true)]
public class EnemyInspector : Editor
{
    private EnemyState currentState;
    private EnemyType enemyType;
    
    private SerializedProperty health;
    private SerializedProperty speed;
    private SerializedProperty detectionRange;
    private SerializedProperty attackRange;

    private SerializedProperty bulletPrefab;
    private SerializedProperty bulletSpeed;
    private SerializedProperty cooldown;

    protected virtual void OnEnable()
    {
        health = serializedObject.FindProperty("health");
        speed = serializedObject.FindProperty("speed");
        detectionRange = serializedObject.FindProperty("detectionRange");
        attackRange = serializedObject.FindProperty("attackRange");

        bulletPrefab = serializedObject.FindProperty("bulletPrefab");
        bulletSpeed = serializedObject.FindProperty("bulletSpeed");
        cooldown = serializedObject.FindProperty("cooldown");
    }


    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();

        DrawComponents();

        serializedObject.ApplyModifiedProperties();
    }

    protected virtual void DrawComponents()
    {
        EnemyController enemyController = (EnemyController)target;
        enemyController.currentState = (EnemyState)EditorGUILayout.EnumPopup("Enemy State", currentState);
        enemyController.enemyType = (EnemyType)EditorGUILayout.EnumPopup("Enemy Type", enemyType);

        EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
        EditorGUILayout.LabelField("Base Stats", EditorStyles.boldLabel);

        EditorGUILayout.FloatField("Health", health.floatValue);
        EditorGUILayout.FloatField("Speed", speed.floatValue);
        EditorGUILayout.FloatField("Detection Range", detectionRange.floatValue);
        EditorGUILayout.FloatField("Attack Range", attackRange.floatValue);

        if(enemyType == EnemyType.RANGED || enemyType == EnemyType.STATIONARY)
        {
            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
            EditorGUILayout.LabelField("Range Stats", EditorStyles.boldLabel);

            EditorGUILayout.ObjectField("Bullet Prefab", bulletPrefab.objectReferenceValue, typeof(GameObject), true);
            EditorGUILayout.FloatField("Bullet Speed", bulletSpeed.floatValue);
            EditorGUILayout.FloatField("Cooldown", cooldown.floatValue);
        }
    }
}*/
