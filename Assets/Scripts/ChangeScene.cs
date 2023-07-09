using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class ChangeScene : MonoBehaviour
{
    [Header("Choose Scene")]
    public ListScene sceneSelect;
    private List<string> listScene = new List<string> {
        "Cha1_Lvl2", "Cha1_Lvl3", "CutScene2", "Cha2_Lvl1", "Cha2_Lvl2", "Cha2_Lvl3", "CutScene3", "Cha3_Lvl1", "Cha3_Lv2", "CutScene4", "CutScene5", "CutScene6", "End", "DemoTutorial", "MenuStart", "Creditos"
    };

    [Space(10)]
    [Header("CutScene")]
    public VideoPlayer CutScene;

    public enum ListScene
    {
        Cha1_Lvl2 = 0,
        Cha1_Lvl3 = 1,
        CutScene2 = 2,

        Cha2_Lvl1 = 3,
        Cha2_Lvl2 = 4,
        Cha2_Lvl3 = 5,
        CutScene3 = 6,

        Cha3_Lvl1 = 7,
        Cha3_Lvl2 = 8,
        CutScene4 = 9,
        CutScene5 = 10,
        CutScene6 = 11,

        End = 12,
        DemoTutorial = 13,
        Menu = 14,
        creditos = 15,
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
            ChangeSceneManager();
    }

    void ChangeSceneManager()
    {
        SceneManager.LoadScene(listScene[(int)sceneSelect]);
    }

    private void Start()
    {
        CutScene.loopPointReached += VideoPlayer_LoopPointReached;
    }

    private void VideoPlayer_LoopPointReached(VideoPlayer source)
    {
        ChangeSceneManager();
    }
}
