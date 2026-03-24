using UnityEngine;

public class Crouch : MonoBehaviour
{
    public KeyCode key = KeyCode.LeftControl;

    [Header("Slow Movement")]
    [Tooltip("Movement to slow down when crouched.")]
    public FirstPersonMovement movement;
    [Tooltip("Movement speed when crouched.")]
    public float movementSpeed = 2;

    [Header("Low Head")]
    [Tooltip("Head to lower when crouched.")]
    public Transform headToLower;
    [HideInInspector]
    public float? defaultHeadYLocalPosition;
    public float crouchYHeadPosition = 1;
    
    [Tooltip("Collider to lower when crouched.")]
    public CapsuleCollider colliderToLower;
    [HideInInspector]
    public float? defaultColliderHeight;
    [HideInInspector]
    public Vector3? defaultColliderCenter; // NEW: cache the default center

    public bool IsCrouched { get; private set; }
    public event System.Action CrouchStart, CrouchEnd;


    void Reset()
    {
        // Try to get components.
        movement = GetComponentInParent<FirstPersonMovement>();
        headToLower = movement.GetComponentInChildren<Camera>().transform;
        colliderToLower = movement.GetComponentInChildren<CapsuleCollider>();
    }

    void LateUpdate()
    {
        if (Input.GetKey(key))
        {
            // Enforce a low head.
            if (headToLower)
            {
                // If we don't have the defaultHeadYLocalPosition, get it now.
                if (!defaultHeadYLocalPosition.HasValue)
                {
                    defaultHeadYLocalPosition = headToLower.localPosition.y;
                }

                // Lower the head.
                headToLower.localPosition = new Vector3(headToLower.localPosition.x, crouchYHeadPosition, headToLower.localPosition.z);
            }

            // Enforce a low colliderToLower.
            if (colliderToLower)
            {
                // If we don't have the defaults, get them now.
                if (!defaultColliderHeight.HasValue)
                {
                    defaultColliderHeight = colliderToLower.height;
                    defaultColliderCenter = colliderToLower.center; // NEW: cache center alongside height
                }

                // Get lowering amount.
                float loweringAmount;
                if(defaultHeadYLocalPosition.HasValue)
                {
                    loweringAmount = defaultHeadYLocalPosition.Value - crouchYHeadPosition;
                }
                else
                {
                    loweringAmount = defaultColliderHeight.Value * .5f;
                }

                // Lower the colliderToLower.
                colliderToLower.height = Mathf.Max(defaultColliderHeight.Value - loweringAmount, 0);
                // NEW: offset center down by half the lowering amount to keep the bottom grounded
                colliderToLower.center = defaultColliderCenter.Value - Vector3.up * (loweringAmount * 0.5f);
            }

            // Set IsCrouched state.
            if (!IsCrouched)
            {
                IsCrouched = true;
                SetSpeedOverrideActive(true);
                CrouchStart?.Invoke();
            }
        }
        else
        {
            if (IsCrouched)
            {
                // Rise the head back up.
                if (headToLower)
                {
                    headToLower.localPosition = new Vector3(headToLower.localPosition.x, defaultHeadYLocalPosition.Value, headToLower.localPosition.z);
                }

                // Reset the colliderToLower's height and center.
                if (colliderToLower)
                {
                    colliderToLower.height = defaultColliderHeight.Value;
                    colliderToLower.center = defaultColliderCenter.Value; // NEW: restore cached center
                }

                // Reset IsCrouched.
                IsCrouched = false;
                SetSpeedOverrideActive(false);
                CrouchEnd?.Invoke();
            }
        }
    }


    #region Speed override.
    void SetSpeedOverrideActive(bool state)
    {
        // Stop if there is no movement component.
        if(!movement)
        {
            return;
        }

        // Update SpeedOverride.
        if (state)
        {
            // Try to add the SpeedOverride to the movement component.
            if (!movement.speedOverrides.Contains(SpeedOverride))
            {
                movement.speedOverrides.Add(SpeedOverride);
            }
        }
        else
        {
            // Try to remove the SpeedOverride from the movement component.
            if (movement.speedOverrides.Contains(SpeedOverride))
            {
                movement.speedOverrides.Remove(SpeedOverride);
            }
        }
    }

    float SpeedOverride() => movementSpeed;
    #endregion
}