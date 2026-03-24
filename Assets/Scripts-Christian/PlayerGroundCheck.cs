using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
public class PlayerGroundCheck : MonoBehaviour
{
    public float distanceThreshold = 0.8f;
    public float radius = 0.25f;
    public LayerMask groundMask;
    // own variable, isGrounded is automatically true
    public bool isGrounded;
    public event System.Action Grounded;
    //public event System.Action LeftGround;
    const float OriginOffset = 0.1f;
    void Update()
    {
        Vector3 origin = transform.position + Vector3.down * OriginOffset;

        RaycastHit hit;

        bool groundedNow = Physics.SphereCast(
            origin,
            radius,
            Vector3.down,
            out hit,
            distanceThreshold,
            groundMask,
            QueryTriggerInteraction.Ignore
        );

        Debug.Log("groundedNow: " + groundedNow);

        // DO NOT COMMENT OUT, WILL NOT WORK IF YOU DO
        Debug.Log("Hit object: " + hit.collider.gameObject.name);

        // if grounded now but not grounded before, invoke grounded
        //if (groundedNow && !isGrounded) Grounded?.Invoke();

        // if not grounded now but grounded before, invoke left ground
        //if (!groundedNow && isGrounded) LeftGround?.Invoke();
        isGrounded = groundedNow;
    }
}