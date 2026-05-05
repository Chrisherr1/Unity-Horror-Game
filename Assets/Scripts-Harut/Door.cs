using UnityEngine;

public class Door : MonoBehaviour
{
public float openAngle = 90f;
public float openSpeed = 2f;


private bool isOpen = false;
private bool playerNearby = false;

private Quaternion closedRotation;
private Quaternion openRotation;

void Start()
{
    closedRotation = transform.rotation;
    openRotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0, openAngle, 0));
}

void Update()
{
    if (playerNearby && Input.GetKeyDown(KeyCode.E))
    {
        isOpen = !isOpen;
    }

    // Smooth rotation
    transform.rotation = Quaternion.Slerp(
        transform.rotation,
        isOpen ? openRotation : closedRotation,
        Time.deltaTime * openSpeed
    );
}

private void OnTriggerEnter(Collider other)
{
    if (other.CompareTag("Player"))
        playerNearby = true;
}

private void OnTriggerExit(Collider other)
{
    if (other.CompareTag("Player"))
        playerNearby = false;
}


}
