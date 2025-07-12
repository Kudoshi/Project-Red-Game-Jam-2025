using System.Collections.Generic;
using UnityEngine;

public class BackgroundTiling : MonoBehaviour
{
    public GameObject tilePrefab;
    public GameObject mainTilePrefab;
    public Transform targetToFollow; // Usually camera or player
    public float tileSpacing = 100f;
    public Vector3 offset;
    public int preloadTiles = 3;

    private HashSet<int> spawnedTileIndices = new HashSet<int>();

    void Start()
    {
        if (targetToFollow == null)
            targetToFollow = Camera.main.transform;

        SpawnInitialTiles();
    }

    void Update()
    {
        float xPos = targetToFollow.position.x;
        int centerIndex = Mathf.RoundToInt(xPos / tileSpacing);

        // Check around the center tile
        for (int i = -preloadTiles; i <= preloadTiles; i++)
        {
            int tileIndex = centerIndex + i;

            if (!spawnedTileIndices.Contains(tileIndex))
            {
                Vector3 spawnPos = new Vector3(tileIndex * tileSpacing, 0f, 0f) + offset;
                Instantiate(tilePrefab, spawnPos, Quaternion.identity).gameObject.SetActive(true);
                spawnedTileIndices.Add(tileIndex);
            }
        }
    }

    void SpawnInitialTiles()
    {
        float xPos = targetToFollow.position.x;
        int centerIndex = Mathf.RoundToInt(xPos / tileSpacing);

        for (int i = -preloadTiles; i <= preloadTiles; i++)
        {   
            int tileIndex = centerIndex + i;
            Vector3 spawnPos = new Vector3(tileIndex * tileSpacing, 0f, 0f) + offset;

            if (i == 0)
            {
                Instantiate(mainTilePrefab, spawnPos, Quaternion.identity).gameObject.SetActive(true);
            }
            else
                Instantiate(tilePrefab, spawnPos, Quaternion.identity).gameObject.SetActive(true);
            spawnedTileIndices.Add(tileIndex);
        }
    }
}
