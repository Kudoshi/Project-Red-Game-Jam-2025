using System;
using UnityEngine;

public class CoinCollect : MonoBehaviour
{
    public float rotateSpeed = 5f;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            UICoin.Instance.AddCoin();
            SoundManager.Instance.PlaySound("sfx_ticket_collect");
            Destroy(gameObject);
        }
    }

    void Update()
    {
        transform.Rotate(new Vector3(0, rotateSpeed, 0));
    }
}
