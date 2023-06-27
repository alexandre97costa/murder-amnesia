using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    [Header("Choose Scene")]
    public ListScene sceneSelect;
    private List<string> listScene = new List<string> {
        "Cha1_Lvl2", "Cha1_Lvl3", "Cha2_Lvl1", "Cha2_Lvl2", "End", "DemoTutorial"
    };

    public enum ListScene
    {
        Cha1_Lvl2 = 0,
        Cha1_Lvl3 = 1,
        Cha2_Lvl1 = 2,
        Cha2_Lvl2 = 3,
        End = 4,
        DemoTutorial = 5,
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
            ChangeSceneManager();
    }

    void ChangeSceneManager()
    {
        Debug.Log(listScene[((int)sceneSelect)]);
        SceneManager.LoadScene(listScene[((int)sceneSelect)]);
    }
}
