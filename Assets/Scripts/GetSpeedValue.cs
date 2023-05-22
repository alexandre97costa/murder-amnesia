using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GetSpeedValue : MonoBehaviour
{
    public TMP_Text text;
    public GameObject Player;
    public InertiaMovement PlayerMovement;
    public float PlayerCurrentSpeed;
    // Start is called before the first frame update
    void Start()
    {
        text = gameObject.GetComponent<TMP_Text>();
        PlayerMovement = Player.GetComponent<InertiaMovement>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        PlayerCurrentSpeed = Mathf.Round(PlayerMovement.currentTotalSpeed * 100.0f) / 100.0f;
        text.text = "Speed: " + PlayerCurrentSpeed;
    }
}
