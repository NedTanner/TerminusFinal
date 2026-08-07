using UnityEngine;
using UnityEngine.AI;

public class GruntEnemy : MonoBehaviour
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

    private enum State { Idle, Chase }
    private State currentState = State.Idle;

    private void Awake()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = stoppingDistance;
    }

    private void Update()
    {
        switch (currentState)
        {
            // check if the player is within sight range, if yes chase RAAHHHHHH
            case State.Idle:
                if (CanSeePlayer())
                    currentState = State.Chase;
                break;
            // RAAAAHHHHHHH chase player and lose interest if they get far enough away
            case State.Chase:
                float distance = Vector3.Distance(transform.position, player.position);

                if (distance > loseInterestRange)
                {
                    currentState = State.Idle;
                    agent.ResetPath();
                    break;
                }

                agent.SetDestination(player.position);
                break;
        }

        // drive animator with agent speed
        if (animator != null)
            animator.SetFloat("Speed", agent.velocity.magnitude);
    }

    private bool CanSeePlayer()
    {
        Vector3 eyeOffset = Vector3.up * 1f; // adjust to roughly chest/eye height for your models, without this the enemy was sometimes just looking at the floor
        Vector3 origin = transform.position + eyeOffset;
        Vector3 targetPoint = player.position + eyeOffset;

        Vector3 toPlayer = targetPoint - origin;
        float distance = toPlayer.magnitude;

        if (distance > detectionRange)
            return false;

        // gets the field of view for the sight cone
        float angle = Vector3.Angle(transform.forward, toPlayer);
        if (angle > fieldOfView * 0.5f)
            return false;

        // sends out a raycast to find player. if it hits a wall before player then sight is blocked if not chase player
        if (requireLineOfSight)
        {
            bool hitSomething = Physics.Raycast(origin, toPlayer.normalized, out RaycastHit hit, detectionRange, obstacleMask);
            if (hitSomething && hit.transform != player)
                return false;
        }

        return true;
    }

    // shows the detection/lose interest range in the scene view
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, loseInterestRange);
    }
}