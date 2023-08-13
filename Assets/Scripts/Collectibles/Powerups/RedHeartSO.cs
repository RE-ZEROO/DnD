using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Powerups/Red Heart")]
public class RedHeartSO : PowerupEffect
{
    [SerializeField] private float amount;

    public override void Apply()
    {
        GameController.Health += amount;
    }
}
