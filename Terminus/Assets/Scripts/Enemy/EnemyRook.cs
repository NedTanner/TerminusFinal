using Interfaces;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyRook : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator animator;

    [Header("Detection")]
    [SerializeField] private float detectionRange = 8f;
    [SerializeField] private float fieldOfView = 90f; // set to 360 to ignore facing angle
    [SerializeField] private LayerMask obstacleMask; // walls/objects that block sight
    [SerializeField] private bool requireLineOfSight = true;

    [Header("Chase")]
    [SerializeField] private float stoppingDistance = 1.5f;
    [SerializeField] private float loseInterestRange = 12f; // gives up if player runs far enough away

    [Header("Attack")]
    [SerializeField] private float attackRange = 2f;       // distance at which it stops and swings
    [SerializeField] private float attackCooldown = 1.5f;  // seconds between attacks
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float attackWindup = 0.4f;    // delay before damage lands, sync with anim
    [SerializeField] private float windupTime = 1.5f;
    [SerializeField] private float dashSpeed = 5.0f;

    private enum State { Idle, Chase, Attack, Windup }
    private State currentState = State.Idle;

    private float attackTimer = 0f;

    private void Awake()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = stoppingDistance;
    }

    private void Update()
    {
        float distance = player != null ? Vector3.Distance(transform.position, player.position) : Mathf.Infinity;

        switch (currentState)
        {
            // check if the player is within sight range, if yes chase RAAHHHHHH
            case State.Idle:
                if (CanSeePlayer())
                    currentState = State.Windup;
                break;

            // RAAAAHHHHHHH chase player and lose interest if they get far enough away
            case State.Chase:
                if (distance > loseInterestRange)
                {
                    currentState = State.Idle;
                    agent.ResetPath();
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

            case State.Windup:
                animator.SetTrigger("Windup");
                currentState = State.Attack;
                break;

            case State.Attack:
                Invoke("RookAttack", windupTime);
                currentState = State.Idle;
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
        if (animator != null)
            animator.SetTrigger("Attack");
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

    void RookAttack()
    {
        FacePlayer();
        PerformAttack();
        transform.position = Vector3.MoveTowards(transform.position, player.position, dashSpeed * Time.deltaTime);
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