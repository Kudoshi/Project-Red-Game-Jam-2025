using System;
using UnityEngine;

public class BreakCloud : MonoBehaviour
{
    public float breakDelay = 2f;
    private bool isBreaking = false;
    [SerializeField] private float timer = 0f;

    private void Update()
    {
        if (isBreaking)
        {
            timer += Time.deltaTime;
            if (timer >= breakDelay)
            {
                Break();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Hit Player");
            isBreaking = true;
            timer = 0f;
        }
    }
    

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isBreaking = false;
            timer = 0f;
        }
    }

    private void Break()
    {
        Debug.Log("Obstacle broke!");
        Destroy(gameObject);
    }
}
