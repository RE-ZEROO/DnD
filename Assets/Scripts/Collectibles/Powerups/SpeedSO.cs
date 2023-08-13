using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Powerups/Speed")]
public class SpeedSO : PowerupEffect
{
    [SerializeField] private float amount;

    public override void Apply()
    {
        GameController.MoveSpeed += amount;
    }
}
