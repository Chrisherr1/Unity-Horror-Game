using UnityEngine;

public class SmartDoor : MonoBehaviour
{
    [Header("Setup")]
    public Transform player;
    public float openAngle = 90f;
    public float openSpeed = 4f;

    [Header("State")]
    public bool isOpen = false;
    private bool playerNearby = false;

    private Quaternion closedRotation;
    private Quaternion targetRotation;

    void Start()
    {
        closedRotation = transform.localRotation;
        targetRotation = closedRotation;
    }

    void Update()
    {
        if (playerNearby && Input.GetKeyDown(KeyCode.E))
        {
            if (!isOpen)
            {
                OpenAwayFromPlayer();
            }
            else
            {
                CloseDoor();
            }
        }

        transform.localRotation = Quaternion.Slerp(
            transform.localRotation,
            targetRotation,
            Time.deltaTime * openSpeed
        );
    }

    void OpenAwayFromPlayer()
    {
        Vector3 localPlayerPos = transform.InverseTransformPoint(player.position);

        // If player is on one side, open opposite
        float direction = (localPlayerPos.x >= 0f) ? -1f : 1f;

        targetRotation = closedRotation * Quaternion.Euler(0f, openAngle * direction, 0f);
        isOpen = true;
    }

    void CloseDoor()
    {
        targetRotation = closedRotation;
        isOpen = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;

            if (player == null)
                player = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
        }
    }
}