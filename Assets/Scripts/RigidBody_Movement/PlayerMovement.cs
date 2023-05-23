using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerMovement : MonoBehaviour {

    [Space(10)]
    [Header("Run")]
    [Tooltip("Initial running speed")]
    public float RunInitialSpeed = 6f;
    public float RunMaxSpeed = 12f;
    public float RunSpeedMultiplier = 0.1f;
    private float CurrentRunningSpeed;

    [Space(10)]
    [Header("Other")]
    public bool grounded = true;
    public float groundDrag = 1f;
    public GameObject GroundCheck;
    public LayerMask CharacterLayer;
    private float GroundCheckSphereRadius = 0.3f;
    private float playerHeight;
    

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
        playerHeight = 1.9f;

    }

    void Update() {
        // ground check
        grounded = IsGrounded();

        if (grounded)
            _rb.drag = groundDrag;
        else
            _rb.drag = 0;
    }

    void FixedUpdate() {
        Move();
    }

    void LateUpdate() {

    }

    void Move() {
        MovingDirection = transform.forward * _input.move.y + transform.right * _input.move.x;

        _rb.AddForce(MovingDirection * RunInitialSpeed * 10f, ForceMode.Force);

    }

    bool IsGrounded() {
        return Physics.CheckSphere(GroundCheck.transform.position + new Vector3(0, GroundCheckSphereRadius - 0.2f, 0), GroundCheckSphereRadius, ~CharacterLayer);
    }
}
