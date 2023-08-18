using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Powerups/Heal Max Health")]
public class HealMaxHealthSO : PowerupEffect
{
    [SerializeField] private int amount;

    public override void Apply()
    {
        if (GameController.Health > 30) { return; }

        GameController.MaxHealth += amount;
        GameController.OnPlayerHeal?.Invoke();
    }
}
