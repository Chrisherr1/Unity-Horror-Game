using UnityEngine;

public class Footsteps : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] footstepClips;

    public float walkVolume = 0.5f;
    public float runVolume = 2f;

    public void PlayFootstepWalk()
    {
        PlayFootstep(walkVolume);
    }

    public void PlayFootstepRun()
    {
        PlayFootstep(runVolume);
    }

    void PlayFootstep(float volume)
    {
        if (audioSource == null || footstepClips.Length == 0)
            return;

        AudioClip clip = footstepClips[Random.Range(0, footstepClips.Length)];
        audioSource.pitch = Random.Range(0.95f, 1.05f);
        audioSource.PlayOneShot(clip, volume);
    }
}