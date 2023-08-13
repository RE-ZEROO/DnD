using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Powerups/Tripple Shot")]
public class TrippleShotSO : PowerupEffect
{
    [SerializeField] private bool isTrippleShot;

    public override void Apply()
    {
        PlayerController.isTripleshot = isTrippleShot;
    }
}
