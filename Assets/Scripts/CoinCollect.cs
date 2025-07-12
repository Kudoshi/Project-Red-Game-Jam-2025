using System;
using UnityEngine;
using Unity.UI;
using TMPro;
using UnityEditor.UI;

public class CoinCollect : MonoBehaviour
{
    public float rotateSpeed = 5f;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            UICoin.Instance.AddCoin();
            Destroy(gameObject);
        }
    }

    void Update()
    {

        transform.Rotate(new Vector3(0, rotateSpeed, 0));
    }
}
