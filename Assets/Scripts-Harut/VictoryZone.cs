using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryZone : MonoBehaviour
{
    public GameObject victoryPanel;
    public GameObject player;

    private bool won = false;

    private void Start()
    {
        victoryPanel.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (won) return;

        if (other.CompareTag("Player"))
        {
            won = true;
            ShowVictory();
        }
    }

    void ShowVictory()
    {
        victoryPanel.SetActive(true);

        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ExitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}