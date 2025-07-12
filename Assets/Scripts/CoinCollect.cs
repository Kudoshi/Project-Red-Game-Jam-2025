using System;
using UnityEngine;
using Unity.UI;
using TMPro;
using UnityEditor.UI;

public class CoinCollect : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            UICoin.Instance.AddCoin();
            Destroy(gameObject);
        }
    }
}
