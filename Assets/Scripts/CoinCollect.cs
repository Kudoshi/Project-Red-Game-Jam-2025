using System;
using UnityEngine;
using Unity.UI;
using TMPro;
using UnityEditor.UI;

public class CoinCollect : MonoBehaviour
{
    public TextMeshProUGUI CoinAmount;

    private int coin = 0;


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            coin++;
            CoinAmount.text = "X " + coin.ToString();
            Destroy(gameObject);
        }
    }
}
