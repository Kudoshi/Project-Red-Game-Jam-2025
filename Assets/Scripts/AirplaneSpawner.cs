using UnityEngine;

public class AirplaneSpawner : MonoBehaviour
{
    public GameObject airplanePrefab;
    public float spawnInterval = 5f;
    public float verticalOffsetMin = -3f;
    public float verticalOffsetMax = 3f;

    private float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnAirplane();
            timer = 0f;
        }
    }

    void SpawnAirplane()
    {
        Camera cam = Camera.main;
        if (cam == null) return;

        Vector3 spawnPosition = cam.ViewportToWorldPoint(new Vector3(1.1f, 0.5f, cam.nearClipPlane));
        spawnPosition.z = 0f;
        spawnPosition.y += Random.Range(verticalOffsetMin, verticalOffsetMax);

        Instantiate(airplanePrefab, spawnPosition, Quaternion.identity);
    }
}
