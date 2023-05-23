using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GetSpeedValue : MonoBehaviour
{
    public GameObject Speed;
    private TMP_Text Speed_text;
    public GameObject Sliding;
    public GameObject Crouching;
    public GameObject Player;
    private InertiaMovement PlayerMovement;
    private Rigidbody PlayerRigidBody;
    private float PlayerCurrentSpeed;
    private float PlayerVelocity;
    // Start is called before the first frame update
    void Start()
    {
        PlayerMovement = Player.GetComponent<InertiaMovement>();
        PlayerRigidBody = Player.GetComponent<Rigidbody>();

        Speed_text = Speed.GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        PlayerCurrentSpeed = Mathf.Round(PlayerMovement.currentTotalSpeed * 100.0f) / 100.0f;
        PlayerVelocity = Mathf.Round(PlayerRigidBody.velocity.magnitude * 100.0f) / 100.0f;
        Speed_text.text = "Speed Value: " + PlayerCurrentSpeed + "\nActual Velocity: " + PlayerVelocity;

        Sliding.SetActive(PlayerMovement.isSliding);
        Crouching.SetActive(PlayerMovement.isCrouched);
    }
}
