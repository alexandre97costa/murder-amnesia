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
    [Tooltip("How much will a slide increase the player's speed.")]
    public float SlideBoost = 1.1f;
    [Tooltip("The maximum boost obtainable by slides.")]
    public float MaxSlideBoost = 4.0f;
    [Tooltip("How much will the ramp angle affect the acceleration.")]
    public float SlideAngleMultiplier = 1.0f;
    [Tooltip("How much will the player slow down when sliding in flat surfaces.")]
    public float SlideDecayMultiplier = 1.0f;

    // 🕺 Wall Jump
    [Space(10)]
    [Header("Wall Jump")]
    [Tooltip("How much will a wall jump increase the player's speed.")]
    public float WallJumpBoost = 1.1f;
    [Tooltip("The maximum boost obtainable by wall jumps.")]
    public float MaxWallJumpBoost = 4.0f;

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
    [Header("Player Grounded")]
    [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
    public bool Grounded = true;
    [Tooltip("Useful for rough ground")]
    public float GroundedOffset = -0.14f;
    [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
    public float GroundedRadius = 0.22f;
    [Tooltip("What layers the character uses as ground")]
    public LayerMask GroundLayers;

    [ContextMenu("Calculate Max Speed")]
    void CalculateMaxSpeed() {
        float max = MaxRunningSpeed + MaxJumpBoost + MaxSlideBoost + MaxWallJumpBoost + MaxWallRunBoost;
        Debug.Log("Theoretical Max Speed: " + max + "m/s");
    }

    // 🚀 inertia movement speed
    [HideInInspector] public float currentRunningSpeed = 0.0f;
    [HideInInspector] public float currentJumpBoost;
    [HideInInspector] public float currentSlideBoost;
    [HideInInspector] public float currentWallJumpBoost;
    [HideInInspector] public float currentWallRunBoost;

    // crouch
	private float standingHeight;
	private bool canUncrouch;

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
        Move();
        Crouch();
    }

    void FixedUpdate() {
        UncrouchCheck();
    }

    void Move() {
            // direção do movimento
			Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y);
            inputDirection = transform.right * _input.move.x + transform.forward * _input.move.y;

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

            Debug.Log("Speed: " + currentRunningSpeed );

			_controller.Move(inputDirection.normalized * currentRunningSpeed * Time.deltaTime);
    }

    private void Crouch() {
        Vector3 cam_pos = cameraPosition.transform.localPosition;

        if (_input.crouch || (Grounded && !canUncrouch)) {
            cameraPosition.transform.localPosition = new Vector3(cam_pos.x, CrouchHeight - 0.2f, cam_pos.z);
            _controller.height = CrouchHeight;
            _controller.center = new Vector3(0, CrouchHeight / 2f, 0);

        } else {
            cameraPosition.transform.localPosition = new Vector3(cam_pos.x, standingHeight - 0.2f, cam_pos.z);
            _controller.height = standingHeight;
            _controller.center = new Vector3(0, standingHeight / 2f, 0);
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
}
