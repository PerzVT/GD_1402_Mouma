using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    // AI settings
    [Header("AI Settings")]
    [SerializeField] public Transform target;
    [SerializeField] public float chaseRange = 10f;
    [SerializeField] public float chargeSpeed = 15f;
    [SerializeField] public float chargeCooldown = 5f;

    // Patrol points
    [Header("Patrol")]
    [SerializeField] public Transform pointA;
    [SerializeField] public Transform pointB;
    [SerializeField] public float patrolSpeed = 3f;

    // Particle system
    [Header("Particle")]
    [SerializeField] public ParticleSystem chargeParticles;

    // Exclamation mark for detection
    [Header("Exclamation")]
    [SerializeField] public Image exclamationMark;
    [SerializeField] public float detectionRange = 10f;

    private NavMeshAgent agent;
    private bool isCharging;
    private float chargeTime;
    private Vector3 chargeDirection;
    private Transform currentPatrolPoint;
    private Rigidbody rb;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = patrolSpeed;
        currentPatrolPoint = pointA;

        // Initialize Rigidbody and freeze rotation
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        
        // Make the particle system brighter
        var chargeParticleMain = chargeParticles.main;
        chargeParticleMain.startColor = new Color(1, 1, 1, 1);
    }

    private void Update()
    {
        if (target == null) return;

        float distanceToTarget = Vector3.Distance(target.position, transform.position);

        // Show or hide exclamation mark based on distance
        exclamationMark.enabled = (distanceToTarget <= detectionRange);

        if (!isCharging)
        {
            if (agent.isActiveAndEnabled)
            {
                if (distanceToTarget < chaseRange)
                {
                    agent.SetDestination(target.position);
                    StartCoroutine(ChargeUp());
                }
                else
                {
                    Patrol();
                }
            }
        }
        else
        {
            chargeTime -= Time.deltaTime;

            if (chargeTime <= 0)
            {
                isCharging = false;
                agent.enabled = true;

                // Disable gravity when not charging
                rb.useGravity = false;
            }
        }
    }

    // Patrol between pointA and pointB
    private void Patrol()
    {
        agent.SetDestination(currentPatrolPoint.position);

        if (Vector3.Distance(transform.position, currentPatrolPoint.position) < 1f)
        {
            if (currentPatrolPoint == pointA)
            {
                currentPatrolPoint = pointB;
            }
            else
            {
                currentPatrolPoint = pointA;
            }
        }
    }

    // Coroutine for charging behavior
    private IEnumerator ChargeUp()
    {
        chargeParticles.Play();
        yield return new WaitForSeconds(2f);
        chargeParticles.Stop();

        // Turn towards player
        transform.LookAt(target.position);
        
        // Pause for a second before charging
        yield return new WaitForSeconds(1f);

        isCharging = true;
        agent.enabled = false;
        chargeDirection = (target.position - transform.position).normalized;
        chargeTime = chargeCooldown;

        // Enable gravity when charging
        rb.useGravity = true;
    }
}
