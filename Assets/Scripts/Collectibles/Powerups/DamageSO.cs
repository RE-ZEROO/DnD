using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Powerups/Damage")]
public class DamageSO : PowerupEffect
{
    [SerializeField] private float amount;

    public override void Apply()
    {
        GameController.PlayerDamage += amount;
    }
}
