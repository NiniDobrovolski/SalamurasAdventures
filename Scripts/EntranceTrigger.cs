using TMPro;
using UnityEngine;
 
public class EntranceTrigger : MonoBehaviour
{
    public GameObject entranceText;
 
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(GameManager.instance.collectedBugs.Count < 20)
            {
                entranceText.SetActive(true);
            }
            else
            {
                entranceText.GetComponent<TMP_Text>().text = "შედი გამოქვაბულში";
                entranceText.SetActive(true);
            }
        }
    }
 
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            entranceText.SetActive(false);
        }
    }
}