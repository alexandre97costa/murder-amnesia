using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Timers;
using UnityEngine.SceneManagement;

public class Creditos : MonoBehaviour
{
    private float timer = 0f;
    private float sceneChangeDelay = 10f; // Tempo de espera em segundos

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= sceneChangeDelay)
        {
            ChangeScene();
        }
    }

    private void ChangeScene()
    {
        // Aqui você pode especificar o nome da cena que deseja carregar
        SceneManager.LoadScene("MenuStart");
    }
}