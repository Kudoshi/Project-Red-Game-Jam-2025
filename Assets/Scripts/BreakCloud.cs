using System;
using UnityEngine;

public class BreakCloud : MonoBehaviour
{
    public float breakDelay = 2f;
    public GameObject breakParticle;
    private bool isBreaking = false;
    private ParticleSystem breakEff;
    [SerializeField] private float timer = 0f;

    private void Start()
    {
        breakEff = breakParticle.GetComponent<ParticleSystem>();
    }

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
        if (breakParticle != null)
        {
            GameObject p = Instantiate(breakParticle, transform.position, Quaternion.identity);
            Destroy(p, 2f);
        }

        Destroy(gameObject);
    }
}
