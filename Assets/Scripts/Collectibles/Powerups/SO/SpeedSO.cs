using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Powerups/Speed")]
public class SpeedSO : PowerupEffect
{
    [SerializeField] private float amount;
    [SerializeField] private string floatingText;
    [SerializeField] private bool isPositiv;

    public override void Apply()
    {
        GameController.MoveSpeed += amount;
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
