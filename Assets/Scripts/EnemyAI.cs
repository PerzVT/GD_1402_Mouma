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
    //[SerializeField] private float bitingRange = 2f;
    //private bool isBiting = false;
    private PlayerController player;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = patrolSpeed;
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

        if (!isCharging && !isPouncing)
        {
            if (agent.isActiveAndEnabled)
            {
                if (distanceToTarget < chaseRange)
                {
                    StartCoroutine(PounceAndVanish());
                }
                else
                {
                    Patrol();
                }
            }
        }

        //if (distanceToTarget <= bitingRange)
        //{
        //    isBiting = true;
        //}
        //else
        //{
        //    isBiting = false;
        //}
    }

    private void Patrol()
    {
        agent.SetDestination(currentPatrolPoint.position);

        if (Vector3.Distance(transform.position, currentPatrolPoint.position) < 1f)
        {
            currentPatrolPoint = (currentPatrolPoint == pointA) ? pointB : pointA;
        }
    }

    private IEnumerator PounceAndVanish()
    {
        isCharging = true;
        isPouncing = true;
        agent.enabled = false;

        Vector3 pounceDirection = (target.position - transform.position).normalized;

        float pounceDuration = 0.5f;
        float elapsedTime = 0;

        while (elapsedTime < pounceDuration)
        {
            transform.position += pounceDirection * chargeSpeed * Time.deltaTime;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        isPouncing = false;
        player.BeBitten();
        Vanish();
    }

    private void Vanish()
    {
        vanishAudio.Play();
        gameObject.SetActive(false);  // This will deactivate the cat object after it vanishes
    }
}
