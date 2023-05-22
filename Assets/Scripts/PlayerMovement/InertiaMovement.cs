using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif
using StarterAssets;

[RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
	[RequireComponent(typeof(PlayerInput))]
#endif

public class InertiaMovement : MonoBehaviour
{

    // 🏃‍♂️ Run
    [Space(10)]
    [Header("Running")]
    [Tooltip("Initial movement speed (in m/s).")]
    public float InitialRunningSpeed = 7.0f;
    [Tooltip("Maximum speed obtainable by running (in m/s). Will stack with boosts.")]
    public float MaxRunningSpeed = 10.0f;
    [Tooltip("How fast will the player reach max speed (in m/s).")]
    public float RunningAcceleration = 2.0f;

    // 👴 Gel
    // Isto talvez seja melhor ficar no GameManager, mas para testes fica aqui por enquanto
    [Space(10)]
    [Header("Gel")]
    [Tooltip("How much extra speed the gel gives to the player (in m/s).")]
    public float GelExtraSpeed = 5.0f;
    [Tooltip("How long does the gel powerup lasts (in seconds).")]
    public float GelDuration = 10.0f;
    
    // 🦘 Jump
    [Space(10)]
    [Header("Jump")]
    [Tooltip("How much will a jump increase the player's speed.")]
    public float JumpBoost = 1.1f;
    [Tooltip("The maximum boost obtainable by jumps.")]
    public float MaxJumpBoost = 4.0f;

    // 🦆 Crouch
    [Space(10)]
    [Header("Crouch")]
    [Tooltip("Crouch speed of the character in m/s. Overrides all other speeds.")]
	public float CrouchSpeed = 4.0f;
    [Tooltip("The height of the player when crouched, in meters.")]
	public float CrouchHeight = 1.0f;

    // 🎿 Slide
    [Space(10)]
    [Header("Slide")]
    [HideInInspector] public bool isSliding = false;
    [Tooltip("How much will a slide increase the player's speed.")]
    public float SlideBoost = 1.1f;
    [Tooltip("The maximum boost obtainable by slides.")]
    public float MaxSlideBoost = 4.0f;
    [Tooltip("How much will the ramp angle affect the acceleration.")]
    public float SlideAngleMultiplier = 1.0f;
    [Tooltip("How much will the player slow down when sliding in flat surfaces.")]
    public float SlideDecayMultiplier = 1.0f;
    [Tooltip("The height of the player when sliding, in meters.")]
	public float SlidingHeight = 0.7f;

    [Space(10)]
	[Header("Jump")]
	[Tooltip("The height the player can jump")]
	public float JumpHeight = 1.2f;
	[Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
	public float Gravity = -15.0f;
	[Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
	public float JumpTimeout = 0.1f;
	[Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
	public float FallTimeout = 0.15f;

    // 🕺 Wall Jump
    [Space(10)]
    [Header("Wall Jump")]
    [Tooltip("How much will a wall jump increase the player's speed.")]
    public float WallJumpBoost = 1.1f;
    [Tooltip("The maximum boost obtainable by wall jumps.")]
    public float MaxWallJumpBoost = 4.0f;
    private Vector3 WallJumpDirection = Vector3.zero;

    // 🏄‍♂️ Wall Run
    [Space(10)]
    [Header("Wall Run")]
    [Tooltip("How much will a wall run increase the player's speed.")]
    public float WallRunBoost = 1.1f;
    [Tooltip("The maximum boost obtainable by wall runs.")]
    public float MaxWallRunBoost = 4.0f;
    [Tooltip("How long will the player be able to stick to a wall (in seconds).")]
    public float WallRunDecayMultiplier = 3.0f;

    [Space(10)]
    [Header("Player isGrounded")]
    [Tooltip("If the character is isGrounded or not. Not part of the CharacterController built in isGrounded check")]
    public bool isGrounded = true;
    [Tooltip("Useful for rough ground")]
    public float isGroundedOffset = -0.14f;
    [Tooltip("The radius of the isGrounded check. Should match the radius of the CharacterController")]
    public float isGroundedRadius = 0.22f;
    [Tooltip("What layers the character uses as ground")]
    public LayerMask GroundLayers;

    [ContextMenu("Calculate Max Speed")]
    void CalculateMaxSpeed() {
        float max = MaxRunningSpeed + MaxJumpBoost + MaxSlideBoost + MaxWallJumpBoost + MaxWallRunBoost;
        Debug.Log("Theoretical Max Speed: " + max + "m/s");
    }

    // 🚀 inertia movement speed
    [HideInInspector] public float currentTotalSpeed = 0.0f;
    [HideInInspector] public float currentRunningSpeed = 0.0f;
    [HideInInspector] public float currentJumpBoost = 0.0f;
    [HideInInspector] public float currentSlideBoost = 0.0f;
    [HideInInspector] public float currentWallJumpBoost = 0.0f;
    [HideInInspector] public float currentWallRunBoost = 0.0f;

    // player
	private float _speed;
	private float _rotationVelocity;
	private float _verticalVelocity;
	private float _terminalVelocity = 53.0f;

    // crouch
    [HideInInspector] public bool isCrouched = false;
	private float standingHeight;
	private bool canUncrouch;

    //jump
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private float playerSpeed = 2.0f;
    private float jumpHeight = 1.0f;
    private float gravityValue = -9.81f;

    // components
    #if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
		private PlayerInput _playerInput;
    #endif
    private CharacterController _controller;
	private StarterAssetsInputs _input;
    private GameObject cameraPosition;
    private GameObject orientation;

    void Start()
    {
        _controller = GetComponent<CharacterController>();
        _input = GetComponent<StarterAssetsInputs>();
        _playerInput = GetComponent<PlayerInput>();
        
        cameraPosition = transform.Find("CameraPosition").gameObject;

        standingHeight = _controller.height;
    }


    void Update() {
        Crouch();
        Jump();
        Move();
        WallJump();
    }

    void FixedUpdate() {
        UncrouchCheck();
    }

    void Move() {
        // direção do movimento
        Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y);
        inputDirection = transform.right * _input.move.x + transform.forward * _input.move.y;


        if (isCrouched) {
            currentTotalSpeed = CrouchSpeed;
            ResetSpeedsAndBoosts();
        } else {
            currentTotalSpeed = Run() + Slide();
        }



        _controller.Move(inputDirection.normalized * currentTotalSpeed * Time.deltaTime);
        
    }

    void ResetSpeedsAndBoosts() {
        currentRunningSpeed = 0.0f;
        currentSlideBoost = 0.0f;
    }

    float Run() {
        
        // se ainda nao estiver a correr, começa na velocidade minima
        if (currentRunningSpeed < InitialRunningSpeed) {
            currentRunningSpeed = InitialRunningSpeed;
        } 
        // se ainda nao estiver a correr à velocidade máxima, acelera
        if (currentRunningSpeed >= InitialRunningSpeed && currentRunningSpeed < MaxRunningSpeed) {
            currentRunningSpeed += RunningAcceleration;
        }
        // se já estiver na velocidade maxima (ou acima), dá clamp
        if (currentRunningSpeed >= MaxRunningSpeed) {
            currentRunningSpeed = MaxRunningSpeed;
        }

        // se parar, muda a current running speed outra vez pra 0
        if (_input.move == Vector2.zero) {
            currentRunningSpeed = 0.0f;
        }

        return currentRunningSpeed;
    }

    private void Jump() {

        groundedPlayer = _controller.isGrounded;

        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        Vector3 move = new Vector3(_input.move.x, 0, _input.move.y);
        _controller.Move(move * Time.deltaTime * playerSpeed);

        
        if (_input.jump && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -2.0f * gravityValue);
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        _controller.Move(playerVelocity * Time.deltaTime);

    }

    float Slide() {
        // condições para fazer um slide
        if (isGrounded && _input.crouch && currentTotalSpeed > CrouchSpeed) {

            isSliding = true;
            
            // muda a velocidade
            if (currentSlideBoost == 0.0f) {
                currentSlideBoost = SlideBoost;
            }
            if (currentSlideBoost >= SlideBoost && currentSlideBoost < MaxSlideBoost) {
                currentSlideBoost += 0.01f;
            }
            if (currentSlideBoost >= MaxSlideBoost) {
                currentSlideBoost = MaxSlideBoost;
            }

            // muda a posição da câmara
            NewCameraPosition(SlidingHeight);
        }

        // se parar de fazer crouch e ainda estiver numa speed acima de crouch speed, sobe a câmara
        if (!_input.crouch &&  currentTotalSpeed > CrouchSpeed) {
            isSliding = false;
            NewCameraPosition(standingHeight);
        }

        // se parar, faz reset ao slide booost
        if (currentTotalSpeed <= CrouchSpeed || _input.move == Vector2.zero) {
            isSliding = false;
            currentSlideBoost = 0.0f;
        }

        return currentSlideBoost;
    }

    private void Crouch() {

        if (
            // se está no chão, premiu crouch, e está parado
            (isGrounded && _input.crouch && currentTotalSpeed <= CrouchSpeed) || 
            // Se está no chão, não se pode levantar (porque tem um collider em cima), e está a andar a crouch speed ou menos
            (isGrounded && !canUncrouch && currentTotalSpeed <= CrouchSpeed)
        ) {
            isCrouched = true;
            NewCameraPosition(CrouchHeight);

        } else {
            isCrouched = false;
            NewCameraPosition(standingHeight);
        }
    }

    private void UncrouchCheck() {
        // https://docs.unity3d.com/ScriptReference/Physics.Raycast.html

        // Bit shift the index of the layer (8) to get a bit mask
        int layerMask = 1 << 8;
        // This would cast rays only against colliders in layer 8.
        // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
        layerMask = ~layerMask;

        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.up), out hit, 2, layerMask))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.up) * hit.distance, Color.red);
            canUncrouch = false;
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.up) * 2, Color.green);
            canUncrouch = true;
        }
    }

    private void WallJump() {
        // 1. deteta colisoes com paredes
        // 2. apanha a normal das paredes
        // 3. ao saltar, dar impulso pra cima e na direção da normal.

        if ((_controller.collisionFlags & CollisionFlags.Sides) != 0) {
            // Debug.Log("Touching sides!");
            
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit) {

        if (hit.moveDirection.y >= -0.3) {
            // Debug.Log("Collision! Normal: " + hit.normal.ToString("F3"));
            WallJumpDirection = hit.normal;

            if (_input.jump)
            {
                playerVelocity += WallJumpDirection;
            }


        }
    }


    void NewCameraPosition(float newHeight) {
        Vector3 cam_pos = cameraPosition.transform.localPosition;
        cameraPosition.transform.localPosition = new Vector3(cam_pos.x, newHeight - 0.2f, cam_pos.z);
        _controller.height = newHeight;
        _controller.center = new Vector3(0, newHeight / 2f, 0);
    }
}
