using UnityEngine;

public class PlayerNoise : MonoBehaviour
{
    public float currentNoise = 0f;

    [Header("Noise Levels")]
    public float idleNoise = 0f;
    public float walkNoise = 0.4f;
    public float runNoise = 1f;

    public void SetIdle()
    {
        currentNoise = idleNoise;
    }

    public void SetWalking()
    {
        currentNoise = walkNoise;
    }

    public void SetRunning()
    {
        currentNoise = runNoise;
    }
}