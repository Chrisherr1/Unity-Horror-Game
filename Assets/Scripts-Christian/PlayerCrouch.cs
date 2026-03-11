using UnityEngine;

public class PlayerCrouch : MonoBehaviour
{
    public KeyCode key = KeyCode.LeftControl;

    [Header("Slow Movement")]
    public FirstPersonMovement movement;
    public float movementSpeed = 2;

    [Header("Low Head")]
    public Transform headToLower;
    [HideInInspector] public float? defaultHeadYLocalPosition;
    public float crouchYHeadPosition = 1;

    [Tooltip("Collider to lower when crouched.")]
    public CapsuleCollider colliderToLower;
    [HideInInspector] public float? defaultColliderHeight;
    [HideInInspector] public Vector3? defaultColliderCenter;

    [Header("Animation")]
    public Animator animator;

    public bool IsCrouched { get; private set; }
    public event System.Action CrouchStart, CrouchEnd;

    void Reset()
    {
        movement = GetComponentInParent<FirstPersonMovement>();
        headToLower = movement.GetComponentInChildren<Camera>().transform;
        colliderToLower = movement.GetComponentInChildren<CapsuleCollider>();
    }

    void Awake()
    {
        if (animator == null) animator = GetComponentInChildren<Animator>();
    }

    void LateUpdate()
    {
        if (Input.GetKey(key))
        {
            if (headToLower)
            {
                if (!defaultHeadYLocalPosition.HasValue)
                    defaultHeadYLocalPosition = headToLower.localPosition.y;

                headToLower.localPosition = new Vector3(headToLower.localPosition.x, crouchYHeadPosition, headToLower.localPosition.z);
            }

            if (colliderToLower)
            {
                if (!defaultColliderHeight.HasValue)
                {
                    defaultColliderHeight = colliderToLower.height;
                    defaultColliderCenter = colliderToLower.center;
                }

                float loweringAmount = defaultHeadYLocalPosition.HasValue
                    ? defaultHeadYLocalPosition.Value - crouchYHeadPosition
                    : defaultColliderHeight.Value * .5f;

                colliderToLower.height = Mathf.Max(defaultColliderHeight.Value - loweringAmount, 0);
                colliderToLower.center = defaultColliderCenter.Value - Vector3.up * (loweringAmount * 0.5f);
            }

            if (!IsCrouched)
            {
                IsCrouched = true;
                SetSpeedOverrideActive(true);
                if (animator != null) animator.SetBool("Crouch", true);
                CrouchStart?.Invoke();
            }
        }
        else
        {
            if (IsCrouched)
            {
                if (headToLower)
                    headToLower.localPosition = new Vector3(headToLower.localPosition.x, defaultHeadYLocalPosition.Value, headToLower.localPosition.z);

                if (colliderToLower)
                {
                    colliderToLower.height = defaultColliderHeight.Value;
                    colliderToLower.center = defaultColliderCenter.Value;
                }

                IsCrouched = false;
                SetSpeedOverrideActive(false);
                if (animator != null) animator.SetBool("Crouch", false);
                CrouchEnd?.Invoke();
            }
        }
    }

    #region Speed override
    void SetSpeedOverrideActive(bool state)
    {
        if (!movement) return;

        if (state)
        {
            if (!movement.speedOverrides.Contains(SpeedOverride))
                movement.speedOverrides.Add(SpeedOverride);
        }
        else
        {
            if (movement.speedOverrides.Contains(SpeedOverride))
                movement.speedOverrides.Remove(SpeedOverride);
        }
    }

    float SpeedOverride() => movementSpeed;
    #endregion
}