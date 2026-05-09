using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterAI : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public NavMeshAgent agent;
    public Animator animator;

    [Header("Detection")]
    public float detectionRange = 15f;
    public float losePlayerRange = 20f;
    public float attackRange = 2.5f;

    [Header("Movement")]
    public float walkSpeed = 2f;
    public float chaseSpeed = 5f;

    [Header("Patrol")]
    public List<Transform> waypoints;
    public float minIdleTime = 2f;
    public float maxIdleTime = 5f;

    [Header("Settings")]
    public float endChaseDelay = 3f;
    public float attackCooldown = 2f;

    [Header("Mirror Teleport")]
    public List<Transform> mirrors;
    public float mirrorReachDistance = 2f;
    public float mirrorAlertRange = 5f;
    public float mirrorCooldown = 5f;

    private enum State { Patrolling, Chasing, GoingToMirror, Attacking }
    private State state = State.Patrolling;

    private bool isIdling = false;
    private bool isAttacking = false;
    private Coroutine endChaseCoroutine;
    private string targetAnim = "";
    private Transform teleportTarget;
    private float mirrorCooldownTimer = 0f;

    private static readonly string[] attackAnims = { "attack1", "attack2", "attack3", "attack4", "attack5" };
    private static readonly string[] idleAnims   = { "idle1", "idle2", "idle3", "idle4" };
    private static readonly string[] walkAnims   = { "walk2", "walk3", "walk4" };
    private static readonly string[] runAnims    = { "run1", "run2", "run3" };

    void Start()
    {
        SetNewPatrolDestination();
        PlayAnim(walkAnims);
    }

    void Update()
    {
        float dist = Vector3.Distance(transform.position, player.position);

        if (mirrorCooldownTimer > 0f)
            mirrorCooldownTimer -= Time.deltaTime;
        else
            CheckMirrorAlert();

        switch (state)
        {
            case State.Patrolling:    UpdatePatrol(dist);    break;
            case State.Chasing:       UpdateChase(dist);     break;
            case State.GoingToMirror: UpdateGoingToMirror(); break;
        }

        if (!isAttacking && !string.IsNullOrEmpty(targetAnim) &&
            !animator.GetCurrentAnimatorStateInfo(0).IsName(targetAnim))
            animator.Play(targetAnim);
    }

    void CheckMirrorAlert()
    {
        if (isAttacking) return;
        foreach (var mirror in mirrors)
        {
            if (Vector3.Distance(player.position, mirror.position) <= mirrorAlertRange)
            {
                agent.Warp(mirror.position);
                mirrorCooldownTimer = mirrorCooldown;
                EnterChase();
                agent.SetDestination(player.position);
                return;
            }
        }
    }

    void UpdatePatrol(float dist)
    {
        if (dist <= detectionRange)
        {
            EnterChase();
            return;
        }

        if (isIdling) return;

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (Random.value < 0.5f)
                StartCoroutine(IdleThenPatrol());
            else
                SetNewPatrolDestination();
        }
    }

    void UpdateChase(float dist)
    {
        if (isAttacking) return;

        if (dist > losePlayerRange)
        {
            ExitChase();
            return;
        }

        if (mirrors.Count >= 2 && mirrorCooldownTimer <= 0f)
        {
            Transform closestToMonster = GetClosestMirror(transform.position, null);
            float mirrorDist = Vector3.Distance(transform.position, closestToMonster.position);

            if (mirrorDist < dist)
            {
                Transform closestToPlayer = GetClosestMirror(player.position, closestToMonster);
                teleportTarget = closestToPlayer;
                state = State.GoingToMirror;
                agent.SetDestination(closestToMonster.position);
                return;
            }
        }

        agent.SetDestination(player.position);

        if (dist <= attackRange)
            StartCoroutine(PerformAttack());
    }

    void UpdateGoingToMirror()
    {
        if (!agent.pathPending && agent.remainingDistance <= mirrorReachDistance)
        {
            agent.Warp(teleportTarget.position);
            mirrorCooldownTimer = mirrorCooldown;
            state = State.Chasing;
            agent.SetDestination(player.position);
        }
    }

    void EnterChase()
    {
        StopAllCoroutines();
        isIdling = false;
        state = State.Chasing;
        agent.speed = chaseSpeed;
        PlayAnim(runAnims);
        MusicManager.Instance.StartChase();
    }

    void ExitChase()
    {
        state = State.Patrolling;
        agent.speed = walkSpeed;
        SetNewPatrolDestination();
        PlayAnim(walkAnims);
        endChaseCoroutine = StartCoroutine(DelayedEndChase());
    }

    IEnumerator PerformAttack()
    {
        isAttacking = true;
        state = State.Attacking;
        agent.ResetPath();
        agent.velocity = Vector3.zero;

        Vector3 dir = (player.position - transform.position).normalized;
        dir.y = 0;
        if (dir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(dir);

        animator.Play(attackAnims[Random.Range(0, attackAnims.Length)]);
        yield return new WaitForSeconds(attackCooldown);

        isAttacking = false;
        state = State.Chasing;
        PlayAnim(runAnims);
    }

    IEnumerator IdleThenPatrol()
    {
        isIdling = true;
        agent.ResetPath();
        PlayAnim(idleAnims);
        yield return new WaitForSeconds(Random.Range(minIdleTime, maxIdleTime));
        isIdling = false;
        SetNewPatrolDestination();
        PlayAnim(walkAnims);
    }

    IEnumerator DelayedEndChase()
    {
        yield return new WaitForSeconds(endChaseDelay);
        MusicManager.Instance.EndChase();
    }

    Transform GetClosestMirror(Vector3 position, Transform exclude)
    {
        Transform closest = null;
        float minDist = float.MaxValue;
        foreach (var mirror in mirrors)
        {
            if (mirror == exclude) continue;
            float d = Vector3.Distance(position, mirror.position);
            if (d < minDist)
            {
                minDist = d;
                closest = mirror;
            }
        }
        return closest;
    }

    void SetNewPatrolDestination()
    {
        if (waypoints.Count == 0) return;
        agent.SetDestination(waypoints[Random.Range(0, waypoints.Count)].position);
    }

    void PlayAnim(string[] options)
    {
        targetAnim = options[Random.Range(0, options.Length)];
        animator.Play(targetAnim);
    }
}
