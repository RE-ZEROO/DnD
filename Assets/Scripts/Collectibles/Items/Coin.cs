using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour, ICollectible
{
    public static event Action OnCoinCollected;

    public void Collect()
    {
        AudioManager.Instance.PlaySFX("Coin");
        GameController.CoinCount++;

        Destroy(gameObject);
        OnCoinCollected?.Invoke();
    }
}
