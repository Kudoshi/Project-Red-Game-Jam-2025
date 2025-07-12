using UnityEngine;

public class WingFan : MonoBehaviour
{
    public float blowForce = 30f;   
    public Vector2 blowDirection = Vector2.up;  

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.AddForce(blowDirection.normalized * blowForce, ForceMode2D.Force);
            }
        }
    }
}
