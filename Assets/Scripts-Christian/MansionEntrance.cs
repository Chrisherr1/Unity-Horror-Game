using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MansionEntrance : MonoBehaviour
{
    [Header("Settings")]
    public string sceneToLoad = "MansionInterior";
    public KeyCode interactKey = KeyCode.E;

    [Header("UI")]
    public GameObject promptUI;

    private bool playerInRange = false;

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(interactKey))
            SceneManager.LoadScene(sceneToLoad);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = true;
        if (promptUI != null) promptUI.SetActive(true);
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = false;
        if (promptUI != null) promptUI.SetActive(false);
    }
}
