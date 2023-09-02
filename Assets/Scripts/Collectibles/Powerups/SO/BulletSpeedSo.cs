using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Powerups/Bullet Speed")]
public class BulletSpeedSo : PowerupEffect
{
    [SerializeField] private float amount;
    [SerializeField] private string floatingText;
    [SerializeField] private bool isPositiv;

    public override void Apply()
    {
        GameController.PlayerBulletSpeed += amount;
    }

    public override string FloatingText()
    {
        return floatingText;
    }

    public override bool IsPositiv()
    {
        return isPositiv;
    }
}
