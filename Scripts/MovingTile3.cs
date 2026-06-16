using UnityEngine;

public class MovingTile : MonoBehaviour
{
    public float moveSpeed;
    public float moveDistance;

    private Vector3 startPos;
    private bool moveRight = true;

    public Rigidbody rb;
    private bool isStopped = false;

    public bool grounded = false;
    public static MovingTile instance;

    public int tileID;

    void OnEnable()
    {
        // Store initial position for movement bounds
        startPos = transform.position;
    }

    void Update()
    {
        if (!isStopped)
        {
            MoveTiles(grounded);
        }
    }

    public void StopTile()
    {
        isStopped = true;
    }

    public void MoveTiles(bool grounded)
    {
        if (!grounded)
        {
            if (moveRight)
            {
                transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);

                if (transform.position.x >= startPos.x + moveDistance)
                    moveRight = false;
            }
            else
            {
                transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);

                if (transform.position.x <= startPos.x - moveDistance)
                    moveRight = true;
            }
        }
        else
        {
            StopTile();
        }
    }
}