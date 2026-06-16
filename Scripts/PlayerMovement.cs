using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public Transform cam;

    public float speed = 25f;
    public float gravity = -9.81f;
    public float jumpHeight = 8;
    public Vector3 velocity;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip poisonSound;

    private bool isGrounded = true;
    public bool isClimbing = false;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    float turnSmoothVelocity;
    public float turnSmoothTime = 0.1f;

    public Animator animator;

    private float _groundRayDistance = 10f;
    private RaycastHit _slopeHit;

    private bool _standingOnGround = false;

    [SerializeField] private LayerMask groundLayer;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Check if player is grounded using CharacterController
        isGrounded = controller.isGrounded;

        // Extra ground detection for slope handling
        _standingOnGround = CheckStandingOnGround();

        // Disable movement while climbing
        if (isClimbing)
        {
            animator.SetBool("IsWalking", false);
            animator.SetBool("IsClimbing", true);
            velocity = Vector3.zero;
            return;
        }
        else
        {
            animator.SetBool("IsClimbing", false);
        }

        // Ground snap and gravity handling
        if (isGrounded && velocity.y < 0f)
        {
            velocity.y = -2f;

            if (_standingOnGround)
            {
                velocity.x = 0f;
                velocity.z = 0f;
            }
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }

        // Jump input
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        // Handle steep slope movement
        if (!_standingOnGround && OnSteepSlope())
        {
            SteepSlopeMovement();
        }

        // Movement input
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        bool isMoving = direction.magnitude > 0.1f;
        animator.SetBool("IsWalking", isMoving);

        Vector3 moveThisFrame = Vector3.zero;

        if (isMoving)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;

            float angle = Mathf.SmoothDampAngle(
                transform.eulerAngles.y,
                targetAngle,
                ref turnSmoothVelocity,
                turnSmoothTime
            );

            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            moveThisFrame = moveDir.normalized * speed * Time.deltaTime;
        }

        moveThisFrame += velocity * Time.deltaTime;
        controller.Move(moveThisFrame);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = true;

        // Poison damage + sound effect
        if (collision.gameObject.CompareTag("Poison") && !isClimbing)
        {
            FindObjectOfType<GameManager>().DecreaseHealth(2);
            audioSource.PlayOneShot(poisonSound);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Fall damage from ladder
        if (other.CompareTag("LadderFall") && !isClimbing)
        {
            FindObjectOfType<GameManager>().DecreaseHealth(4);
            audioSource.PlayOneShot(poisonSound);
        }
    }

    private bool CheckStandingOnGround()
    {
        Vector3 origin = transform.position;
        float radius = controller.radius * 0.95f;
        float distance = controller.height / 2f + 2f;

        if (Physics.SphereCast(origin, radius, Vector3.down, out RaycastHit hit, distance, groundLayer))
            return true;

        return false;
    }

    private bool OnSteepSlope()
    {
        if (!controller.isGrounded) return false;
        if (_standingOnGround) return false;

        if (Physics.Raycast(
            transform.position,
            Vector3.down,
            out _slopeHit,
            (controller.height / 2f) + _groundRayDistance))
        {
            if (_slopeHit.collider.gameObject.layer == LayerMask.NameToLayer("Koshki"))
            {
                float angle = Vector3.Angle(_slopeHit.normal, Vector3.up);
                return angle > controller.slopeLimit;
            }
        }

        return false;
    }

    private void SteepSlopeMovement()
    {
        Vector3 slopeDirection = Vector3.ProjectOnPlane(Vector3.down, _slopeHit.normal).normalized;
        velocity.x = slopeDirection.x * speed;
        velocity.z = slopeDirection.z * speed;
    }
}