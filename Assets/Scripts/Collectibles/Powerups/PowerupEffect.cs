using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PowerupEffect : ScriptableObject
{
    public abstract void Apply();
    public abstract string FloatingText();
    public abstract bool IsPositiv();
}
