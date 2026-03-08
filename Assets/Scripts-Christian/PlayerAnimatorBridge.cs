using UnityEngine;

public class PlayerAnimatorBridge : MonoBehaviour
{
    [Header("References")]
    public Animator animator;
    public Rigidbody rb;

    [Header("Settings")]
    public float groundCheckDistance = 1.1f;
    public float groundCheckRadius = 0.3f;
    public LayerMask groundLayer;

    private bool wasGrounded;

    void Start()
    {
        // Explicit assignment instead of auto-finding
        if (animator == null) animator = GetComponentInChildren<Animator>();
        if (rb == null) rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        bool isGrounded = CheckGrounded();
        float speed = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z).magnitude;
        float verticalVelocity = rb.linearVelocity.y;

        animator.SetFloat("Speed", speed);
        animator.SetBool("Grounded", isGrounded);
        animator.SetBool("FreeFall", !isGrounded && verticalVelocity < -0.5f);
        animator.SetFloat("MotionSpeed", speed > 5f ? 2f : 1f);

        // Fixed jump detection
        if (wasGrounded && !isGrounded && verticalVelocity > 0.1f)
        {
            animator.SetTrigger("Jump");
        }

        wasGrounded = isGrounded;
    }

    private bool CheckGrounded()
    {
        return Physics.SphereCast(
            transform.position,
            groundCheckRadius,
            Vector3.down,
            out RaycastHit hit,
            groundCheckDistance,
            groundLayer
        );
    }
}