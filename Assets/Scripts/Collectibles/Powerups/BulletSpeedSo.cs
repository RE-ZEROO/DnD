using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Powerups/Bullet Speed")]
public class BulletSpeedSo : PowerupEffect
{
    [SerializeField] private float amount;

    public override void Apply()
    {
        GameController.PlayerBulletSpeed += amount;
    }
}
