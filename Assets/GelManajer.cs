using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GelManajer : MonoBehaviour
{
    public float gel;
    float maxGel = 5;

    public Slider gelBar;
    public float decreaseTime = 5f; 
    private float decreaseRate; 
    private float decreaseTimer; 

    void Start()
    {
        maxGel += gel;
        gelBar.maxValue = maxGel;
        decreaseRate = maxGel / decreaseTime;
    }

    void Update()
    {
        if (decreaseTimer > 0f)
        {
            DecreaseEnergy();
            decreaseTimer -= Time.deltaTime;
        }
        
        gelBar.value = gel;
    }

    private void DecreaseEnergy()
    {
        gel -= decreaseRate * Time.deltaTime;

        if (gel <= 0f)
        {
            gel = 0f;
            decreaseTimer = 0f;
        }
    }

    public void CollectGel()
    {
        gel = maxGel;
        decreaseTimer = decreaseTime;
    }
}
