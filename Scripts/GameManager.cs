using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public BugUI bugUI;

    public List<BugCollector> activeBugs = new List<BugCollector>();
    public List<BugCollector> collectedBugs = new List<BugCollector>();

    public int health = 0;

    void Awake()
    {
        Time.timeScale = 1f;

        // Singleton setup
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Register all bugs in the scene at start
        BugCollector[] bugs = FindObjectsOfType<BugCollector>();
        activeBugs.AddRange(bugs);
    }

    public void CollectBug(BugCollector bug)
    {
        // Mark bug as collected and disable it
        bug.isCollected = true;
        bug.gameObject.SetActive(false);

        activeBugs.Remove(bug);
        collectedBugs.Add(bug);

        health++;

        bugUI.UpdateBugUI();
    }

    public void DecreaseHealth(int amount)
    {
        int oldHealth = health;
        health -= amount;

        if (health < 0)
        {
            RespawnCollectedBugs(oldHealth);
            health = 0;
        }
        else
        {
            RespawnCollectedBugs(amount);
        }
    }

    void RespawnCollectedBugs(int amount)
    {
        if (collectedBugs.Count == 0)
            return;

        for (int i = 0; i < amount; i++)
        {
            if (collectedBugs.Count == 0)
                break;

            int randomIndex = Random.Range(0, collectedBugs.Count);
            BugCollector bug = collectedBugs[randomIndex];

            // Restore bug state and reactivate
            bug.isCollected = false;
            bug.gameObject.SetActive(true);

            collectedBugs.RemoveAt(randomIndex);
            activeBugs.Add(bug);
        }

        bugUI.UpdateBugUI();
    }
}