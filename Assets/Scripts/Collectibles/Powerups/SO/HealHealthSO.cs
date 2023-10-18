using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Powerups/Heal Health")]
public class HealHealthSO : PowerupEffect
{
    [SerializeField] private float amount;
    [SerializeField] private string floatingText;
    [SerializeField] private bool isPositiv;

    public override void Apply()
    {
        if(GameController.Health >= GameController.MaxHealth) { return; }

        GameController.Health += amount;

        if (GameController.Health > GameController.MaxHealth)
            GameController.Health = GameController.MaxHealth;

        GameController.OnPlayerHeal?.Invoke();
    }

    public override string FloatingText()
    {
        if (GameController.Health >= GameController.MaxHealth) { return " "; }

        return floatingText;
    }

    public override bool IsPositiv()
    {
        return isPositiv;
    }
}
