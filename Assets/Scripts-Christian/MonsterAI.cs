using UnityEngine;
using System.Collections;

public class MonsterAI : MonoBehaviour
{
    [Header("Detection")]
    public Transform player;
    public float detectionRange = 15f;
    public float losePlayerRange = 20f;

    [Header("Settings")]
    public float endChaseDelay = 3f; // seconds before music fades out

    private bool isChasingPlayer = false;
    private Coroutine endChaseCoroutine;

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange && !isChasingPlayer)
        {
            // Monster spots the player
            isChasingPlayer = true;
            if (endChaseCoroutine != null) StopCoroutine(endChaseCoroutine);
            MusicManager.Instance.StartChase();
        }
        else if (distanceToPlayer > losePlayerRange && isChasingPlayer)
        {
            // Monster loses the player
            isChasingPlayer = false;
            endChaseCoroutine = StartCoroutine(DelayedEndChase());
        }
    }

    IEnumerator DelayedEndChase()
    {
        yield return new WaitForSeconds(endChaseDelay); // tension delay
        MusicManager.Instance.EndChase();
    }
}