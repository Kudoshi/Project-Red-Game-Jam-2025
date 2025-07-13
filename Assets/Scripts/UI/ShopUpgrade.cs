
using System;
using UnityEngine;
using UnityEngine.UI;

public class ShopUpgrade : MonoBehaviour
{
    public string ShopID;

    public int CoinCost;

    private Button _btn;

    private void Awake()
    {
        _btn = GetComponent<Button>();
        _btn.onClick.AddListener(PurchaseItem);
    }

    private void PurchaseItem()
    {
        ShopSystem.Instance.PurchaseItem(ShopID, CoinCost);
    }
}