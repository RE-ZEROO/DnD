using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    private BossTemplate enemyBoss;
    private Slider slider;

    private bool showHealthBar;

    private void OnEnable()
    {
        BossTemplate.OnHealthBar += ShowHealthbar;
    }

    private void OnDisable()
    {
        BossTemplate.OnHealthBar -= ShowHealthbar;
    }

    void Start()
    {
        slider = GetComponent<Slider>();
        

        foreach (Transform child in transform)
            child.gameObject.SetActive(false);
    }

    void Update()
    {
        if(!showHealthBar) { return; }

        if (slider.value <= slider.minValue)
            fillImage.enabled = false;

        if (slider.value > slider.minValue && !fillImage.enabled)
            fillImage.enabled = true;

        float fillValue = enemyBoss.health / enemyBoss.maxHealth;

        slider.value = fillValue;

        if (enemyBoss.health <= 0)
            Destroy(gameObject);
    }

    private void ShowHealthbar()
    {
        enemyBoss = FindObjectOfType<BossTemplate>();

        foreach (Transform child in transform)
            child.gameObject.SetActive(true);

        showHealthBar = true;
    }
}
