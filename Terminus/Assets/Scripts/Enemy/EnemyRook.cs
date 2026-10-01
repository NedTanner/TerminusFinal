using System.Collections;
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
    [SerializeField] private float stun = 5.0f;
    [SerializeField] private LineRenderer dashTelegraph;

    [Header("Dash Collision")]
    [SerializeField] private LayerMask enemyMask;  // layer(s) other enemies live on
    [SerializeField] private float dashCollisionRadius = 0.6f;
    [SerializeField] private float knockbackForce = 8f;

    private enum State { Idle, Chase, Attack, Windup, Stunned }
    private State currentState = State.Idle;

    private float attackTimer = 0f;
    private Vector3 dashTarget;
    private bool isAttacking = false;

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
                // Locks on dash the player via raycast so the dash doesn't
                dashTarget = CalculateDashTarget();
                ShowDashTelegraph(dashTarget);
                currentState = State.Attack;
                break;

            case State.Attack:
                if (!isAttacking)
                {
                    isAttacking = true;
                    Invoke(nameof(BeginDash), windupTime);
                }
                break;

            case State.Stunned:
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

    // Casts toward the player once and returns the point the rook should dash to:
    // either where the raycast hits an obstacle, or a point along that direction
    private Vector3 CalculateDashTarget()
    {
        Vector3 eyeOffset = Vector3.up * 1f;
        Vector3 origin = transform.position + eyeOffset;
        Vector3 dir = (player.position - transform.position);
        dir.y = 0f;
        dir = dir.sqrMagnitude > 0.0001f ? dir.normalized : transform.forward;

        float castDistance = Vector3.Distance(transform.position, player.position);

        if (Physics.Raycast(origin, dir, out RaycastHit hit, castDistance, obstacleMask))
        {
            // Stop short of whatever it hit, don't dash into a wall
            return hit.point - dir * 0.3f;
        }

        return transform.position + dir * castDistance;
    }

    private void BeginDash()
    {
        HideDashTelegraph();
        StartCoroutine(DashRoutine());
    }

    private IEnumerator DashRoutine()
    {
        FacePlayer();
        PerformAttack();

        // Hand movement over from the NavMeshAgent to a manual dash for the duration.
        agent.ResetPath();
        agent.enabled = false;

        float remaining = Vector3.Distance(transform.position, dashTarget);

        while (remaining > 0.05f)
        {
            Vector3 nextPos = Vector3.MoveTowards(transform.position, dashTarget, dashSpeed * Time.deltaTime);

            HandleDashCollisions(nextPos);

            transform.position = nextPos;
            remaining = Vector3.Distance(transform.position, dashTarget);
            yield return null;
        }

        agent.enabled = true;
        currentState = State.Stunned;

        if (animator != null)
            animator.SetBool("Stunned", true);

        float stunTime = stun;
        yield return new WaitForSeconds(stunTime);

        if (animator != null)
            animator.SetBool("Stunned", false);

        isAttacking = false;
        currentState = State.Idle;

    }

    // Checks for other enemies in the rook's path and shoves them aside so it
    // doesn't just clip through them mid-dash.
    private void HandleDashCollisions(Vector3 nextPos)
    {
        //code for knockback collisions
    }

    //shows the dash telegraph
    private void ShowDashTelegraph(Vector3 target)
    {
        if (dashTelegraph == null) return;

        dashTelegraph.enabled = true;
        dashTelegraph.positionCount = 2;
        dashTelegraph.SetPosition(0, transform.position);
        dashTelegraph.SetPosition(1, target);
    }

    // hides the telegraph once the dash begins
    private void HideDashTelegraph()
    {
        if (dashTelegraph == null) return;
        dashTelegraph.enabled = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, loseInterestRange);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        if (Application.isPlaying && currentState == State.Attack)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, dashTarget);
            Gizmos.DrawWireSphere(dashTarget, 0.3f);
        }
    }
}