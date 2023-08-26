using System;
using UnityEngine;

public class Key : MonoBehaviour, ICollectible
{
    public static event Action OnKeyCollected;
    public static event Action OnKeyDestroyed;

    public void Collect()
    {
        AudioManager.Instance.PlaySFX("Key");
        GameController.KeyCount++;

        Destroy(gameObject);
        OnKeyCollected?.Invoke();
    }
}
