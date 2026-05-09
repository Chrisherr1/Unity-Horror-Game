using UnityEngine;
using UnityEngine.AI;

public class BlindEnemyAI : MonoBehaviour
{
    [Header("Player")]
    public Transform player;
    private FirstPersonAudio playerAudio;

    private NavMeshAgent agent;
    private Animator animator;

    [Header("Roaming")]
    public float roamRadius = 10f;
    public float minRoamDistance = 3f;
    public float roamWaitTime = 2f;

    [Header("Hearing")]
    public float hearingRange = 20f;

    [Header("Chase")]
    public float roamSpeed = 2f;
    public float chaseSpeed = 4.5f;
    public float giveUpTime = 2f;

    [Header("Attack")]
    public float attackDistance = 1.6f;
    public float attackCooldown = 2f;
    public float attackGraceTime = 0.1f;

    private float roamTimer;
    private float loseTimer;
    private float attackTimer;
    private float attackGraceTimer;
    private float smoothAnimSpeed;

    private bool isChasing = false;
    private bool wasChasing = false;

    private Vector3 roamCenter;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();

        if (player != null)
            playerAudio = player.GetComponentInChildren<FirstPersonAudio>();

        roamCenter = transform.position;
        roamTimer = 0f;

        agent.speed = roamSpeed;
        agent.stoppingDistance = 0.1f;
        agent.isStopped = false;

        PickRoamPoint();
    }

    void Update()
    {
        if (agent == null)
            return;

        HandleHearing();
        HandleMusic();

        if (isChasing)
            ChasePlayer();
        else
            Roam();

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
                attackTimer = 0f;
                attackGraceTimer = 0f;

                agent.isStopped = false;

                if (animator != null)
                    animator.ResetTrigger("Attack");
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

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        attackTimer -= Time.deltaTime;

        if (distanceToPlayer <= attackDistance)
        {
            attackGraceTimer = attackGraceTime;
            agent.isStopped = true;

            if (attackTimer <= 0f)
            {
                if (animator != null)
                    animator.SetTrigger("Attack");

                attackTimer = attackCooldown;
            }

            return;
        }

        attackGraceTimer -= Time.deltaTime;

        if (attackGraceTimer <= 0f)
        {
            if (animator != null)
                animator.ResetTrigger("Attack");

            agent.isStopped = false;
        }

        agent.speed = chaseSpeed;
        agent.SetDestination(player.position);
    }

    void Roam()
    {
        agent.isStopped = false;
        agent.speed = roamSpeed;

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.2f)
        {
            roamTimer -= Time.deltaTime;

            if (roamTimer <= 0f)
                PickRoamPoint();
        }
    }

    void PickRoamPoint()
    {
        for (int i = 0; i < 20; i++)
        {
            Vector2 circle = Random.insideUnitCircle * roamRadius;
            Vector3 candidate = roamCenter + new Vector3(circle.x, 0f, circle.y);

            if (Vector3.Distance(transform.position, candidate) < minRoamDistance)
                continue;

            NavMeshHit hit;

            if (NavMesh.SamplePosition(candidate, out hit, 2f, NavMesh.AllAreas))
            {
                NavMeshPath path = new NavMeshPath();

                if (agent.CalculatePath(hit.position, path) &&
                    path.status == NavMeshPathStatus.PathComplete)
                {
                    agent.SetDestination(hit.position);
                    roamTimer = roamWaitTime;
                    return;
                }
            }
        }

        roamTimer = 1f;
    }

    void UpdateAnimations()
    {
        if (animator == null || agent == null)
            return;

        float targetSpeed;

        if (isChasing && !agent.isStopped)
            targetSpeed = chaseSpeed;
        else
            targetSpeed = agent.velocity.magnitude;

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