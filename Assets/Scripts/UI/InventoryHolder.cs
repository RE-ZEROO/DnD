using TMPro;
using UnityEngine;

public class InventoryHolder : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI bombCountText;
    [SerializeField] private TextMeshProUGUI coinCountText;
    [SerializeField] private TextMeshProUGUI keyCountText;

    private void OnEnable()
    {
        BombItem.OnBombCollected += UpdateBombValue;
        Coin.OnCoinCollected += UpdateCoinValue;
        Key.OnKeyCollected += UpdateKeyValue;
    }

    private void OnDisable()
    {
        BombItem.OnBombCollected -= UpdateBombValue;
        Coin.OnCoinCollected -= UpdateCoinValue;
        Key.OnKeyCollected -= UpdateKeyValue;
    }

    private void UpdateBombValue() => bombCountText.text = GameController.BombCount.ToString();

    private void UpdateCoinValue() => coinCountText.text = GameController.CoinCount.ToString();

    private void UpdateKeyValue() => keyCountText.text = GameController.KeyCount.ToString();
}
