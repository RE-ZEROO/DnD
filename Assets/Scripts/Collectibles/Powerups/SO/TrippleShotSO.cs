using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Powerups/Tripple Shot")]
public class TrippleShotSO : PowerupEffect
{
    [SerializeField] private bool isTrippleShot;
    [SerializeField] private string floatingText;
    [SerializeField] private bool isPositiv;

    public override void Apply()
    {
        PlayerController.isTripleshot = isTrippleShot;
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
