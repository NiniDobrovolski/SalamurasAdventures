using Unity.Mathematics;
using Unity.VisualScripting;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovementScene2 : MonoBehaviour
{
    private float _laneDistance = 3;
    private float _laneChangeSpeed = 7;
    private float _startX;
    private bool _isGrounded = false;

    Animator animator;
    private GameFlow gameFlow;

    [SerializeField] private Rigidbody rb;
    [SerializeField] private CapsuleCollider capsule;

    [SerializeField] private float standingHeight = 2f;
    [SerializeField] private float crouchHeight = 1f;

    private Vector3 standingCenter;
    private Vector3 crouchCenter;

    private int _currentLane;

    void Start()
    {
        // Setup capsule sizes for standing and crouching
        standingCenter = new Vector3(0, standingHeight / 2f, 0);
        crouchCenter = new Vector3(0, crouchHeight / 2f, 0);

        capsule.height = standingHeight;
        capsule.center = standingCenter;

        _startX = transform.position.x;

        animator = GetComponent<Animator>();
    }

    void Update()
    {
        HandleInput();
        MovePlayer();

        animator.SetBool("IsGrounded", _isGrounded);

        // Walking animation state
        bool isWalking = _isGrounded && !animator.GetBool("IsCrouching");
        animator.SetBool("IsWalking", isWalking);

        // Jump input
        if ((Input.GetKeyDown(KeyCode.UpArrow) ||
             Input.GetKeyDown(KeyCode.Space) ||
             Input.GetKeyDown(KeyCode.W)) && _isGrounded)
        {
            animator.SetTrigger("Jump");
            _isGrounded = false;

            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 7, rb.linearVelocity.z);
        }

        // Crouch input
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            StartCoroutine(CrouchForOneSecond());
        }
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            ChangeLane(-1);

        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            ChangeLane(1);
    }

    private void ChangeLane(int direction)
    {
        _currentLane += direction;
        _currentLane = Mathf.Clamp(_currentLane, -1, 1);
    }

    private void MovePlayer()
    {
        Vector3 targetPosition = transform.position;
        targetPosition.x = _startX + _currentLane * _laneDistance;

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            _laneChangeSpeed * Time.deltaTime
        );
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            _isGrounded = true;

        if (collision.gameObject.CompareTag("Obsticle"))
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MiddleTrigger") && GameFlow.Instance.reachedEnd == false)
        {
            GameFlow.Instance.SpawnTile();
        }
    }

    IEnumerator CrouchForOneSecond()
    {
        animator.SetBool("IsCrouching", true);

        capsule.height = crouchHeight;
        capsule.center = crouchCenter;

        yield return new WaitForSeconds(0.55f);

        animator.SetBool("IsCrouching", false);

        capsule.height = standingHeight;
        capsule.center = standingCenter;
    }
}