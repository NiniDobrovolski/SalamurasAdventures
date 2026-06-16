using UnityEngine;
using System.Collections;

public class PlayerController4 : MonoBehaviour
{
    public Animator animator;
    public bool IsFalling = false;

    public float jumpForce = 7f;
    public float flipSpeed = 1000f;

    // 2 = double flip, 3 = triple flip
    public int flipCount = 2;

    private bool isFlipping = false;
    private Rigidbody rb;
}