using UnityEngine;

public class Jump : MonoBehaviour
{
    Rigidbody rigidbody;
    public float jumpStrength = 2;
    public event System.Action Jumped;

    [SerializeField, Tooltip("Prevents jumping when the transform is in mid-air.")]
    GroundCheck groundCheck;

    private Animator animator;

    void Reset()
    {
        groundCheck = GetComponentInChildren<GroundCheck>();
    }

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        // Find animator on this object or any child
        animator = GetComponentInChildren<Animator>();
    }

    void LateUpdate()
    {
        if (Input.GetButtonDown("Jump") && (!groundCheck || groundCheck.isGrounded))
        {
            rigidbody.AddForce(Vector3.up * 100 * jumpStrength);
            
            // Trigger jump animation
            if (animator != null)
            {
                animator.SetTrigger("Jump");
            }

            Jumped?.Invoke();
        }
    }
}