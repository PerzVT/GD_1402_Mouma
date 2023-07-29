using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// I would like to see the camera given its own class as it feels a bit off where it is and the rotation is unnatural.  I like how you have split things up into headers, as it makes it a lot more digestable.
/// The Inputs, however, should be handle by the Input Manager (https://docs.unity3d.com/2021.3/Documentation/Manual/class-InputManager.html) and should not be down to KeyCodes. You want this to be something you could map to a controller as well as having work with a keyboard and mouse
/// 
/// </summary>
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

    private int pointsCounter;
    private int currentHealth;
    private float originalSpeed;

    //Animator
    [SerializeField] private Animator animator;
    private bool Attack = false;


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

        // Dash
        PerformDash();

        if (Input.GetMouseButtonDown(1) && !Attack)
        {
            Attack = true;
            animator.SetTrigger("Attack");
        }
    }
    public void EndAttack()
    {
        Attack = false;
    }

    private void FixedUpdate()
    {
        ApplyMovementForce();

        // Update animator parameters
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        
        float normalizedHorizontalInput = Mathf.Clamp(horizontalInput, -1f, 1f);
        float normalizedVerticalInput = Mathf.Clamp(verticalInput, -1f, 1f);

        
        animator.SetFloat("X", normalizedHorizontalInput);
        animator.SetFloat("Y", normalizedVerticalInput);

        
        Vector3 moveDirection = new Vector3(horizontalInput, 0f, verticalInput).normalized;
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
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
        pointsText.text = $"Coins: <color=green>{pointsCounter} / {pointsObjective} </color>";
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

        // Limit camera rotation to prevent going upside down
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cameraTransform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.transform.rotation = Quaternion.Euler(0, yRotation, 0);

        // Rotate the player object based on the mouse input along the y-axis
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

            // Trigger jump animation
            animator.SetTrigger("Jump");
        }
    }

    private void PerformDash()
    {
        if (Input.GetKeyDown(dashKey))
        {
            rb.AddForce(moveDirection.normalized * dashSpeed, ForceMode.Impulse);
        }
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
        for (int i = 0; i < 3; i++)
        {
            pointsText.text = $"<color=green>You win!</color> Restarting in {seconds - i} seconds...";
            yield return new WaitForSeconds(1);
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
        // Check if we are on a slope by sending a raycast down
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
        // Decrease player's health by the damage amount
        currentHealth -= damage;

        // Update health text
        UpdateHealthText();

        // Handle player death if health reaches zero
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Play the death animation
        animator.SetTrigger("DieTrigger");

        // Disable the player's movement and other actions during the death animation
        rb.velocity = Vector3.zero;
        rb.isKinematic = true;
        enabled = false;

        // Implement player death behavior
        // For example, restart the level
        StartCoroutine(Restart(1));

    }

    

    public void BoostSpeed(int speedBoost, float boostDuration)
    {
        // Increase the player's speed
        moveSpeed += speedBoost;

        // After the boost duration, reset the player's speed
        Invoke("ResetSpeed", boostDuration);
    }

    private void ResetSpeed()
    {
        // Reset the player's speed to its original value
        moveSpeed = originalSpeed;
    }
}
