using Interfaces;
using UnityEngine;
using UnityEngine.AI;

public class GruntEnemy : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator animator;

    [Header("Range")]
    [SerializeField] private float aggroRange = 20f; // Distance from startingPosition enemy can travel before resetting to idle behaviour and returning home
    private Vector3 startingPosition;

    [Header("Detection")]
    [SerializeField] private float detectionRange = 8f;
    [SerializeField] private float fieldOfView = 90f; // set to 360 to ignore facing angle
    [SerializeField] private LayerMask obstacleMask; // walls/objects that block sight
    [SerializeField] private bool requireLineOfSight = true;

    [Header("Chase")]
    [SerializeField] private float stoppingDistance = 1.5f;
    [SerializeField] private float loseInterestRange = 12f; // gives up if player runs far enough away
    private float baseSpeed; // base running speed
    private float baseAngularSpeed; // base turning speed

    [Header("Attack")]
    [SerializeField] private float attackRange = 2f;       // distance at which it stops and swings
    [SerializeField] private float attackCooldown = 1.5f;  // seconds between attacks
    [SerializeField] private float attackDamage = 2;

    private PlayerHealth playerHealth;

    private enum State { Idle, Chase, Attack }
    private State currentState = State.Idle;

    private float attackTimer = 0f;

    private void Awake()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();

        player = GameObject.FindWithTag("Player").transform; // Sets player target automatically, don't have to manually set when you drag in prefab now

        agent.stoppingDistance = stoppingDistance;
        baseSpeed = agent.speed;
        baseAngularSpeed = agent.angularSpeed;

        startingPosition = transform.position; // Sets enemy territory, they return here after losing interest in player
    }

    private void Update()
    {
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }
        // Stops enemy movement and rotation, only when attacking
        agent.speed = attackTimer > 0f ? 0f : baseSpeed;
        agent.angularSpeed = attackTimer > 0f ? 0f : baseAngularSpeed;

        float distance = player != null ? Vector3.Distance(transform.position, player.position) : Mathf.Infinity;
        bool outOfRange = Vector3.Distance(transform.position, startingPosition) > aggroRange;

        switch (currentState)
        {
            // check if the player is within sight range, if yes chase RAAHHHHHH
            case State.Idle:
                if (CanSeePlayer())
                    currentState = State.Chase;
                break;
            // RAAAAHHHHHHH chase player and lose interest if they get far enough away
            case State.Chase:

                if (outOfRange)
                {
                    currentState = State.Idle;
                    agent.SetDestination(startingPosition);
                    break;
                }

                if (distance > loseInterestRange)
                {
                    currentState = State.Idle;
                    agent.SetDestination(startingPosition);
                    break;
                }

                if (distance <= attackRange)
                {
                    currentState = State.Attack;
                    agent.ResetPath(); // stop moving to attack
                    break;
                }

                agent.SetDestination(player.position);
                break;

            case State.Attack:

                if (distance > loseInterestRange)
                {
                    currentState = State.Idle;
                    break;
                }

                // player backed out of range, go back to chasing
                if (distance > attackRange)
                {
                    currentState = State.Chase;
                    break;
                }

                if (attackTimer <= 0f)
                {
                    attackTimer = attackCooldown;
                    PerformAttack();
                }
                break;
        }

        if (animator != null)
            animator.SetFloat("Speed", agent.velocity.magnitude);
    }

    private void FacePlayer()
    {
        Vector3 dir = player.position - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.0001f) return;

        Quaternion targetRot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 10f);
    }

    private void PerformAttack()
    {
        FacePlayer();
        if (animator != null)
            animator.SetTrigger("Attack");
        PlayerHit();
    }

    private void PlayerHit()
    {
      
            playerHealth = player.GetComponent<PlayerHealth>();
            playerHealth.TakeDamage(attackDamage);
        
    }


    private bool CanSeePlayer()
    {
        Vector3 eyeOffset = Vector3.up * 1f;
        Vector3 origin = transform.position + eyeOffset;
        Vector3 targetPoint = player.position + eyeOffset;
        Vector3 toPlayer = targetPoint - origin;
        float distance = toPlayer.magnitude;

        if (distance > detectionRange)
            return false;

        float angle = Vector3.Angle(transform.forward, toPlayer);
        if (angle > fieldOfView * 0.5f)
            return false;

        if (requireLineOfSight)
        {
            bool hitSomething = Physics.Raycast(origin, toPlayer.normalized, out RaycastHit hit, detectionRange, obstacleMask);
            if (hitSomething && hit.transform != player)
                return false;
        }

        return true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, loseInterestRange);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}