using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GelManajer : MonoBehaviour
{

    public GameManager gm;
    public GameObject Player;
    public PlayerMovement pm;
    public TMP_Text GelText;

    // Start is called before the first frame update
    void Start()
    {
        pm = Player.GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        
        GelText.text = "Duração do Gel: " + Mathf.Round(pm.GelTimer);

    }



}
