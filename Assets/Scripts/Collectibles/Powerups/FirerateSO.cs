using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Powerups/Firerate")]
public class FirerateSO : PowerupEffect
{
    [SerializeField] private float amount;

    public override void Apply()
    {
        GameController.FireRate += amount;
    }
}
