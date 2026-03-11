using UnityEngine;
public class PlayerGroundCheck : MonoBehaviour
{
    public float distanceThreshold = 0.8f;
    public float radius = 0.25f;
    public LayerMask groundMask;
    public bool isGrounded;
    public event System.Action Grounded;
    public event System.Action LeftGround;
    const float OriginOffset = 0.02f;
    void Update()
    {
        Vector3 origin = transform.position + Vector3.up * OriginOffset;
        bool groundedNow = Physics.SphereCast(
            origin,
            radius,
            Vector3.down,
            out _,
            distanceThreshold,
            groundMask,
            QueryTriggerInteraction.Ignore
        );
        if (groundedNow && !isGrounded) Grounded?.Invoke();
        if (!groundedNow && isGrounded) LeftGround?.Invoke();
        isGrounded = groundedNow;
    }
}