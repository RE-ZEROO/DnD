using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField] private PowerupEffect powerupEffect1;
    [SerializeField] private PowerupEffect powerupEffect2;
    [SerializeField] private PowerupEffect powerupEffect3;
    [SerializeField] private PowerupEffect powerupEffect4;
    [SerializeField] private PowerupEffect powerupEffect5;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!collision.CompareTag("Player")) { return; }

        Destroy(gameObject);
        AudioManager.Instance.PlaySFX("Powerup");

        powerupEffect1?.Apply();
        powerupEffect2?.Apply();
        powerupEffect3?.Apply();
        powerupEffect4?.Apply();
        powerupEffect5?.Apply();
    }
}
