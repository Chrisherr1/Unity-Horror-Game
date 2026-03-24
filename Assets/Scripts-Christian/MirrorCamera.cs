using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MirrorCamera : MonoBehaviour
{
    public Transform mirrorTransform;
    public Transform playerCamera;
    public RenderTexture mirrorTexture;

    [Range(1, 8)]
    public int blurAmount = 4;

    Camera mirrorCam;
    int frameCount = 0;

    void Start()
    {
        mirrorCam = GetComponent<Camera>();
        mirrorCam.targetTexture = mirrorTexture;
        mirrorCam.transform.localPosition = new Vector3(0, 0, 0.01f);
        mirrorCam.transform.localRotation = Quaternion.Euler(0, 180, 0);
    }

    void Update()
    {
        if (playerCamera == null || mirrorTransform == null) return;

        // Performance: only render every 3rd frame
        frameCount++;
        if (frameCount % 3 != 0) return;

        // Performance: skip if player isn't looking at mirror
        Vector3 dirToPlayer = playerCamera.position - mirrorTransform.position;
        float dot = Vector3.Dot(mirrorTransform.forward, dirToPlayer);
        if (dot > 0) 
        {
            mirrorCam.enabled = false;
            return;
        }
        mirrorCam.enabled = true;

        // Mirror the player camera position across the mirror plane
        Vector3 mirrorNormal = mirrorTransform.forward;
        Vector3 playerPos = playerCamera.position;
        float distToMirror = Vector3.Dot(playerPos - mirrorTransform.position, mirrorNormal);
        mirrorCam.transform.position = playerPos - 2f * distToMirror * mirrorNormal;

        // Mirror the rotation
        Vector3 reflectedForward = Vector3.Reflect(playerCamera.forward, mirrorNormal);
        Vector3 reflectedUp = Vector3.Reflect(playerCamera.up, mirrorNormal);
        mirrorCam.transform.rotation = Quaternion.LookRotation(reflectedForward, reflectedUp);
    }
}