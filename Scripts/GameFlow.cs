using System.Collections.Generic;
using UnityEngine;

public class GameFlow : MonoBehaviour
{
    public static GameFlow Instance;

    // References
    public Transform player;
    public Transform tileObj;
    public Transform wholeRoad;
    public Transform plantObj;
    public Transform treeObj;
    public Transform woodObj;
    public Note noteObj2;
    public Note noteObj3;
    public Note noteObj4;
    public Transform finalTilePrefab;
    public Transform SafetyTile;

    private Transform lastTile;
    private List<Transform> spawnedTiles = new List<Transform>();
    public bool reachedEnd = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        lastTile = wholeRoad.GetChild(wholeRoad.childCount - 1);
    }

    void Update()
    {
        if (reachedEnd) return;

        // Remove tiles that are far behind the player
        while (spawnedTiles.Count > 0)
        {
            Transform firstTile = spawnedTiles[0];

            if (firstTile == null)
            {
                spawnedTiles.RemoveAt(0);
                continue;
            }

            Renderer rend = firstTile.GetComponentInChildren<Renderer>();
            float tileEndZ = rend.bounds.max.z;

            if (tileEndZ < player.position.z - 1f && firstTile != lastTile)
            {
                Destroy(firstTile.gameObject);
                spawnedTiles.RemoveAt(0);
            }
            else
            {
                break;
            }
        }

        spawnedTiles.RemoveAll(tile => tile == null);
    }

    static float tileLength = 200f;
    float endZ = tileLength;
    float spacing = 10f;

    public void SpawnTile()
    {
        Renderer lastRenderer = lastTile.GetComponentInChildren<Renderer>();
        float lastTileLength = lastRenderer.bounds.size.z;

        Vector3 spawnPosition = lastTile.position + Vector3.forward * lastTileLength;
        Transform newTile = Instantiate(tileObj, spawnPosition, Quaternion.identity, wholeRoad);
        lastTile = newTile;

        // Spawn obstacles and notes across the tile
        for (float z = 0; z < endZ; z += spacing)
        {
            int[] allLanes = { 3, 6, 9 };
            List<int> freeLanes = new List<int>(allLanes);

            Dictionary<int, Transform> obstacleInLane = new Dictionary<int, Transform>();

            int obstacleCount = Random.Range(1, 3);

            for (int i = 0; i < obstacleCount; i++)
            {
                if (freeLanes.Count == 0) break;

                int laneIndex = Random.Range(0, freeLanes.Count);
                int lane = freeLanes[laneIndex];
                freeLanes.RemoveAt(laneIndex);

                Vector3 obstaclePos = newTile.GetChild(0).position;
                obstaclePos.x = lane;
                obstaclePos.y = -2f;
                obstaclePos.z = obstaclePos.z + z - 100f;

                Transform[] obstacles = { plantObj, woodObj, treeObj };
                Transform randomObstacle = obstacles[Random.Range(0, obstacles.Length)];

                if (randomObstacle == treeObj)
                    obstaclePos.x = lane - 1;

                Transform spawnedObstacle = Instantiate(randomObstacle, obstaclePos, Quaternion.identity, newTile);

                obstacleInLane[lane] = spawnedObstacle;
            }

            Transform[] notes = { noteObj2.transform, noteObj3.transform, noteObj4.transform };

            foreach (var pair in obstacleInLane)
            {
                int lane = pair.Key;
                Transform obstacle = pair.Value;

                Vector3 notePos = newTile.GetChild(0).position;
                notePos.x = lane - 1;
                notePos.y = 0f;
                notePos.z = notePos.z - 100f + z;

                if (obstacle.name.Contains(plantObj.name))
                {
                    notePos.y -= 1f;
                }
                else if (obstacle.name.Contains(woodObj.name))
                {
                    notePos.y += 1.0f;
                }
                else
                {
                    continue;
                }

                Quaternion noteRotation = Quaternion.Euler(180f, 0f, 180f);
                Transform randomNote = notes[Random.Range(0, notes.Length)];

                Instantiate(randomNote, notePos, noteRotation, newTile);
                NoteManager.Instance.AddNote();
            }
        }

        spawnedTiles.Add(newTile);
    }

    public void ReachEnd()
    {
        if (reachedEnd) return;

        reachedEnd = true;

        Transform finalTile = Instantiate(finalTilePrefab,
            lastTile.position + Vector3.forward * (tileLength + 165f),
            Quaternion.identity,
            wholeRoad);

        SafetyTile.gameObject.SetActive(true);

        // Clean up remaining spawned tiles
        for (int i = 0; i < spawnedTiles.Count; i++)
        {
            if (spawnedTiles[i] != null)
            {
                Destroy(spawnedTiles[i].gameObject);
            }
        }

        lastTile = finalTile;
    }
}