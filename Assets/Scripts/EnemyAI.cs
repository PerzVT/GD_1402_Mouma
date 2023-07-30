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

    //Animator
    [SerializeField] private Animator animator;
    [SerializeField] private float bitingRange = 2f;
    private bool isBiting = false;
    private PlayerController player;


    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = patrolSpeed;
        currentPatrolPoint = pointA;

        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        
        var chargeParticleMain = chargeParticles.main;
        chargeParticleMain.startColor = new Color(1, 1, 1, 1);

        //Animator
        animator = GetComponent<Animator>();
        player = FindObjectOfType<PlayerController>();

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

                    // Calculate the normalized direction vector to the current patrol point
                    Vector3 patrolDirection = (currentPatrolPoint.position - transform.position).normalized;
                    // Set the animator parameters based on the patrol direction
                    animator.SetFloat("X", patrolDirection.x);
                    animator.SetFloat("Y", patrolDirection.z);
                }
            }
        }
        else
        {
            chargeTime -= Time.deltaTime;
            // Move the enemy during the charge
            rb.velocity = chargeDirection * chargeSpeed;

            // Set animator parameters based on the charging direction
            animator.SetFloat("X", chargeDirection.x);
            animator.SetFloat("Y", chargeDirection.z);

            if (chargeTime <= 0)
            {
                isCharging = false;
                agent.enabled = true;
                rb.velocity = Vector3.zero;  // Reset velocity when not charging
            }
        }

        // Check if colliding with the player and trigger biting animation
        if (distanceToTarget <= bitingRange)
        {
            isBiting = true;
        }
        else
        {
            isBiting = false;
        }

        // Set the "IsBiting" parameter in the animator to trigger the biting animation
        animator.SetBool("IsBiting", isBiting);
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

        // If the attack is successful and the player is bitten:
        player.BeBitten();

        isCharging = true;
        agent.enabled = false;
        chargeDirection = (target.position - transform.position).normalized;
        chargeTime = chargeCooldown;
    }
}
