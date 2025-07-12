using System;
using UnityEngine;
using Unity.UI;
using TMPro;
using UnityEditor.UI;

public class CoinCollect : MonoBehaviour
{
    public TextMeshProUGUI CoinAmount;

    private int coin = 0;


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            coin++;
            CoinAmount.text = "X" + coin.ToString();
            Destroy(gameObject);
        }
    }
}
