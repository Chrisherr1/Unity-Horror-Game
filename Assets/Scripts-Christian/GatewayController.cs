using System.Collections;
using UnityEngine;

public class GatewayController : MonoBehaviour
{
    public Transform player;
    public float openRange = 5f;
    public float closeDelay = 10f;
    public Animation gateAnimation;
    public AnimationClip openClip;

    private bool playerInRange = false;
    private bool isOpen = false;
    private Coroutine closeCoroutine;

    void Update()
    {
        bool nowInRange = Vector3.Distance(transform.position, player.position) <= openRange;
        if (nowInRange == playerInRange) return;

        playerInRange = nowInRange;
        if (playerInRange)
        {
            if (closeCoroutine != null) StopCoroutine(closeCoroutine);
            OpenGate();
        }
        else
        {
            closeCoroutine = StartCoroutine(DelayedClose());
        }
    }

    void OpenGate()
    {
        if (isOpen) return;
        isOpen = true;
        gateAnimation.Stop(openClip.name);
        var state = gateAnimation[openClip.name];
        state.wrapMode = WrapMode.ClampForever;
        state.speed = 1f;
        state.time = 0f;
        gateAnimation.Play(openClip.name);
    }

    IEnumerator DelayedClose()
    {
        yield return new WaitForSeconds(closeDelay);
        CloseGate();
    }

    void CloseGate()
    {
        if (!isOpen) return;
        isOpen = false;
        var state = gateAnimation[openClip.name];
        state.wrapMode = WrapMode.ClampForever;
        state.speed = -1f;
        state.time = state.length;
        gateAnimation.Play(openClip.name);
    }
}
