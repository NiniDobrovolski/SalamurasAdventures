using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject[] obstacles;
    public float rangeX = 5f;

    public void SpawnObstacle()
    {
        // Clear previous obstacle
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        float randomX = Random.Range(-rangeX, rangeX);

        Vector3 spawnPos = new Vector3(
            transform.position.x + randomX,
            transform.position.y,
            transform.position.z
        );

        GameObject obs = Instantiate(
            obstacles[Random.Range(0, obstacles.Length)],
            spawnPos,
            Quaternion.identity
        );

        // Rat setup (rotation + position offset)
        if (obs.CompareTag("Rat"))
        {
            obs.transform.rotation = Quaternion.Euler(270f, 0f, -90f);
            obs.transform.position += new Vector3(0f, 0.25f, 0f);
        }

        // Juice positioning
        if (obs.CompareTag("Juice"))
        {
            obs.transform.position += new Vector3(0f, 1.85f, 0f);
        }

        // Spike positioning
        if (obs.CompareTag("Spike"))
        {
            obs.transform.position += new Vector3(0f, 2f, 0f);
        }

        obs.transform.SetParent(transform, true);

        Vector3 parentScale = transform.localScale;

        // Trap scaling adjustment
        if (obs.CompareTag("Trap"))
        {
            obs.transform.localScale = new Vector3(
                0.5f / parentScale.x,
                0.5f / parentScale.y,
                0.5f / parentScale.z
            );
        }

        // Juice scaling adjustment
        if (obs.CompareTag("Juice"))
        {
            obs.transform.localScale = new Vector3(
                4f / parentScale.x,
                4f / parentScale.y,
                4f / parentScale.z
            );
        }
    }
}