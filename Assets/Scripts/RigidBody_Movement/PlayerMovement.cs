using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerMovement : MonoBehaviour {

    [Space(5)]
    [Header("Run")]
    [Tooltip("Initial running speed")]
    public float RunInitialSpeed = 6f;
    public float RunMaxSpeed = 12f;
    public float RunSpeedMultiplier = 0.1f;
    [HideInInspector] public float CurrentRunningSpeed = 0;

    [Space(10)]
    [Header("Jump")]
    public float JumpHeight = 3f;
    public float JumpCooldown = 1f;
    public bool isJumping = false;
    public bool isFalling = false;
    public bool isMidAir = false;
    public bool isLanding = false;
    public bool CanJump = true;
    public float JumpBoost = 0.4f;
    public float MaxJumpBoost = 3.2f;
    private float CurrentJumpBoost = 0;

    [Space(10)]
    [Header("Wall Jump")]
    public float WallJumpHeight = 3f;
    public float WallJumpPushMultiplier = 2f;
    public float WallJumpCooldown = 0.1f;
    public bool CanWallJump = true;
    public bool IsTouchingWall = false;
    private Vector3 WallJumpNormal = Vector3.zero;

    [Space(10)]
    [Header("Crouch")]
    [Tooltip("Crouch speed of the character in m/s. Overrides all other speeds.")]
    public float CrouchSpeed = 4.0f;
    [Tooltip("The height of the player when crouched, in meters.")]
    public float CrouchHeight = 1.0f;
    public bool isCrouched = false;
    private float standingHeight;
    public bool canUncrouch;

    [Space(10)]
    [Header("Gel")]
    public float GelDuration = 10f;
    public float MaxGelBoost = 5f;
    public bool isGelActivated = false;
    [HideInInspector] public float GelTimer = 0f;
    private bool isRespawning = false;
    public GameObject gelPrefab;
    public Transform gelSpawnPoint;
    public GelManajer gelManager;

    [Space(10)]
    [Header("Sliding")]
	public float maxSlideBoost;
	public float slideForce;
	public float slideTimer;
	public float slideYScale;
	public float startYScale;
    public bool isSliding;

    [Header ("Input")]
	public KeyCode slideKey = KeyCode.LeftControl;
	private float horizontalInputS;
	private float verticalInputS;

    [Header("References")]
	public Transform orientation;
	private PlayerMovementAdvanced pm;


    [Space(10)]
    [Header("Other")]
    public bool grounded = true;
    public float groundDrag = 1f;
    public float airDrag = 1f;
    public GameObject GroundCheck;
    public float AirMultiplier = 0.5f;
    [Space(5)]
    public LayerMask CharacterLayer;
    private float GroundCheckSphereRadius = 0.3f;
    

    // Helpers
    [Space(10)]
    public float TotalSpeed;

    // Components
    private StarterAssetsInputs _input;
    private Rigidbody _rb;
    private CapsuleCollider _collider;
    private GameObject _camera;


    void Start() {
        _input = GetComponent<StarterAssetsInputs>();
        _rb = GetComponent<Rigidbody>();
        _collider = GetComponent<CapsuleCollider>();
        _camera = transform.Find("CameraPosition").gameObject;

        standingHeight = _collider.height;

        gelManager = FindObjectOfType<GelManajer>();
        //sliding
        //pm = GetComponent<PlayerMovementAdvanced>();
		//startYScale = playerObj.localScale.y;
    }

    void Update() {
        CheckGel(); //para maior precisao nos segundos
    }

    void FixedUpdate() {
        UncrouchCheck();
        

        

        Run();
        Crouch();
        Jump();
        WallJump();
        Move(); // deixar o move sempre pra ultimo pls //Ok caro colega <3 // mas eu queria meter por baixo :( // faz crounch no meu rigidbody <3 // ulalah
    }

    void LateUpdate() {
        grounded = IsGrounded();
        isJumping = !IsGrounded();

        _rb.drag = grounded ? groundDrag : airDrag;
    }

    Vector3 MovingDirection() {
        return (transform.forward * _input.move.y + transform.right * _input.move.x);
    }

    void Move() {

        // reset aos boosts e velocidades se o marmelo estiver parado
        // se nao estiver a carregar no wasd OU
        // se a velocidade do rb for 0
        if(MovingDirection().magnitude <= 0.1f) {
            CurrentRunningSpeed = RunInitialSpeed;
            CurrentJumpBoost = 0f;
        }

        // aqui vamos metendo os boosts das outras mecanicas
        TotalSpeed = CurrentRunningSpeed + CurrentJumpBoost;

        if (isGelActivated)
        {
            TotalSpeed += MaxGelBoost; //nao tenho a certeza de fazer isto assim mas funfa
        }

        if(isCrouched)
        {
            TotalSpeed = CrouchSpeed;
        }

        groundDrag = TotalSpeed * 0.80f;

        _rb.AddForce(MovingDirection() * TotalSpeed * (grounded ? 1f : AirMultiplier) * 10f, ForceMode.Force);

    }

    private void Run() {
        if (MovingDirection().magnitude > 0.1f) {
            if(CurrentRunningSpeed < RunInitialSpeed) {
                CurrentRunningSpeed = RunInitialSpeed;
            }
            if(CurrentRunningSpeed >= RunInitialSpeed && CurrentRunningSpeed < RunMaxSpeed) {
                CurrentRunningSpeed += RunSpeedMultiplier;
            }
            if(CurrentRunningSpeed >= RunMaxSpeed) {
                CurrentRunningSpeed = RunMaxSpeed;
            }
        }
    }

    private void Jump()
    {
        if(isJumping) { isFalling = true; }
        if (_input.jump && grounded && CanJump) {
            CanJump = false;
            CurrentJumpBoost = (CurrentJumpBoost >= MaxJumpBoost) ? MaxJumpBoost : (CurrentJumpBoost + JumpBoost);
            Invoke(nameof(ResetJump), JumpCooldown);
            _rb.velocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);
            _rb.AddForce(transform.up * JumpHeight * 3f, ForceMode.Impulse);
        }

        if(isJumping)
        {
            if(Mathf.Round(_rb.velocity.y) > -1) { isMidAir = true; }
            if(Mathf.Round(_rb.velocity.y) <= -1 ) { isFalling = true; isMidAir = false; }

        } else { isFalling = false; }

        if (!isJumping && IsGrounded()) { isLanding = false; }
    }
    private void ResetJump() { CanJump = true; }

    void Crouch()
    {
        if (isCrouched && _rb.velocity.y < 0) { isSliding = true; } 
        else { isSliding = false; }


        // se est� no ch�o, premiu crouch, e est� parado
        if (
            (grounded && _input.crouch /*&& TotalSpeed <= CrouchSpeed*/) ||
        // Se est� no ch�o, n�o se pode levantar (porque tem um collider em cima), e est� a andar a crouch speed ou menos
            (grounded && !canUncrouch /*&& TotalSpeed <= CrouchSpeed*/))
        {
            isCrouched = true;
            NewCameraPosition(CrouchHeight);

        } else if (!_input.crouch && canUncrouch)
        {
            isCrouched = false;
            NewCameraPosition(standingHeight);
        }
    }

    private void UncrouchCheck()
    {
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

    private void CheckGel()
    {
        if(isGelActivated)
        {
            GelTimer -= Time.deltaTime;

            if(GelTimer <= 0)
            {
                ActivateGel(false);
            }
            
            if(GelTimer <= GelDuration / 2 && isRespawning)
            {
                RespawnGel();
                // Debug.Log("SPAWNEI");
            }
        }
    }

    private void ActivateGel(bool activate)
    {
        if(activate)
        {
            isGelActivated = true;
            GelTimer = GelDuration;
            isRespawning = true;
        }
        else
        {
            isGelActivated = false;
            GelTimer = 0f;
        }
    }

    private void OnTriggerEnter(Collider gel)
    {
        if(gel.CompareTag("Gel"))
        {
            ActivateGel(true);
            gelManager.CollectGel();
            Destroy(gel.gameObject);
        }
    }

    private void RespawnGel()
    {
        isRespawning = false;
        Instantiate(gelPrefab, gelSpawnPoint.position, gelSpawnPoint.rotation);
    }

    private void WallJump() {
        if(_input.jump && !grounded && IsTouchingWall && CanWallJump) {
            // cooldown
            CanWallJump = false;
            Invoke(nameof(ResetWallJump), WallJumpCooldown);

            // reset vertical speed
            _rb.velocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);

            // add força pra cima e para o lado contrário à parede
            _rb.AddForce((transform.up * WallJumpHeight + WallJumpNormal * WallJumpPushMultiplier) * 3f, ForceMode.Impulse);
        }
    }
    private void ResetWallJump() { CanWallJump = true; }

    void OnCollisionStay(Collision collisionInfo) {

        if(collisionInfo.collider.CompareTag("Wall")) {
            WallJumpNormal = collisionInfo.GetContact(0).normal;
            IsTouchingWall = true;
        } 
        
    }

    void OnCollisionExit(Collision collisionInfo) {
        if(collisionInfo.collider.CompareTag("Wall")) {
            IsTouchingWall = false;

        }
    }


    void NewCameraPosition(float newHeight)
    {
        Vector3 cam_pos = _camera.transform.localPosition;
        _camera.transform.localPosition = new Vector3(cam_pos.x, newHeight - 0.2f, cam_pos.z);
        _collider.height = newHeight;
        _collider.center = new Vector3(0, newHeight / 2f, 0);
    }

    bool IsGrounded() {
        return Physics.CheckSphere(GroundCheck.transform.position + new Vector3(0, GroundCheckSphereRadius - 0.2f, 0), GroundCheckSphereRadius, ~CharacterLayer);
    }

 //   private void StartSlide()
	//{
	//	isSliding = true;

	//	playerObj.localScale = new Vector3(playerObj.localScale.x, slideYScale,playerObj.localScale.z);
	//	_rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);

	//	slideTimer = maxSlideBoost;
	//}

	//private void UpdateSlide()
	//{
	//	horizontalInputS = Input.GetAxisRaw("Horizontal");
	//	verticalInputS = Input.GetAxisRaw("Vertical");

 //       Debug.Log("horizontalInputS: " + horizontalInputS);
 //       Debug.Log("verticalInputS: " + verticalInputS);


	//	if (Input.GetKeyDown(slideKey) && (horizontalInputS < 0 || verticalInputS < 0))
	//	    StartSlide();

	//	if(Input.GetKeyUp(slideKey) && isSliding)
	//	    StopSlide();
	//}

	//private void FixedUpdateSliding()
	//{
 //       UpdateSlide();

 //       if (isSliding)
	//		SlidingMovement();

	//}

	//private void SlidingMovement()
	//{
	//	Vector3 inputDirection = orientation.forward * verticalInputS + orientation.right * horizontalInputS;

	//	_rb.AddForce(inputDirection.normalized * slideForce, ForceMode.Force);

	//	slideTimer -= Time.deltaTime;

	//	if(slideTimer <= 0)
	//		StopSlide();
				
	//}

	//private void StopSlide()
	//{
 //       isSliding = false;

	//	playerObj.localScale = new Vector3(playerObj.localScale.x, startYScale, playerObj.localScale.z);
	//}

}
