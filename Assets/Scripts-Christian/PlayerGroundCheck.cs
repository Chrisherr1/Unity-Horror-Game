using UnityEngine;

public class PlayerGroundCheck : MonoBehaviour
{
    public float distanceThreshold = 0.3f;
    public float radius = 0.2f;
    public LayerMask groundMask;

    public bool isGrounded;
    public event System.Action Grounded;

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

        if (groundedNow && !isGrounded)
            Grounded?.Invoke();

        isGrounded = groundedNow;
    }
}