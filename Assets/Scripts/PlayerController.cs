using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    // Movement
    [Header("Movement")]
    [SerializeField] private float playerHeight = 2f;
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float airMultiplier = 0.4f;
    [SerializeField] private float movementMultiplier = 10f;
    [SerializeField] private float dashSpeed = 20f; // Dash speed
    [SerializeField] private Transform orientation;
    [SerializeField] private AudioSource jumpAudio;

    // Sprinting
    [Header("Sprinting")]
    [SerializeField] private float walkSpeed = 4f;
    [SerializeField] private float sprintSpeed = 6f;
    [SerializeField] private float acceleration = 50f;

    // Jumping
    [Header("Jumping")]
    [SerializeField] private float jumpForce = 14f;

    // Keybinds
    [Header("Keybinds")]
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode dashKey = KeyCode.E; // Dash key

    // Drag
    [Header("Drag")]
    [SerializeField] private float groundDrag = 6f;
    [SerializeField] private float airDrag = 2f;

    // Ground Detection
    [Header("Ground Detection")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.2f;
    [SerializeField] private LayerMask groundMask;

    // Camera
    [Header("Camera")]
    [SerializeField] private float sensitivityX = 150f;
    [SerializeField] private float sensitivityY = 150f;
    [SerializeField] private float sensitivityMultiplier = 0.01f;
    [SerializeField] private Transform cameraTransform;

    // Collectables
    [Header("Collectables")]
    [SerializeField] private TextMeshProUGUI pointsText;
    [SerializeField] private int pointsObjective = 7;

    [Header("Health")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private TextMeshProUGUI healthText;

    [Header("Dash")]
    [SerializeField] private float dashCooldown = 1f; // Cooldown for dashing
    private bool canDash = true; // Dash control variable

    [Header("Doors")]
    [SerializeField] private float doorInteractionRange = 2f;
    [SerializeField] private LayerMask doorLayerMask; // Define the layer for the doors

    [Header("Win")]
    [SerializeField] private TextMeshProUGUI winText;

    private float bittenCooldown = 1f; // Cooldown for bitten response
    private float lastBittenTime = 0f; // Time of the last bitten response
    private float horizontalMovement;
    private float verticalMovement;
    private bool isGrounded;
    private Vector3 moveDirection;
    private Vector3 slopeMoveDirection;
    private Rigidbody rb;
    private RaycastHit slopeHit;

    private Camera actualCam;
    private float mouseX;
    private float mouseY;
    private float xRotation;
    private float yRotation;

    private int keyCount = 0;
    private int pointsCounter;
    private int currentHealth;
    private float originalSpeed;

    //Animator
    [SerializeField] private Animator animator;
    private bool Attack = false;
    private bool isBitten = false;
    [SerializeField] private int damageAmount = 10; // damage amount
    private void Start()
    {
        InitializeComponents();
        InitializeCursor();
        UpdatePointsText();
        currentHealth = maxHealth;
        UpdateHealthText();
        // Initialize the original speed value with the starting speed
        originalSpeed = moveSpeed;

        //Animator
        animator = GetComponent<Animator>();
    }
    private void Update()
    {
        UpdateGroundedStatus();
        GetInputAxes();
        CalculateMoveDirection();

        // RotatePlayerTowardsMoveDirection();
        UpdateMouseRotation();

        UpdateDragBasedOnGroundedStatus();
        UpdateSprintingStatus();

        PerformJump();

        CalculateSlopeMoveDirection();
        RestartOnFalling();

        InteractWithDoor();

        // Dash
        if (Input.GetKeyDown(dashKey) && canDash)
        {
            PerformDash();
            StartCoroutine(DashCooldown());
        }

        if (Input.GetMouseButtonDown(1) && !Attack && animator != null)
        {
            Attack = true;
            animator.SetTrigger("Attack");
        }

        //if (Input.GetMouseButtonDown(1) && !Attack)
        //{
        //    Attack = true;
        //    animator.SetTrigger("Attack");
        //}
    }
    public void EndAttack()
    {
        Attack = false;
    }
    public void BeBitten()
    {
        if (!isBitten && Time.time - lastBittenTime >= bittenCooldown)
        {
            lastBittenTime = Time.time;
            isBitten = true;
            if (animator != null) animator.SetTrigger("Bitten");
            TakeDamage(damageAmount);
            isBitten = false;
        }
        isBitten = false;
    }
    private void FixedUpdate()
    {
        ApplyMovementForce();

        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        
        float normalizedHorizontalInput = Mathf.Clamp(horizontalInput, -1f, 1f);
        float normalizedVerticalInput = Mathf.Clamp(verticalInput, -1f, 1f);

        
        animator.SetFloat("X", normalizedHorizontalInput);
        animator.SetFloat("Y", normalizedVerticalInput);

        //Vector3 moveDirection = new Vector3(horizontalInput, 0f, verticalInput).normalized;
        //transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
    }
    private void InitializeComponents()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        actualCam = cameraTransform.GetComponentInChildren<Camera>();
    }
    private void InitializeCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
        private void UpdatePointsText()
    {
        pointsText.text = "x <color=green>" + pointsCounter.ToString() + " / " + pointsObjective.ToString() + "</color>";
    }
    private void UpdateGroundedStatus()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
    }
    private void GetInputAxes()
    {
        horizontalMovement = Input.GetAxisRaw("Horizontal");
        verticalMovement = Input.GetAxisRaw("Vertical");
    }
    private void CalculateMoveDirection()
    {
        moveDirection = orientation.forward * verticalMovement + orientation.right * horizontalMovement;
    }
    private void UpdateMouseRotation()
    {
        mouseX = Input.GetAxisRaw("Mouse X");
        mouseY = Input.GetAxisRaw("Mouse Y");

        yRotation += mouseX * sensitivityX * sensitivityMultiplier;
        xRotation -= mouseY * sensitivityY * sensitivityMultiplier;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cameraTransform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.transform.rotation = Quaternion.Euler(0, yRotation, 0);

        transform.rotation = Quaternion.Euler(0, yRotation, 0);
    }
    private void UpdateDragBasedOnGroundedStatus()
    {
        rb.drag = isGrounded ? groundDrag : airDrag;
    }
    private void UpdateSprintingStatus()
    {
        if (Input.GetKey(sprintKey) && isGrounded && !(horizontalMovement.Equals(0) && verticalMovement.Equals(0)))
        {
            moveSpeed = Mathf.Lerp(moveSpeed, sprintSpeed, acceleration * Time.deltaTime);
            actualCam.fieldOfView = Mathf.Lerp(actualCam.fieldOfView, 90f, 10f * Time.deltaTime);
        }
        else
        {
            moveSpeed = Mathf.Lerp(moveSpeed, walkSpeed, acceleration * Time.deltaTime);
            actualCam.fieldOfView = Mathf.Lerp(actualCam.fieldOfView, 80f, 10f * Time.deltaTime);
        }
    }
    private void PerformJump()
    {
        if (Input.GetKeyDown(jumpKey) && isGrounded)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
            jumpAudio.Play();

            animator.SetTrigger("Jump");
        }
    }
    private void PerformDash()
    {
        rb.AddForce(moveDirection.normalized * dashSpeed, ForceMode.Impulse);
    }
    private IEnumerator DashCooldown()
    {
        canDash = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
    private void CalculateSlopeMoveDirection()
    {
        slopeMoveDirection = Vector3.ProjectOnPlane(moveDirection, slopeHit.normal);
    }
    private void RestartOnFalling()
    {
        if (rb.velocity.y < -14f)
        {
            StartCoroutine(FallGroundRestart());
        }
    }
    private void ApplyMovementForce()
    {
        if (isGrounded && !OnSlope())
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * movementMultiplier, ForceMode.Acceleration);
            rb.useGravity = true;
        }
        else if (isGrounded && OnSlope())
        {
            rb.AddForce(slopeMoveDirection.normalized * moveSpeed * movementMultiplier, ForceMode.Acceleration);
            rb.useGravity = false;
        }
        else if (!isGrounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * movementMultiplier * airMultiplier, ForceMode.Acceleration);
            rb.useGravity = true;
        }
    }
    private IEnumerator FallGroundRestart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        yield return new WaitForSeconds(1);
    }
    public IEnumerator Restart(int seconds)
    {
        if (winText != null)
        {
            winText.gameObject.SetActive(true); // Enable the win text

            for (int i = 0; i < seconds; i++)
            {
                winText.text = $"<color=green>You win!</color> Restarting in {seconds - i} seconds...";
                yield return new WaitForSeconds(1);
            }

            winText.gameObject.SetActive(false); // Disable the win text
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void AddScore(int value)
    {
        pointsCounter += value;
        UpdatePointsText();
        if (pointsCounter >= pointsObjective)
        {
            StartCoroutine(Restart(5));
        }
    }
    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight / 2 + 0.5f))
        {
            if (slopeHit.normal != Vector3.up)
            {
                return true;
            }
        }
        return false;
    }
    public void SetCursorState(CursorLockMode lockState, bool visible)
    {
        Cursor.lockState = lockState;
        Cursor.visible = visible;
    }
    private void UpdateHealthText()
    {
        healthText.text = $"Health: <color=green>{currentHealth}</color>";
    }
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        UpdateHealthText();

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            animator.SetTrigger("Bitten");
        }
    }
    private void Die()
    {
        animator.SetTrigger("DieTrigger");

        rb.velocity = Vector3.zero;
        rb.isKinematic = true;
        enabled = false;

        StartCoroutine(Restart(1));
    }
    public void BoostSpeed(int speedBoost, float boostDuration)
    {
        moveSpeed += speedBoost;

        Invoke("ResetSpeed", boostDuration);
    }
    private void ResetSpeed()
    {
        moveSpeed = originalSpeed;
    }
    public void IncrementKeyCount()
    {
        keyCount++;
    }
    public int GetKeys()
    {
        return keyCount;
    }
    public void UseKeys(int numberOfKeysToUse)
    {
        keyCount -= numberOfKeysToUse;
        keyCount = Mathf.Max(keyCount, 0);
    }
    private void InteractWithDoor()
    {
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, doorInteractionRange, doorLayerMask))
        {
            Door door = hit.transform.GetComponent<Door>();
            if (door != null && keyCount > 0)
            {
                if (Input.GetKeyDown(KeyCode.F))
                {
                    door.OpenDoor();
                }
            }
        }
    }
}