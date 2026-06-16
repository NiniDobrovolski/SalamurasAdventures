using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public GameObject movingTilePrefab;
    public Transform startTile;
    public Transform tilesParent;

    private List<GameObject> activeTiles = new List<GameObject>();

    private float yOffset = 15f;
    private float currentY;

    private int tileCounter = 0;
    public TileManager tileManager;

    private int nextTileID = 0;

    void Start()
    {
        // Initial spawn position based on start tile
        currentY = startTile.position.y - yOffset;

        SpawnTenTiles();
    }

    public void SpawnTenTiles()
    {
        // Spawn a batch of tiles
        for (int i = 0; i < 10; i++)
        {
            Vector3 spawnPos = new Vector3(
                0,
                currentY,
                startTile.position.z
            );

            GameObject newTile = Instantiate(
                movingTilePrefab,
                spawnPos,
                Quaternion.identity,
                tilesParent
            );

            // Assign unique tile ID for order validation
            MovingTile tile = newTile.GetComponent<MovingTile>();
            tile.tileID = nextTileID;
            nextTileID++;

            // Spawn obstacles on tile
            tile.GetComponent<ObstacleSpawner>().SpawnObstacle();

            activeTiles.Add(newTile);

            currentY -= yOffset;
        }

        tileCounter += 10;
    }

    public void CheckAndSpawn(GameObject currentTile)
    {
        int index = activeTiles.IndexOf(currentTile);

        // Spawn more tiles when approaching end of current batch
        if (index == activeTiles.Count - 3 && tileCounter + 10 <= 60)
        {
            SpawnTenTiles();

            // Remove old tiles behind player
            for (int i = 0; i < 5; i++)
            {
                if (activeTiles[0] != currentTile)
                {
                    Destroy(activeTiles[0]);
                    activeTiles.RemoveAt(0);
                }
            }
        }
    }

    public int GetTileIndex(GameObject tile)
    {
        return activeTiles.IndexOf(tile);
    }

    public GameObject GetNextTile(GameObject tile)
    {
        int index = activeTiles.IndexOf(tile);

        if (index > 0)
            return activeTiles[index - 1];

        return null;
    }
}