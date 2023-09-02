using System;
using UnityEngine;

public class BombItem : MonoBehaviour, ICollectible
{
    public static Action OnBombCollected;

    public void Collect()
    {
        AudioManager.Instance.PlaySFX("BombItem");
        GameController.BombCount++;

        Destroy(gameObject);
        OnBombCollected?.Invoke();
    }
}
