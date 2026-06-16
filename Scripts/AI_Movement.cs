using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_Movement : MonoBehaviour
{
    Animator animator;

    // Movement speed of the AI
    public float moveSpeed = 0.2f;

    Vector3 stopPosition;

    float walkTime;
    public float walkCounter;
    float waitTime;
    public float waitCounter;

    // Direction index (0-3)
    int WalkDirection;

    // Indicates whether the AI is currently moving
    public bool isWalking;

    void Start()
    {
        // Randomize walk and wait durations
        walkTime = Random.Range(3, 6);
        waitTime = Random.Range(5, 7);

        waitCounter = waitTime;
        walkCounter = walkTime;

        ChooseDirection();
    }

    void Update()
    {
        if (isWalking)
        {
            walkCounter -= Time.deltaTime;

            switch (WalkDirection)
            {
                case 0:
                    transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
                    transform.position += transform.right * moveSpeed * Time.deltaTime;
                    break;

                case 1:
                    transform.localRotation = Quaternion.Euler(0f, 180, 0f);
                    transform.position += transform.right * moveSpeed * Time.deltaTime;
                    break;

                case 2:
                    transform.localRotation = Quaternion.Euler(0f, -90, 0f);
                    transform.position += transform.right * moveSpeed * Time.deltaTime;
                    break;

                case 3:
                    transform.localRotation = Quaternion.Euler(0f, 0, 0f);
                    transform.position += transform.right * moveSpeed * Time.deltaTime;
                    break;
            }

            // Stop walking when the timer expires
            if (walkCounter <= 0)
            {
                stopPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
                isWalking = false;
                transform.position = stopPosition;
                waitCounter = waitTime;
            }
        }
        else
        {
            // Wait before choosing a new direction
            waitCounter -= Time.deltaTime;

            if (waitCounter <= 0)
            {
                ChooseDirection();
            }
        }
    }

    // Selects a random movement direction
    public void ChooseDirection()
    {
        WalkDirection = Random.Range(0, 4);

        isWalking = true;
        walkCounter = walkTime;
    }
}