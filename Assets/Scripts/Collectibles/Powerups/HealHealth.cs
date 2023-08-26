using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Powerups/Heal Health")]
public class HealHealth : PowerupEffect
{
    [SerializeField] private float amount;
    //public GameObject floatingTextPrefab;

    public override void Apply()
    {
        if(GameController.Health >= GameController.MaxHealth) { return; }

        GameController.Health += amount;
        GameController.OnPlayerHeal?.Invoke();
    }
}
