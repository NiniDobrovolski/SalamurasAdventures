using UnityEngine;
using DG.Tweening;
using TMPro;

[RequireComponent(typeof(CharacterController))]
public class PlayerLadderClimb : MonoBehaviour
{
    public float climbDuration = 1.5f;

    private CharacterController controller;
    private LadderSc currentLadder;
    private bool isClimbing = false;

    public TMP_Text climbPrompt;

    private string currentTriggerTag = "";

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Show prompt when near ladder
        if (currentLadder != null && !isClimbing && climbPrompt != null)
        {
            climbPrompt.gameObject.SetActive(true);
        }

        if ((currentLadder == null || isClimbing) && climbPrompt != null)
        {
            climbPrompt.gameObject.SetActive(false);
        }

        // Handle climb input
        if (!isClimbing && Input.GetKeyDown(KeyCode.E) && currentLadder != null)
        {
            if (currentTriggerTag == "LadderBottom")
                ClimbUp();
            else if (currentTriggerTag == "LadderTop")
                ClimbDown();
        }
    }

    private void ClimbUp()
    {
        isClimbing = true;
        controller.enabled = false;

        PlayerMovement pm = GetComponent<PlayerMovement>();
        if (pm != null) pm.isClimbing = true;

        transform.DOMove(currentLadder.topPoint.position, climbDuration * 0.12f)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                controller.enabled = true;
                isClimbing = false;

                if (pm != null) pm.isClimbing = false;
            });
    }

    private void ClimbDown()
    {
        if (currentLadder == null) return;

        isClimbing = true;

        PlayerMovement pm = GetComponent<PlayerMovement>();
        if (pm != null) pm.isClimbing = true;

        controller.enabled = false;

        Vector3 startPos = transform.position;
        Vector3 midPoint = startPos + currentLadder.topPoint.transform.right * 6f;
        Vector3 endPoint = currentLadder.bottomPoint.position;

        transform.DOMove(midPoint, climbDuration * 0.13f)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                transform.DOMove(endPoint, climbDuration * 0.2f)
                    .SetEase(Ease.Linear)
                    .OnComplete(() =>
                    {
                        controller.enabled = true;
                        isClimbing = false;

                        if (pm != null) pm.isClimbing = false;
                    });
            });
    }

    private void OnTriggerEnter(Collider other)
    {
        LadderSc ladder = other.GetComponentInParent<LadderSc>();
        if (ladder != null)
        {
            currentLadder = ladder;

            if (other.CompareTag("LadderTop"))
                currentTriggerTag = "LadderTop";
            else if (other.CompareTag("LadderBottom"))
                currentTriggerTag = "LadderBottom";
        }
    }

    private void OnTriggerExit(Collider other)
    {
        LadderSc ladder = other.GetComponentInParent<LadderSc>();
        if (ladder != null)
        {
            if ((currentTriggerTag == "LadderTop" && other.CompareTag("LadderTop")) ||
                (currentTriggerTag == "LadderBottom" && other.CompareTag("LadderBottom")))
            {
                currentLadder = null;
                currentTriggerTag = "";
                climbPrompt.gameObject.SetActive(false);
            }
        }
    }
}