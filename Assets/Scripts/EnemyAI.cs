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
    [SerializeField] public Transform pointA;
    [SerializeField] public Transform pointB;
    [SerializeField] public float patrolSpeed = 3f;

    [Header("Particle")]
    [SerializeField] public ParticleSystem chargeParticles;

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

        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        
        var chargeParticleMain = chargeParticles.main;
        chargeParticleMain.startColor = new Color(1, 1, 1, 1);
    }

    private void Update()
    {
        if (target == null) return;

        float distanceToTarget = Vector3.Distance(target.position, transform.position);
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
            // Move the enemy during the charge
            rb.velocity = chargeDirection * chargeSpeed;

            if (chargeTime <= 0)
            {
                isCharging = false;
                agent.enabled = true;
                rb.velocity = Vector3.zero;  // Reset velocity when not charging
            }
        }
    }

    private void Patrol()
    {
        agent.SetDestination(currentPatrolPoint.position);

        if (Vector3.Distance(transform.position, currentPatrolPoint.position) < 1f)
        {
            currentPatrolPoint = (currentPatrolPoint == pointA) ? pointB : pointA;
        }
    }

    private IEnumerator ChargeUp()
    {
        chargeParticles.Play();
        yield return new WaitForSeconds(2f);
        chargeParticles.Stop();

        transform.LookAt(target.position);
        yield return new WaitForSeconds(1f);

        isCharging = true;
        agent.enabled = false;
        chargeDirection = (target.position - transform.position).normalized;
        chargeTime = chargeCooldown;
    }
}
