using UnityEngine;

public class PogoCollect : MonoBehaviour
{
    public float rotateSpeed = 5f;
    public int GetPogoAmt;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PogoJump.Instance.JumpAmount += GetPogoAmt;

            Destroy(gameObject);
        }
    }

    void Update()
    {
        transform.Rotate(new Vector3(0, rotateSpeed, 0));
    }
}
