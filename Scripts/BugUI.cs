using TMPro;
using UnityEngine;

public class BugUI : MonoBehaviour
{
    // UI text element showing collected bugs
    public TextMeshProUGUI bugText;

    void Start()
    {
        UpdateBugUI();
    }

    // Updates the bug counter display
    public void UpdateBugUI()
    {
        int collected = GameManager.instance.collectedBugs.Count;
        int total = 20;

        bugText.text = collected + "/" + total;
    }
}