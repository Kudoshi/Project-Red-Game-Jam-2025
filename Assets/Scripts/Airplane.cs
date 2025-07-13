using UnityEngine;

public class Airplane : MonoBehaviour
{
    public float speed = 5f;

    void Update()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);
        
        if (Camera.main != null && transform.position.x > Camera.main.transform.position.x + 10f)
        {
            Destroy(gameObject);
        }
    }
}
 