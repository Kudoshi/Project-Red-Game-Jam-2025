using System;
using UnityEngine;

public class CoinCollect : MonoBehaviour
{
    public float rotateSpeed = 5f;
    public AudioSource ticketSound;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            UICoin.Instance.AddCoin();
            GetComponent<AudioSource>().Play();
            Destroy(gameObject);
        }
    }

    void Update()
    {
        transform.Rotate(new Vector3(0, rotateSpeed, 0));
    }
}
