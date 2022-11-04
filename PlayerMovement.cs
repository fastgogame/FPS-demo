using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private Transform orientation;

    private Rigidbody rb;
    private AudioSource jumpSound;
    private float x, z;
    private bool jumpButton, crouchButton;

    [SerializeField] private float groundDistance = 0.3f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] Transform groundCheck;
    private bool isGrounded;

    [SerializeField] private float accel = 200f;
    [SerializeField] private float maxSpeed = 20f;
    [SerializeField] private float groundFriction = 6f;
    [SerializeField] private float jumpForce = 10f;
    private float velocityDot;
    private float lastJumpPress = -1f;
    private float jumpPressDuration = 0.1f;

    private float playerScale;
    private float crouchScale;
    private void Awake()
    {
        rb = GetComponentInChildren<Rigidbody>();
        playerScale = rb.transform.localScale.y;
        crouchScale = playerScale / 2;
        jumpSound = GetComponentInChildren<AudioSource>();
    }
    private void Update()
    {
        if (!isLocalPlayer) return;

        InputControls();
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundLayer);

        if (jumpButton)
        {
            lastJumpPress = Time.time;
            if (isGrounded)
                jumpSound.Play();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene("SampleScene");
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Application.Quit();
        }
    }

    private void FixedUpdate()
    {
        if (!isLocalPlayer) return;

        Vector2 input = new Vector2(x, z);

        Vector3 playerVelocity = rb.velocity;
        playerVelocity = CalculateFriction(playerVelocity);
        playerVelocity += CalculateMovement(input.normalized, playerVelocity);

        if (velocityDot == 0f)
        {
            playerVelocity *= (1 - Time.fixedDeltaTime / 2f);
        }

        rb.velocity = playerVelocity;
    }


	private Vector3 CalculateFriction(Vector3 currentVelocity)
    {
        float speed = currentVelocity.magnitude;

        if (!isGrounded || jumpButton || speed == 0f)
            return currentVelocity;

        float drop = speed * groundFriction * Time.deltaTime;
        return currentVelocity * (Mathf.Max(speed - drop, 0f) / speed);
    }

    private Vector3 CalculateMovement(Vector2 input, Vector3 velocity)
    {
        float curMaxSpeed = maxSpeed;
        float maxAirSpeed = maxSpeed / 4f;

        if (!isGrounded)
            curMaxSpeed = maxAirSpeed;

        //Get rotation input and make it a vector
        Vector3 camRotation = new Vector3(0f, orientation.transform.rotation.eulerAngles.y, 0f);
        Vector3 inputVelocity = Quaternion.Euler(camRotation) *
                                new Vector3(input.x * accel, 0f, input.y * accel);

        //Ignore vertical component of rotated input
        Vector3 alignedInputVelocity = new Vector3(inputVelocity.x, 0f, inputVelocity.z) * Time.deltaTime;

        //Get current velocity
        Vector3 currentVelocity = new Vector3(velocity.x, 0f, velocity.z);

        //How close the current speed to max velocity is (1 = not moving, 0 = at/over max speed)
        float max = Mathf.Max(0f, 1 - (currentVelocity.magnitude / curMaxSpeed));
        
        //How perpendicular the input to the current velocity is (0 = 90°)
        velocityDot = Vector3.Dot(currentVelocity, alignedInputVelocity);

        //Scale the input to the max speed
        Vector3 modifiedVelocity = alignedInputVelocity * max;

        //The more perpendicular the input is, the more the input velocity will be applied
        Vector3 correctVelocity = Vector3.Lerp(alignedInputVelocity, modifiedVelocity, velocityDot);

        //Apply jump
        correctVelocity += GetJumpVelocity(velocity.y);

        //Return
        return correctVelocity;
    }

	private Vector3 GetJumpVelocity(float yVelocity)
    {
        Vector3 jumpVelocity = Vector3.zero;

        if (Time.time < lastJumpPress + jumpPressDuration && yVelocity < jumpForce && isGrounded)
        {
            lastJumpPress = -1f;
            jumpVelocity = new Vector3(0f, jumpForce - yVelocity, 0f);
        }

        return jumpVelocity;
    }

    private void InputControls()
    {
        x = Input.GetAxisRaw("Horizontal");
        z = Input.GetAxisRaw("Vertical");
        jumpButton = Input.GetButton("Jump") || Input.GetAxis("Mouse ScrollWheel") < 0f;
        crouchButton = Input.GetKey(KeyCode.LeftControl);
    }
}