using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Powerups/Bullet Size")]
public class BulletSizeSO : PowerupEffect
{
    [SerializeField] private float amount;

    public override void Apply()
    {
        GameController.BulletSize += amount;
    }
}
