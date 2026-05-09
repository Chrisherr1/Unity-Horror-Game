using UnityEngine;
using UnityEngine.AI;

public class PatrolMonsterAI : MonoBehaviour
{
    [Header("Player")]
    public Transform player;
    private FirstPersonAudio playerAudio;

    private NavMeshAgent agent;
    private Animator animator;

    [Header("Patrol")]
    public Transform[] patrolPoints;
    public float patrolWaitTime = 1.5f;
    private int patrolIndex = 0;
    private float patrolTimer;

    [Header("Hearing")]
    public float hearingRange = 20f;

    [Header("Chase")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4.5f;
    public float giveUpTime = 2f;
    private float loseTimer;

    [Header("Attack / Kill")]
    public float killDistance = 1.4f;

    [Header("Doors")]
    public float doorOpenRange = 2.2f;
    public LayerMask doorLayer;

    private bool isChasing = false;
    private bool wasChasing = false;
    private float smoothAnimSpeed;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();

        if (player != null)
            playerAudio = player.GetComponentInChildren<FirstPersonAudio>();

        agent.speed = patrolSpeed;
        agent.isStopped = false;

        GoToNextPatrolPoint();
    }

    void Update()
    {
        if (agent == null)
            return;

        TryOpenNearbyDoors();
        HandleHearing();
        HandleMusic();

        if (isChasing)
            ChasePlayer();
        else
            Patrol();

        UpdateAnimations();
    }

    void HandleHearing()
    {
        if (player == null || playerAudio == null)
            return;

        float currentNoise = GetPlayerNoise();
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        float effectiveHearing = currentNoise * hearingRange;

        bool canHearPlayer = distanceToPlayer <= effectiveHearing;

        if (canHearPlayer)
        {
            isChasing = true;
            loseTimer = giveUpTime;
        }
        else if (isChasing)
        {
            loseTimer -= Time.deltaTime;

            if (loseTimer <= 0f)
            {
                isChasing = false;
                agent.isStopped = false;
                GoToNextPatrolPoint();
            }
        }
    }

    float GetPlayerNoise()
    {
        if (playerAudio == null)
            return 0f;

        if (playerAudio.runningAudio != null && playerAudio.runningAudio.isPlaying)
            return 1f;

        if (playerAudio.stepAudio != null && playerAudio.stepAudio.isPlaying)
            return 0.4f;

        if (playerAudio.crouchedAudio != null && playerAudio.crouchedAudio.isPlaying)
            return 0.15f;

        return 0f;
    }

    void ChasePlayer()
    {
        if (player == null)
            return;

        agent.isStopped = false;
        agent.speed = chaseSpeed;
        agent.SetDestination(player.position);

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= killDistance)
        {
            PlayerDeath death = player.GetComponent<PlayerDeath>();

            if (death != null)
                death.Die();
        }
    }

    void Patrol()
    {
        agent.speed = patrolSpeed;

        if (patrolPoints == null || patrolPoints.Length == 0)
            return;

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.2f)
        {
            patrolTimer -= Time.deltaTime;

            if (patrolTimer <= 0f)
                GoToNextPatrolPoint();
        }
    }

    void GoToNextPatrolPoint()
    {
        if (patrolPoints == null || patrolPoints.Length == 0)
            return;

        agent.SetDestination(patrolPoints[patrolIndex].position);

        patrolIndex++;
        if (patrolIndex >= patrolPoints.Length)
            patrolIndex = 0;

        patrolTimer = patrolWaitTime;
    }

    void TryOpenNearbyDoors()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, doorOpenRange, doorLayer);

        foreach (Collider hit in hits)
        {
            SmartDoor door = hit.GetComponentInParent<SmartDoor>();

            if (door != null && !door.isOpen)
                door.OpenAwayFrom(transform);
        }
    }

    void UpdateAnimations()
    {
        if (animator == null || agent == null)
            return;

        float targetSpeed = isChasing ? chaseSpeed : agent.velocity.magnitude;

        if (targetSpeed < 0.15f)
            targetSpeed = 0f;

        smoothAnimSpeed = Mathf.Lerp(smoothAnimSpeed, targetSpeed, Time.deltaTime * 8f);

        animator.SetFloat("Speed", smoothAnimSpeed);
    }

    void HandleMusic()
    {
        if (isChasing && !wasChasing)
        {
            if (MusicManager.Instance != null)
                MusicManager.Instance.StartChase();
        }
        else if (!isChasing && wasChasing)
        {
            if (MusicManager.Instance != null)
                MusicManager.Instance.EndChase();
        }

        wasChasing = isChasing;
    }
}