using UnityEngine;

public class SmartDoor : MonoBehaviour
{
    [Header("Settings")]
    public float openAngle = 90f;
    public float openSpeed = 4f;

    [Header("State")]
    public bool isOpen = false;

    private bool playerNearby = false;
    private Transform player;

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
            if (isOpen)
                CloseDoor();
            else
                OpenAwayFrom(player);
        }

        transform.localRotation = Quaternion.Slerp(
            transform.localRotation,
            targetRotation,
            Time.deltaTime * openSpeed
        );
    }

    public void OpenAwayFrom(Transform opener)
    {
        if (opener == null)
            return;

        Vector3 localOpenerPos = transform.InverseTransformPoint(opener.position);

        float direction = localOpenerPos.x >= 0f ? -1f : 1f;

        targetRotation = closedRotation * Quaternion.Euler(0f, openAngle * direction, 0f);
        isOpen = true;
    }

    public void CloseDoor()
    {
        targetRotation = closedRotation;
        isOpen = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
            player = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerNearby = false;
    }
}