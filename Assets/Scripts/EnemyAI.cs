using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    [Header("AI Settings")]
    [SerializeField] public Transform target;
    [SerializeField] public float chaseRange = 10f;
    [SerializeField] public float chargeSpeed = 15f;
    [SerializeField] public float chargeCooldown = 5f;

    [Header("Patrol")]
    //[SerializeField] public float patrolRadius = 5f;
    //[SerializeField] public float patrolSpeed = 3f;
    //private Vector3 startPosition;
    [SerializeField] public Transform pointA;
    [SerializeField] public Transform pointB;
    [SerializeField] public float patrolSpeed = 3f;

    [Header("Particle")]
    [SerializeField] public ParticleSystem chargeParticles;

    [Header("Exclamation")]
    [SerializeField] public Image exclamationMark;
    [SerializeField] public float detectionRange = 10f;

    [Header("Vanish")]
    [SerializeField] private AudioSource vanishAudio;

    private NavMeshAgent agent;
    private bool isCharging;
    private bool isPouncing;
    private float chargeTime;
    private Vector3 chargeDirection;
    private Transform currentPatrolPoint;
    private Rigidbody rb;

    private Animator animator;
    private PlayerController player;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = patrolSpeed;
        //startPosition = transform.position; // Store the starting position for patrol
        currentPatrolPoint = pointA;

        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        player = FindObjectOfType<PlayerController>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (target == null) return;

        float distanceToTarget = Vector3.Distance(target.position, transform.position);
        exclamationMark.enabled = (distanceToTarget <= detectionRange);

        if (!isCharging && !isPouncing && agent.isActiveAndEnabled)
        {
            if (distanceToTarget < chaseRange)
            {
                StartCoroutine(Pounce());
            }
            else
            {
                Patrol();

                // Calculate the direction in local space
                Vector3 localDirection = transform.InverseTransformDirection(agent.velocity);
                float forwardSpeed = localDirection.z;
                float rightSpeed = localDirection.x;

                // Update the animation parameters
                animator.SetFloat("X", rightSpeed);
                animator.SetFloat("Y", forwardSpeed);
            }
        }
    }
    private void Patrol()
    {
        //if (Vector3.Distance(transform.position, agent.destination) < 1f)
        //{
        //    SetRandomDestinationWithinRadius();
        //}
        agent.SetDestination(currentPatrolPoint.position);

        if (Vector3.Distance(transform.position, currentPatrolPoint.position) < 1f)
        {
            currentPatrolPoint = (currentPatrolPoint == pointA) ? pointB : pointA;
            agent.SetDestination(currentPatrolPoint.position);
        }
    }
    //private void SetRandomDestinationWithinRadius()
    //{
        // Calculate a random direction within the patrol radius
        //Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        //randomDirection += startPosition;

        //NavMeshHit hit;
        //NavMesh.SamplePosition(randomDirection, out hit, patrolRadius, 1);
        //Vector3 finalPosition = hit.position;

        //agent.SetDestination(finalPosition);
    //}

        private IEnumerator Pounce()
    {
        isCharging = true;
        agent.enabled = false;
        Vector3 dashDirection = (target.position - transform.position).normalized;
        float dashDuration = 0.2f;
        float elapsedTime = 0;
        while (elapsedTime < dashDuration)
        {
            transform.position += dashDirection * chargeSpeed * Time.deltaTime;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        player.BeBitten();
        yield return Vanish();
        isCharging = false;
    }
    private IEnumerator Vanish()
    {
        vanishAudio.Play();
        gameObject.SetActive(false);
        yield return null;
    }
}
