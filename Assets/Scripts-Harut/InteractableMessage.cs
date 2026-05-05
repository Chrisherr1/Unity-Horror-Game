using UnityEngine;
using TMPro;

public class InteractableMessage : MonoBehaviour
{
    public string message = "Hello!";
    public TextMeshProUGUI messageText;
    public GameObject pressEText;
    public GameObject darkOverlay;

    private bool playerNearby = false;
    private bool messageOpen = false;

    void Update()
    {
        if (playerNearby && !messageOpen && Input.GetKeyDown(KeyCode.E))
        {
            ShowMessage();
        }

        if (messageOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            HideMessage();
        }
    }

    void ShowMessage()
    {
        messageOpen = true;

        messageText.text = message;
        messageText.gameObject.SetActive(true);
        darkOverlay.SetActive(true);

        if (pressEText != null)
            pressEText.SetActive(false);
    }

    void HideMessage()
    {
        messageOpen = false;

        messageText.gameObject.SetActive(false);
        darkOverlay.SetActive(false);

        if (playerNearby && pressEText != null)
            pressEText.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;

            if (!messageOpen && pressEText != null)
                pressEText.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;

            if (pressEText != null)
                pressEText.SetActive(false);

            if (messageOpen)
                HideMessage();
        }
    }
}