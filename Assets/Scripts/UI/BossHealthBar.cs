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

    

    /*void Start()
    {
        slider = GetComponent<Slider>();
        enemyBoss = FindObjectOfType<BossTemplate>();
        //Invoke(nameof(GetBoss), 0.5f);
    }

    private void GetBoss()
    {
        enemyBoss = FindObjectOfType<BossTemplate>();
    }

    void Update()
    {
        if(slider.value <= slider.minValue)
            fillImage.enabled = false;

        if (slider.value > slider.minValue && !fillImage.enabled)
            fillImage.enabled = true;

        float fillValue = enemyBoss.health / enemyBoss.maxHealth;

        slider.value = fillValue;
    }*/
}
