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
    private float CurrentRunningSpeed = 0;

    [Space(10)]
    [Header("Jump")]
    public float JumpHeight = 2f;
    public float JumpCooldown = 1f;
    public bool isJumping = false;
    public bool CanJump = true;
    public float JumpBoost = 0.4f;
    public float MaxJumpBoost = 3.2f;
    private float CurrentJumpBoost = 0;

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
    private Vector3 MovingDirection;
    public float TotalSpeed;

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
            _rb.drag = airDrag;
        }

        Jump();
        Move(); // deixar o move sempre pra ultimo pls
    }

    void LateUpdate() {

    }

    void Move() {
        MovingDirection = transform.forward * _input.move.y + transform.right * _input.move.x;

        if(MovingDirection.magnitude <= 0.1f) {
            CurrentRunningSpeed = 0f;
            CurrentJumpBoost = 0f;
        } else {
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

        // aqui vamos metendo os boosts das outras mecanicas
        TotalSpeed = CurrentRunningSpeed + CurrentJumpBoost;

        _rb.AddForce(MovingDirection * TotalSpeed * (grounded ? 1f : AirMultiplier) * 10f, ForceMode.Force);

    }

    private void Jump()
    {
        if (_input.jump && grounded && !isJumping && CanJump) {
            isJumping = true;
            CanJump = false;
            CurrentJumpBoost = (CurrentJumpBoost >= MaxJumpBoost) ? MaxJumpBoost : (CurrentJumpBoost + JumpBoost);
            Invoke(nameof(ResetJump), JumpCooldown);
            _rb.velocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);
            _rb.AddForce(transform.up * JumpHeight * 3f, ForceMode.Impulse);
        }
    }
    private void ResetJump() {
        CanJump = true;
    }

    bool IsGrounded() {
        return Physics.CheckSphere(GroundCheck.transform.position + new Vector3(0, GroundCheckSphereRadius - 0.2f, 0), GroundCheckSphereRadius, ~CharacterLayer);
    }
}
