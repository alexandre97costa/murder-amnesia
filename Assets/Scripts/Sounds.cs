using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footsteps : MonoBehaviour
{
    public AudioSource footsteps;
    public AudioSource audioSource;

   
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            footsteps.enabled = true;
        }
        else
        {
            footsteps.enabled = false;

        }
    } public void IncreaseSpeed(float incrementAmount)
    {
        audioSource.pitch += incrementAmount;
    }
}


// using UnityEngine;

// public class SoundSpeedController : MonoBehaviour
// {
//     public AudioSource audioSource;
//     public float maxPlayerSpeed = 10f;
//     public float maxPitchMultiplier = 2f;

//     private Rigidbody playerRigidbody;

//     private void Awake()
//     {
//         playerRigidbody = GetComponent<Rigidbody>();
//     }

//     private void Update()
//     {
//         // Calculate the pitch multiplier based on player speed
//         float speedMultiplier = Mathf.Clamp01(playerRigidbody.velocity.magnitude / maxPlayerSpeed);
//         float pitchMultiplier = Mathf.Lerp(1f, maxPitchMultiplier, speedMultiplier);

//         // Set the pitch of the audio source
//         audioSource.pitch = pitchMultiplier;
//     }
// }
