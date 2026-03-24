using UnityEngine;
public class PlayerAnimatorBridge : MonoBehaviour
{
    [Header("References")]
    public Animator animator;
    public Rigidbody rb;
    public PlayerGroundCheck groundCheck;
    public PlayerJump playerJump;

    void Start()
    {
        if (animator == null) animator = GetComponentInChildren<Animator>();
        if (rb == null) rb = GetComponent<Rigidbody>();
        if (groundCheck == null) groundCheck = GetComponentInChildren<PlayerGroundCheck>();
        if (playerJump == null) playerJump = GetComponent<PlayerJump>();
    }

    void OnEnable()
    {
        if (playerJump != null) playerJump.Jumped += OnJumped;
    }

    void OnDisable()
    {
        if (playerJump != null) playerJump.Jumped -= OnJumped;
    }

    void OnJumped() => animator.SetTrigger("Jump");

    void Update()
    {
        bool isGrounded = groundCheck != null && groundCheck.isGrounded;
        float speed = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z).magnitude;

        animator.SetFloat("Speed", speed);
        animator.SetBool("Grounded", isGrounded);
        animator.SetBool("FreeFall", !isGrounded && rb.linearVelocity.y < -0.5f);
        animator.SetFloat("MotionSpeed", speed > 5f ? 2f : 1f);
    }
}