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
	public class FirstPersonController : MonoBehaviour
	{
		public enum MovementType {Default, Inertia};
		[Space(10)]
		[Tooltip("Choose which movement type will control the character")]
		public MovementType movementType;

		[Header("Inertia Movement")]
		
		// 🏃‍♂️ Run
		[Space(5)]
		[Tooltip("Initial movement speed (in m/s).")]
		public float InitialSpeed = 7.0f;
		[Tooltip("Maximum speed obtainable by running (in m/s). Will stack with boosts.")]
		public float MaxSpeed = 10.0f;
		[Tooltip("How fast will the player reach max speed (in m/s).")]
		public float RunningAcceleration = 2.0f;
		
		// 🦘 Jump
		[Space(5)]
		[Tooltip("How much will a jump increase the player's speed.")]
		public float JumpBoost = 1.1f;
		[Tooltip("The maximum boost obtainable by jumps.")]
		public float MaxJumpBoost = 4.0f;

		// 🎿 Slide
		[Space(5)]
		[Tooltip("How much will a slide increase the player's speed.")]
		public float SlideBoost = 1.1f;
		[Tooltip("The maximum boost obtainable by slides.")]
		public float MaxSlideBoost = 4.0f;
		[Tooltip("How much will the ramp angle affect the acceleration.")]
		public float SlideAngleMultiplier = 1.0f;
		[Tooltip("How much will the player slow down when sliding in flat surfaces.")]
		public float SlideDecayMultiplier = 1.0f;

		// 🕺 Wall Jump
		[Space(5)]
		[Tooltip("How much will a wall jump increase the player's speed.")]
		public float WallJumpBoost = 1.1f;
		[Tooltip("The maximum boost obtainable by wall jumps.")]
		public float MaxWallJumpBoost = 4.0f;

		// 🏄‍♂️ Wall Run
		[Space(5)]
		[Tooltip("How much will a wall run increase the player's speed.")]
		public float WallRunBoost = 1.1f;
		[Tooltip("The maximum boost obtainable by wall runs.")]
		public float MaxWallRunBoost = 4.0f;
		[Tooltip("How long will the player be able to stick to a wall (in seconds).")]
		public float WallRunDecayMultiplier = 3.0f;

		[ContextMenu("Calculate Max Speed")]
		void CalculateMaxSpeed() {
			float max = MaxSpeed + MaxJumpBoost + MaxSlideBoost + MaxWallJumpBoost + MaxWallRunBoost;
			Debug.Log("Theoretical Max Speed: " + max + "m/s");
		}

		[Space(10)]
		[Header("Player")]
		[Tooltip("Move speed of the character in m/s")]
		public float MoveSpeed = 8.0f;
		[Tooltip("Sprint speed of the character in m/s")]
		public float SprintSpeed = 16.0f;
		[Tooltip("Crouch speed of the character in m/s")]
		public float CrouchSpeed = 4.0f;
		[Tooltip("The height of the player when crouched, in meters")]
		public float CrouchHeight = 1.0f;
		[Tooltip("Rotation speed of the character")]
		public float RotationSpeed = 1.0f;
		[Tooltip("Acceleration and deceleration")]
		public float SpeedChangeRate = 10.0f;

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

		[Header("Player Grounded")]
		[Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
		public bool Grounded = true;
		[Tooltip("Useful for rough ground")]
		public float GroundedOffset = -0.14f;
		[Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
		public float GroundedRadius = 0.22f;
		[Tooltip("What layers the character uses as ground")]
		public LayerMask GroundLayers;

		[Header("Cinemachine")]
		[Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
		public GameObject CinemachineCameraTarget;
		[Tooltip("How far in degrees can you move the camera up")]
		public float TopClamp = 90.0f;
		[Tooltip("How far in degrees can you move the camera down")]
		public float BottomClamp = -90.0f;

		// cinemachine
		private float _cinemachineTargetPitch;

		// 🚀 inertia movement
		private float currentRunningSpeed;
		private float currentJumpBoost;
		private float currentSlideBoost;
		private float currentWallJumpBoost;
		private float currentWallRunBoost;

		// player
		private float _speed;
		private float _rotationVelocity;
		private float _verticalVelocity;
		private float _terminalVelocity = 53.0f;

		// crouch
		private float standingHeight;
		private bool canUncrouch;

		// timeout deltatime
		private float _jumpTimeoutDelta;
		private float _fallTimeoutDelta;

	
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
			if (cameraPosition == null) {
				Debug.Log("The player's camera position was not found! The crouch view will not work.");
			}

			// reset our timeouts on start
			_jumpTimeoutDelta = JumpTimeout;
			_fallTimeoutDelta = FallTimeout;

			standingHeight = _controller.height;
		}

		private void Update() {
			JumpAndGravity();
			GroundedCheck();
			Move();
			Crouch();
		}

		private void FixedUpdate() {
			UncrouchCheck();
		}

		private void LateUpdate() {
			CameraRotation();
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

		private void GroundedCheck() {
			// set sphere position, with offset
			Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
			Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
		}

		private void CameraRotation() {
			// if there is an input
			if (_input.look.sqrMagnitude >= _threshold)
			{
				//Don't multiply mouse input by Time.deltaTime
				float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;
				
				_cinemachineTargetPitch += _input.look.y * RotationSpeed * deltaTimeMultiplier;
				_rotationVelocity = _input.look.x * RotationSpeed * deltaTimeMultiplier;

				// clamp our pitch rotation
				_cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

				// Update Cinemachine camera target pitch
				CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);

				// rotate the player left and right
				transform.Rotate(Vector3.up * _rotationVelocity);
			}
		}

		private void Move() {
			// set target speed based on move speed, sprint speed and if sprint is pressed

			float targetSpeed = MoveSpeed;

			if (Grounded) {
				if (_input.crouch || !canUncrouch) {
					targetSpeed = CrouchSpeed;
				} else if (_input.sprint) {
						targetSpeed = SprintSpeed;
				}
			}

			// a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

			// note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
			// if there is no input, set the target speed to 0
			if (_input.move == Vector2.zero) targetSpeed = 0.0f;

			// a reference to the players current horizontal velocity
			float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

			float speedOffset = 0.1f;
			float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

			// accelerate or decelerate to target speed
			if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
			{
				// creates curved result rather than a linear one giving a more organic speed change
				// note T in Lerp is clamped, so we don't need to clamp our speed
				_speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);

				// round speed to 3 decimal places
				_speed = Mathf.Round(_speed * 1000f) / 1000f;
			}
			else
			{
				_speed = targetSpeed;
			}

			// normalise input direction
			Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

			// note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
			// if there is a move input rotate player when the player is moving
			if (_input.move != Vector2.zero)
			{
				// move
				inputDirection = transform.right * _input.move.x + transform.forward * _input.move.y;
			}

			// move the player
			_controller.Move(inputDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
		}

		private void JumpAndGravity() {
			if (Grounded)
			{
				// reset the fall timeout timer
				_fallTimeoutDelta = FallTimeout;

				// stop our velocity dropping infinitely when grounded
				if (_verticalVelocity < 0.0f)
				{
					_verticalVelocity = -2f;
				}

				// Jump
				if (_input.jump && _jumpTimeoutDelta <= 0.0f)
				{
					Debug.Log("Jump!");
					// the square root of H * -2 * G = how much velocity needed to reach desired height
					_verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
				}

				// jump timeout
				if (_jumpTimeoutDelta >= 0.0f)
				{
					_jumpTimeoutDelta -= Time.deltaTime;
				}
			}
			else
			{
				// reset the jump timeout timer
				_jumpTimeoutDelta = JumpTimeout;

				// fall timeout
				if (_fallTimeoutDelta >= 0.0f)
				{
					_fallTimeoutDelta -= Time.deltaTime;
				}

				// if we are not grounded, do not jump
				_input.jump = false;
			}

			// apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
			if (_verticalVelocity < _terminalVelocity)
			{
				_verticalVelocity += Gravity * Time.deltaTime;
			}
		}

		private static float ClampAngle(float lfAngle, float lfMin, float lfMax) {
			if (lfAngle < -360f) lfAngle += 360f;
			if (lfAngle > 360f) lfAngle -= 360f;
			return Mathf.Clamp(lfAngle, lfMin, lfMax);
		}

		private void OnDrawGizmosSelected() {
			Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
			Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

			if (Grounded) Gizmos.color = transparentGreen;
			else Gizmos.color = transparentRed;

			// when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
			Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
		}
	}
}