using UnityEngine;
using TMPro;

public class CodeDoor : MonoBehaviour
{
    [Header("Door")]
    public float openAngle = 90f;
    public float openSpeed = 4f;
    public string correctCode = "1234";

    [Header("UI")]
    public GameObject pressEText;
    public GameObject codePanel;
    public GameObject darkOverlay;
    public TextMeshProUGUI[] slotTexts;

    private bool playerNearby = false;
    private bool codePanelOpen = false;
    private bool doorOpen = false;

    private string currentInput = "";

    private Quaternion closedRotation;
    private Quaternion openRotation;
    private Quaternion targetRotation;

    void Start()
    {
        closedRotation = transform.localRotation;
        openRotation = closedRotation * Quaternion.Euler(0f, openAngle, 0f);
        targetRotation = closedRotation;

        codePanel.SetActive(false);
        darkOverlay.SetActive(false);
        pressEText.SetActive(false);

        ClearSlots();
    }

    void Update()
    {
        transform.localRotation = Quaternion.Slerp(
            transform.localRotation,
            targetRotation,
            Time.deltaTime * openSpeed
        );

        if (playerNearby && !doorOpen && !codePanelOpen && Input.GetKeyDown(KeyCode.E))
        {
            OpenCodePanel();
        }

        if (codePanelOpen)
        {
            HandleCodeInput();

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CloseCodePanel();
            }
        }
    }

    void HandleCodeInput()
    {
        for (int i = 0; i <= 9; i++)
        {
            if (Input.GetKeyDown(i.ToString()))
            {
                AddDigit(i.ToString());
            }
        }

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            RemoveDigit();
        }
    }

    void AddDigit(string digit)
    {
        if (currentInput.Length >= 4)
            return;

        currentInput += digit;
        UpdateSlots();

        if (currentInput.Length == 4)
        {
            CheckCode();
        }
    }

    void RemoveDigit()
    {
        if (currentInput.Length <= 0)
            return;

        currentInput = currentInput.Substring(0, currentInput.Length - 1);
        UpdateSlots();
    }

    void CheckCode()
    {
        if (currentInput == correctCode)
        {
            OpenDoor();
        }
        else
        {
            currentInput = "";
            UpdateSlots();
        }
    }

    void OpenDoor()
    {
        doorOpen = true;
        targetRotation = openRotation;
        CloseCodePanel();
    }

    void OpenCodePanel()
    {
        codePanelOpen = true;
        currentInput = "";

        UpdateSlots();

        codePanel.SetActive(true);
        darkOverlay.SetActive(true);
        pressEText.SetActive(false);
    }

    void CloseCodePanel()
    {
        codePanelOpen = false;

        codePanel.SetActive(false);
        darkOverlay.SetActive(false);

        if (playerNearby && !doorOpen)
            pressEText.SetActive(true);
    }

    void UpdateSlots()
    {
        for (int i = 0; i < slotTexts.Length; i++)
        {
            if (i < currentInput.Length)
                slotTexts[i].text = currentInput[i].ToString();
            else
                slotTexts[i].text = "_";
        }
    }

    void ClearSlots()
    {
        currentInput = "";
        UpdateSlots();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !doorOpen)
        {
            playerNearby = true;

            if (!codePanelOpen)
                pressEText.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            pressEText.SetActive(false);

            if (codePanelOpen)
                CloseCodePanel();
        }
    }
}