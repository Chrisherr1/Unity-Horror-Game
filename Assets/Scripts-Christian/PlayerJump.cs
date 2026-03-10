using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    Rigidbody rigidbody;
    public float jumpStrength = 6f;
    public event System.Action Jumped;

    [SerializeField, Tooltip("Prevents jumping when the transform is in mid-air.")]
    PlayerGroundCheck groundCheck;

    private Animator animator;

    void Reset()
    {
        groundCheck = GetComponentInChildren<PlayerGroundCheck>();
    }

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Jump") && (groundCheck == null || groundCheck.isGrounded))
        {
            var v = rigidbody.linearVelocity;
            if (v.y < 0f) v.y = 0f;
            rigidbody.linearVelocity = v;

            rigidbody.AddForce(Vector3.up * jumpStrength, ForceMode.Impulse);

            if (animator != null) animator.SetTrigger("Jump");
            Jumped?.Invoke();
        }
    }
}