using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    //private Vector2 Spawn;
    private float xOffset = 20f;
    private float animationTime = 0.35f / 2f;

    [SerializeField] private GameObject floatingTextPrefab;
    private Color positivColor = Color.green;
    private Color negativColor = Color.red;

    [Header("Effects")]
    [SerializeField] private PowerupEffect powerupEffect1;
    [SerializeField] private PowerupEffect powerupEffect2;
    [SerializeField] private PowerupEffect powerupEffect3;
    [SerializeField] private PowerupEffect powerupEffect4;
    [SerializeField] private PowerupEffect powerupEffect5;

    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!collision.CompareTag("Player")) { return; }

        AudioManager.Instance.PlaySFX("Powerup");
        GetComponent<SpriteRenderer>().enabled = false;
        
        powerupEffect1?.Apply();
        powerupEffect2?.Apply();
        powerupEffect3?.Apply();
        powerupEffect4?.Apply();
        powerupEffect5?.Apply();

        StartCoroutine(SpawnFloatingText());
    }

    private IEnumerator SpawnFloatingText()
    {
        if(powerupEffect1 != null)
        {
            GameObject text1 = Instantiate(floatingTextPrefab, SpawnPos(), Quaternion.identity);
            text1.GetComponentInChildren<TextMeshPro>().SetText(powerupEffect1.FloatingText());
            text1.GetComponentInChildren<TextMeshPro>().color = powerupEffect1.IsPositiv() ? positivColor : negativColor;
        }

        yield return new WaitForSeconds(animationTime);

        if (powerupEffect2 != null)
        {
            GameObject text2 = Instantiate(floatingTextPrefab, SpawnPos(), Quaternion.identity);
            text2.GetComponentInChildren<TextMeshPro>().SetText(powerupEffect2.FloatingText());
            text2.GetComponentInChildren<TextMeshPro>().color = powerupEffect2.IsPositiv() ? positivColor : negativColor;
        }

        yield return new WaitForSeconds(animationTime);

        if (powerupEffect3 != null)
        {
            GameObject text3 = Instantiate(floatingTextPrefab, SpawnPos(), Quaternion.identity);
            text3.GetComponentInChildren<TextMeshPro>().SetText(powerupEffect3.FloatingText());
            text3.GetComponentInChildren<TextMeshPro>().color = powerupEffect3.IsPositiv() ? positivColor : negativColor;
        }

        yield return new WaitForSeconds(animationTime);

        if (powerupEffect4 != null)
        {
            GameObject text4 = Instantiate(floatingTextPrefab, SpawnPos(), Quaternion.identity);
            text4.GetComponentInChildren<TextMeshPro>().SetText(powerupEffect4.FloatingText());
            text4.GetComponentInChildren<TextMeshPro>().color = powerupEffect4.IsPositiv() ? positivColor : negativColor;
        }

        yield return new WaitForSeconds(animationTime);

        if (powerupEffect5 != null)
        {
            GameObject text5 = Instantiate(floatingTextPrefab, SpawnPos(), Quaternion.identity);
            text5.GetComponentInChildren<TextMeshPro>().SetText(powerupEffect5.FloatingText());
            text5.GetComponentInChildren<TextMeshPro>().color = powerupEffect5.IsPositiv() ? positivColor : negativColor;
        }

        Destroy(gameObject);
    }

    private Vector2 SpawnPos()
    {
        var Spawn = transform.position;
        Spawn.x += xOffset;

        return Spawn;
    }
}
