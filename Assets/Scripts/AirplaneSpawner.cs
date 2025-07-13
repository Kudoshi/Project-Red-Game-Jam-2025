using UnityEngine;

public class AirplaneSpawner : MonoBehaviour
{
    public GameObject airplanePrefab;
    public GameObject player;  // Assign your player in Inspector
    public float spawnInterval = 5f;
    public float verticalOffsetMin = -3f;
    public float verticalOffsetMax = 3f;
    public float startSpawnHeight; // Height at which airplane starts spawning

    private float timer = 0f;

    void Update()
    {
        if (player == null) return;

        // Only spawn if player reaches the required height
        if (player.transform.position.y >= startSpawnHeight)
        {
            timer += Time.deltaTime;
            if (timer >= spawnInterval)
            {
                SpawnAirplane();
                timer = 0f;
            }
        }
    }

    void SpawnAirplane()
    {
        Camera cam = Camera.main;
        if (cam == null) return;

        Vector3 spawnPosition = cam.ViewportToWorldPoint(new Vector3(-0.9f, 0.5f, cam.nearClipPlane));
        spawnPosition.z = 0f;
        spawnPosition.y += Random.Range(verticalOffsetMin, verticalOffsetMax);

        Instantiate(airplanePrefab, spawnPosition, Quaternion.identity);
    }
}
