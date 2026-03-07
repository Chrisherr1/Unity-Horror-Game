using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class MirrorEnemyAI : MonoBehaviour
{
    public NavMeshAgent ai;
    public List<Transform> destinations;
    public Animator aiAnimator;

    public float walkSpeed, chaseSpeed,minIdleTime, maxIdleTime,idleTime;
    public bool walking, chasing, idling;
    public Transform player;
    Transform currentDestination;
    Vector3 destination;

    int randNum, randNum2;

    public int destinationAmount;

    void Start()
    {
        walking = true;
        randNum = Random.Range(0, destinationAmount);
        currentDestination = destinations[randNum];

    }
    void Update()
    {
        if(walking == true)
        {
            destination = currentDestination.position;
            ai.destination = destination;
            ai.speed =  walkSpeed;
            if(ai.remainingDistance <= ai.stoppingDistance)
            {
                randNum2 = Random.Range(0,2);
                if(randNum2 == 0)
                {
                    randNum = Random.Range(0, destinationAmount);
                    currentDestination = destinations[randNum];
                }
                if(randNum2 == 1)
                {
                    aiAnimator.ResetTrigger("walk");
                    aiAnimator.SetTrigger("idle");
                    StopCoroutine("stayIdle");
                    StartCoroutine("stayIdle");
                    walking = false;
                }

            }
        }
    }
    IEnumerator stayIdle()
    {
        idleTime = Random.Range(minIdleTime,maxIdleTime);
        yield return new WaitForSeconds(idleTime);
        walking = true;
        randNum = Random.Range(0, destinationAmount);
        currentDestination = destinations[randNum];
        aiAnimator.ResetTrigger("idle");
        aiAnimator.SetTrigger("walk");

    }
}
