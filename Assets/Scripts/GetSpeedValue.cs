using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GetSpeedValue : MonoBehaviour
{
    public TMP_Text text;
    public GameObject Player;
    public InertiaMovement PlayerMovement;
    public CharacterController PlayerController;
    public float PlayerCurrentSpeed;
    public float PlayerVelocity;
    // Start is called before the first frame update
    void Start()
    {
        text = gameObject.GetComponent<TMP_Text>();
        PlayerMovement = Player.GetComponent<InertiaMovement>();
        PlayerController = Player.GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        PlayerCurrentSpeed = Mathf.Round(PlayerMovement.currentTotalSpeed * 100.0f) / 100.0f;
        PlayerVelocity = Mathf.Round(PlayerController.velocity.magnitude * 100.0f) / 100.0f;
        text.text = "Speed Value: " + PlayerCurrentSpeed + "\nActual Velocity: " + PlayerVelocity;
    }
}
