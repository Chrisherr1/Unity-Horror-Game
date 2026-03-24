using UnityEngine;

// jumping works, need to assign animation
public class PlayerJump : MonoBehaviour
{
    Rigidbody rigidbody;
    public float jumpStrength = 6f;
    public event System.Action Jumped;

    [SerializeField, Tooltip("Prevents jumping when the transform is in mid-air.")]
    // prevents infinite jumping (checks if collider at player's feet has tag "Ground")
    PlayerGroundCheck groundCheck;

    void Reset()
    {
        // initializes ground check object 
        groundCheck = GetComponentInChildren<PlayerGroundCheck>();
    }

    // called before Start 
    void Awake()
    {
        // get rigidbody component 
        rigidbody = GetComponent<Rigidbody>();

        // set groundCheck in case it wasn't assigned properly the first time
        if (groundCheck == null)
            groundCheck = GetComponentInChildren<PlayerGroundCheck>();
    }

    void Update()
    {
        // if jump pressed and either no ground or player is grounded, jump
        if (Input.GetButtonDown("Jump") && groundCheck.isGrounded)
        {
            //Debug.Log("Jumping!");
            var v = rigidbody.linearVelocity;
            if (v.y < 0f) v.y = 0f;
            rigidbody.linearVelocity = v;

            rigidbody.AddForce(Vector3.up * jumpStrength, ForceMode.Impulse);

            Jumped?.Invoke();
        }

    }
}