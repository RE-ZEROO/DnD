using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Powerups/Heal Max Health")]
public class HealMaxHealthSO : PowerupEffect
{
    [SerializeField] private int amount;
    [SerializeField] private string floatingText;
    [SerializeField] private bool isPositiv;

    public override void Apply()
    {
        if (GameController.MaxHealth >= 30) { return; }

        GameController.MaxHealth += amount;
        GameController.OnPlayerHeal?.Invoke();
    }

    public override string FloatingText()
    {
        if (GameController.MaxHealth >= 30) { return " "; }

        return floatingText;
    }

    public override bool IsPositiv()
    {
        return isPositiv;
    }
}
