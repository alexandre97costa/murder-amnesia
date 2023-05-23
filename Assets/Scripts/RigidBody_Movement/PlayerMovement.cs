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
    private float CurrentRunningSpeed;

    [Space(10)]
    [Header("Jump")]
    public float JumpHeight = 2f;
    public float JumpCooldown = 1f;
    public bool isJumping = false;
    public bool CanJump = true;

    [Space(10)]
    [Header("Other")]
    public bool grounded = true;
    public float groundDrag = 1f;
    public GameObject GroundCheck;
    public float AirMultiplier = 0.5f;
    [Space(5)]
    public LayerMask CharacterLayer;
    private float GroundCheckSphereRadius = 0.3f;
    

    // Helpers
    private Vector3 MovingDirection;
    private float TotalSpeed;

    // Components
    private StarterAssetsInputs _input;
    private Rigidbody _rb;
    private CapsuleCollider _collider;
    


    void Start() {
        _input = GetComponent<StarterAssetsInputs>();
        _rb = GetComponent<Rigidbody>();
        _collider = GetComponent<CapsuleCollider>();

    }

    void Update() {
        
    }

    void FixedUpdate() {
        // ground check
        grounded = IsGrounded();

        if (grounded) {
            isJumping = false;
            _rb.drag = groundDrag;
        } else {
            _rb.drag = 0;
        }

        Move();
        Jump();
    }

    void LateUpdate() {

    }

    void Move() {
        MovingDirection = transform.forward * _input.move.y + transform.right * _input.move.x;

        _rb.AddForce(MovingDirection * RunInitialSpeed * (grounded ? 1f : AirMultiplier) * 10f, ForceMode.Force);

    }

    private void Jump()
    {
        if (_input.jump && grounded && !isJumping && CanJump) {
            isJumping = true;
            CanJump = false;
            Invoke(nameof(ResetJump), JumpCooldown);
            _rb.velocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);
            _rb.AddForce(transform.up * JumpHeight, ForceMode.Impulse);
        }
    }
    private void ResetJump() {
        CanJump = true;
    }

    bool IsGrounded() {
        return Physics.CheckSphere(GroundCheck.transform.position + new Vector3(0, GroundCheckSphereRadius - 0.2f, 0), GroundCheckSphereRadius, ~CharacterLayer);
    }
}
