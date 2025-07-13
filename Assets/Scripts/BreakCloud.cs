using DG.Tweening;
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
        if (collision.CompareTag("Player") && !isBreaking)
        {
            Debug.Log("Hit Player");
            isBreaking = true;
            timer = 0f;
            SoundManager.Instance.PlaySound("sfx_cloud_shake");

            DOVirtual.Float(0.02f, 0.1f, breakDelay, (strength) =>
            {
                transform.DOShakePosition(0.2f, strength, 5, 90f, false, true)
                         .SetEase(Ease.OutQuad);
            });
        }
    }
    

    private void OnTriggerExit2D(Collider2D collision)
    {
        //if (collision.CompareTag("Player"))
        //{
        //    isBreaking = false;
        //    timer = 0f;
        //}
    }

    private void Break()
    {
        if (breakParticle != null)
        {
            SoundManager.Instance.PlaySound("sfx_cloud_break");
            GameObject p = Instantiate(breakParticle, transform.position, Quaternion.identity);
            p.GetComponent<ParticleSystem>().Play();
            Destroy(p, 2f);
        }

        Destroy(gameObject);
    }
}
