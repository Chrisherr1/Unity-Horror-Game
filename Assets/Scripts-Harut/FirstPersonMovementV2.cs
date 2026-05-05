using System.Collections.Generic;
using UnityEngine;

public class FirstPersonMovementV2 : MonoBehaviour
{
    public float speed = 5;

    [Header("Running")]
    public bool canRun = true;
    public bool IsRunning { get; private set; }
    public float runSpeed = 9;
    public KeyCode runningKey = KeyCode.LeftShift;

    private Rigidbody rigidbody;
    private PlayerNoise playerNoise;

    public List<System.Func<float>> speedOverrides = new List<System.Func<float>>();

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        playerNoise = GetComponent<PlayerNoise>();
    }

    void FixedUpdate()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        bool isMoving = Mathf.Abs(horizontal) > 0.1f || Mathf.Abs(vertical) > 0.1f;
        IsRunning = canRun && Input.GetKey(runningKey) && isMoving;

        float targetMovingSpeed = IsRunning ? runSpeed : speed;
        if (speedOverrides.Count > 0)
        {
            targetMovingSpeed = speedOverrides[speedOverrides.Count - 1]();
        }

        Vector2 targetVelocity = new Vector2(
            horizontal * targetMovingSpeed,
            vertical * targetMovingSpeed
        );

        rigidbody.linearVelocity = transform.rotation * new Vector3(
            targetVelocity.x,
            rigidbody.linearVelocity.y,
            targetVelocity.y
        );

        if (playerNoise != null)
        {
            if (!isMoving)
                playerNoise.SetIdle();
            else if (IsRunning)
                playerNoise.SetRunning();
            else
                playerNoise.SetWalking();
        }
    }
}