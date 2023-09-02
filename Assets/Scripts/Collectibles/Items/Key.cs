using System;
using UnityEngine;

public class Key : MonoBehaviour, ICollectible
{
    public static Action OnKeyCollected;

    public void Collect()
    {
        AudioManager.Instance.PlaySFX("Key");
        GameController.KeyCount++;

        Destroy(gameObject);
        OnKeyCollected?.Invoke();
    }
}
