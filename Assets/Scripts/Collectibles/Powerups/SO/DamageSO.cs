using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Powerups/Damage")]
public class DamageSO : PowerupEffect
{
    [SerializeField] private int amount;
    [SerializeField] private string floatingText;
    [SerializeField] private bool isPositiv;

    public override void Apply()
    {
        GameController.PlayerDamage += amount;
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
