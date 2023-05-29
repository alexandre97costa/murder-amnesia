using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerCamera : MonoBehaviour
{

    public float LookSensivity = 1f;
    public GameObject CameraPosition;
    public Camera MainCamera;
    [Tooltip("Draw a debug ray to show where the player is looking.")]
    public bool DebugRay = false;

    [HideInInspector] public float CameraPitch;
    [HideInInspector] public float PlayerRotation;
    private StarterAssetsInputs _input;

    // Start is called before the first frame update
    void Start() {
        _input = GetComponent<StarterAssetsInputs>();
    }

    void Update() {
        if (_input.jump) {
            Fov(80f);
        }
    }

    // Update is called once per frame
    void LateUpdate() {
         PlayerCameraRotation();
    }

    private void PlayerCameraRotation() {
        CameraPitch += _input.look.y * LookSensivity;
        CameraPitch = Mathf.Clamp(CameraPitch, -90f, 90f);
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

        MainCamera.fieldOfView = newFov;
    }
    public void ResetFov() {
        Fov(60.0f);
    }
}