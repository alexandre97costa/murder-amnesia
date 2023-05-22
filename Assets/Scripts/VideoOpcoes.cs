using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VideoOpcoes : MonoBehaviour
{

    public Toggle fullScreenToggle, vSyncToggle;

    // Start is called before the first frame update
    public void Start()
    {
        fullScreenToggle.isOn = Screen.fullScreen;


        if (QualitySettings.vSyncCount == 0)
        {
            vSyncToggle.isOn = false;
        }
        else
        {
            vSyncToggle.isOn = true;
        }
    }

    public void GuardarAlteracoes()
    {
        Screen.fullScreen = fullScreenToggle.isOn;

        if (vSyncToggle.isOn)
        {
            QualitySettings.vSyncCount = 1;
        }
        else
        {
            QualitySettings.vSyncCount = 0;
        }
    }
}
