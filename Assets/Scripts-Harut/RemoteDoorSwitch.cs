using UnityEngine;
using TMPro;

public class RemoteDoorSwitch : MonoBehaviour
{
    [Header("UI")]
    public GameObject pressEText;
    public TextMeshProUGUI messageText;
    public string openedMessage = "The big doors have opened.";

    [Header("Doors")]
    public Transform leftDoorHinge;
    public Transform rightDoorHinge;

    public float openAngle = 90f;
    public float openSpeed = 3f;

    private bool playerNearby = false;
    private bool used = false;

    private Quaternion leftClosed;
    private Quaternion rightClosed;

    private Quaternion leftTarget;
    private Quaternion rightTarget;

    void Start()
    {
        leftClosed = leftDoorHinge.localRotation;
        rightClosed = rightDoorHinge.localRotation;

        leftTarget = leftClosed;
        rightTarget = rightClosed;

        pressEText.SetActive(false);
        messageText.gameObject.SetActive(false);
    }

    void Update()
    {
        leftDoorHinge.localRotation = Quaternion.Slerp(
            leftDoorHinge.localRotation,
            leftTarget,
            Time.deltaTime * openSpeed
        );

        rightDoorHinge.localRotation = Quaternion.Slerp(
            rightDoorHinge.localRotation,
            rightTarget,
            Time.deltaTime * openSpeed
        );

        if (playerNearby && !used && Input.GetKeyDown(KeyCode.E))
        {
            OpenDoors();
        }
    }

    void OpenDoors()
    {
        used = true;
        playerNearby = false;

        leftTarget = leftClosed * Quaternion.Euler(0f, -openAngle, 0f);
        rightTarget = rightClosed * Quaternion.Euler(0f, openAngle, 0f);

        pressEText.SetActive(false);

        messageText.text = openedMessage;
        messageText.gameObject.SetActive(true);

        Invoke(nameof(HideMessage), 3f);
    }

    void HideMessage()
    {
        messageText.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !used)
        {
            playerNearby = true;
            pressEText.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            pressEText.SetActive(false);
        }
    }
}