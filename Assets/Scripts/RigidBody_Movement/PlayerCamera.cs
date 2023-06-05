using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class PlayerCamera : MonoBehaviour
{

    public float LookSensivity = 1f;
    public GameObject CameraPosition;
    public GameObject PlayerFollowCamera;
    private CinemachineVirtualCamera CinemachineCam;
    [Tooltip("Draw a debug ray to show where the player is looking.")]
    public bool DebugRay = false;

    [HideInInspector] public float CameraPitch;
    [HideInInspector] public float PlayerRotation;
    private StarterAssetsInputs _input;

    private Rigidbody _rb;
    private float PlayerSpeed;

    // Start is called before the first frame update
    void Start() {
        _input = GetComponent<StarterAssetsInputs>();
        _rb = GetComponent<Rigidbody>();
        CinemachineCam = PlayerFollowCamera.GetComponent<CinemachineVirtualCamera>();
    }

    void Update() {
        PlayerSpeed = new Vector3(_rb.velocity.x, 0 /*, _rb.velocity.z */).magnitude;
        PlayerSpeed = Mathf.Round(PlayerSpeed * 100) / 100;
        // Debug.Log("PlayerSpeed: " + PlayerSpeed);

        Fov(60f + (PlayerSpeed * 0.8f));
    }

    // Update is called once per frame
    void LateUpdate() {
         PlayerCameraRotation();
    }

    private void PlayerCameraRotation() {
        CameraPitch += _input.look.y * LookSensivity;
        CameraPitch = Mathf.Clamp(CameraPitch, -90f, 70f);
        PlayerRotation = _input.look.x * LookSensivity;


        // pitch the camera
        CameraPosition.transform.localRotation = Quaternion.Euler(CameraPitch, 0.0f, 0.0f);

        // rotate the player
        transform.Rotate(Vector3.up * PlayerRotation);

        // todo:
        /*
        if (DebugRay) {
            Debug.DrawRay(CameraPosition.transform.position, transform.TransformDirection(Vector3.forward + (Vector3.down * CameraPitch)), Color.white);
        }
        */
    }

    // todo: camera tilt
    public void Tilt(float tiltAngle) {
        // https://docs.unity3d.com/ScriptReference/Vector3.Slerp.html

        // 1. Definir um centro de pivot (centro ou pés?)
        // 2. Criar o slerp entre o angulo atual e o tiltAngle

    }
    public void ResetTilt() {
        Tilt(0.0f);
    }

    // todo: camera fov
    public void Fov(float newFov) {
        // opcional: 
        // https://docs.unity3d.com/ScriptReference/Vector3.Lerp.html

        CinemachineCam.m_Lens.FieldOfView = newFov;
    }
    public void ResetFov() {
        Fov(60.0f);
    }
}