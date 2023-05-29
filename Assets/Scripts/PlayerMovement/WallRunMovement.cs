using UnityEngine;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	[RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
	[RequireComponent(typeof(PlayerInput))]
#endif
	public class WallRunMovement : MonoBehaviour
	{
		public enum MovementType { Default, Inertia };
		[Space(10)]
		[Tooltip("Choose which movement type will control the character")]
		public MovementType movementType;

		[Header("Inertia Movement")]

		// 🏄‍♂️ Wall Run
		[Space(5)]
		[Tooltip("How much will a wall run increase the player's speed.")]
		public float WallRunBoost = 1.1f;
		[Tooltip("The maximum boost obtainable by wall runs.")]
		public float MaxWallRunBoost = 4.0f;
		[Tooltip("How long will the player be able to stick to a wall (in seconds).")]
		public float WallRunDecayMultiplier = 3.0f;

		[Header("Cinemachine")]
		[Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
		public GameObject CinemachineCameraTarget;
		[Tooltip("How far in degrees can you move the camera up")]
		public float TopClamp = 90.0f;
		[Tooltip("How far in degrees can you move the camera down")]
		public float BottomClamp = -90.0f;

		[Header("References")]
		public Transform orientation;
		private Rigidbody rb;

		//Wall Running
		private bool wallrunning;
		//public KeyCode upwardsRunKey = KeyCode.LeftShift;
		//public KeyCode downwardsRunKey = KeyCode.LeftControl;
		private bool upwardsRunning;
		private bool downwardsRunning;
		private float horizontalInput;
		private float verticalInput;

		public LayerMask whatIsWall;
		public LayerMask whatIsGround;
		public float wallRunForce;
		public float wallJumpUpForce;
		public float wallJumpSideForce;
		public float wallClimbSpeed;
		public float maxWallRunTime;
		private float wallRunTimer;

		public float wallCheckDistance;
		public float minJumpHeight;
		private RaycastHit leftWallhit;
		private RaycastHit rightWallhit;
		private bool wallLeft;
		private bool wallRight;

		private bool exitingWall;
		public float exitWallTime;
		private float exitWallTimer;

		public bool useGravity;
		public float gravityCounterForce;

#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
		private PlayerInput _playerInput;
#endif
		private CharacterController _controller;
		private StarterAssetsInputs _input;
		private GameObject _mainCamera;
		private GameObject cameraPosition;


		private const float _threshold = 0.01f;

		private bool IsCurrentDeviceMouse
		{
			get
			{
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
				return _playerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;
#endif
			}
		}

		private void Awake()
		{
			// get a reference to our main camera
			if (_mainCamera == null)
			{
				_mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
			}
		}

		private void Start()
		{
			_controller = GetComponent<CharacterController>();
			_input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
			_playerInput = GetComponent<PlayerInput>();
#else
				Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif

			cameraPosition = transform.Find("CameraPosition").gameObject;
			if (cameraPosition == null)
			{
				Debug.Log("The player's camera position was not found! The crouch view will not work.");
			}
		}

		private void Update()
		{
			CheckForWall();
			StateMachine();
		}

		private void FixedUpdate()
		{
            if (wallrunning)
                WallRunningMovement();
        }

		//Wall Running
		private void CheckForWall()
		{
			wallRight = Physics.Raycast(transform.position, orientation.right, out rightWallhit, wallCheckDistance, whatIsWall);
			wallLeft = Physics.Raycast(transform.position, -orientation.right, out leftWallhit, wallCheckDistance, whatIsWall);
		}

		private bool AboveGround()
		{
			return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight, whatIsGround);
		}

		private void StateMachine()
		{
			// Getting Inputs
			//horizontalInput = Input.GetAxisRaw("Horizontal");
			//verticalInput = Input.GetAxisRaw("Vertical");

			//upwardsRunning = Input.GetKey(upwardsRunKey);
			//downwardsRunning = Input.GetKey(downwardsRunKey);

			// State 1 - Wallrunning
			if ((wallLeft || wallRight) && verticalInput > 0 && AboveGround() && !exitingWall)
			{
				if (!wallrunning)
					StartWallRun();

				// wallrun timer
				if (wallRunTimer > 0)
					wallRunTimer -= Time.deltaTime;

				if (wallRunTimer <= 0 && wallrunning)
				{
					exitingWall = true;
					exitWallTimer = exitWallTime;
				}

				// wall jump
				if (_input.wallrun)
					WallJump();
			}

			// State 2 - Exiting
			else if (exitingWall)
			{
				if (wallrunning)
					StopWallRun();

				if (exitWallTimer > 0)
					exitWallTimer -= Time.deltaTime;

				if (exitWallTimer <= 0)
					exitingWall = false;
			}

			// State 3 - None
			else
			{
				if (wallrunning)
					StopWallRun();
			}
		}

		private void StartWallRun()
		{
			wallrunning = true;

			wallRunTimer = maxWallRunTime;

			rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

			//DAR FIX
			// apply camera effects
			//cam.DoFov(90f);
			//if (wallLeft) cam.DoTilt(-5f);
			//if (wallRight) cam.DoTilt(5f);
		}

		private void WallRunningMovement()
		{
			rb.useGravity = useGravity;

			Vector3 wallNormal = wallRight ? rightWallhit.normal : leftWallhit.normal;

			Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

			if ((orientation.forward - wallForward).magnitude > (orientation.forward - -wallForward).magnitude)
				wallForward = -wallForward;

			// forward force
			rb.AddForce(wallForward * wallRunForce, ForceMode.Force);

			//NÃO USADO
			// upwards/downwards force
			//if (upwardsRunning)
			//	rb.velocity = new Vector3(rb.velocity.x, wallClimbSpeed, rb.velocity.z);
			//if (downwardsRunning)
			//	rb.velocity = new Vector3(rb.velocity.x, -wallClimbSpeed, rb.velocity.z);

			// push to wall force
			if (!(wallLeft && horizontalInput > 0) && !(wallRight && horizontalInput < 0))
				rb.AddForce(-wallNormal * 100, ForceMode.Force);

			// weaken gravity
			if (useGravity)
				rb.AddForce(transform.up * gravityCounterForce, ForceMode.Force);
		}

		private void StopWallRun()
		{
			wallrunning = false;

			// reset camera effects
			//cam.DoFov(80f);
			//cam.DoTilt(0f);
		}

		private void WallJump()
		{
			// enter exiting wall state
			exitingWall = true;
			exitWallTimer = exitWallTime;

			Vector3 wallNormal = wallRight ? rightWallhit.normal : leftWallhit.normal;

			Vector3 forceToApply = transform.up * wallJumpUpForce + wallNormal * wallJumpSideForce;

			// reset y velocity and add force
			rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
			rb.AddForce(forceToApply, ForceMode.Impulse);
		}
	}
}