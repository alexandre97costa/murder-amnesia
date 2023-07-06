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
        "Cha1_Lvl2", "Cha1_Lvl3", "Cutscene2", "Cha2_Lvl1", "Cha2_Lvl2", "Cha2_Lvl3", "Cutscene3", "Cha3_Lvl1", "Cha3_Lv2", "Cutscene4", "Cutscene5", "Cutscene6", "End", "DemoTutorial"
    };

    [Space(10)]
    [Header("CutScene")]
    public VideoPlayer cutScene;

    public enum ListScene
    {
        Cha1_Lvl2 = 0,
        Cha1_Lvl3 = 1,
        Cutscene2 = 2,

        Cha2_Lvl1 = 3,
        Cha2_Lvl2 = 4,
        Cha2_Lvl3 = 5,
        Cutscene3 = 6,

        Cha3_Lvl1 = 7,
        Cha3_Lvl2 = 8,
        Cutscene4 = 9,
        Cutscene5 = 10,
        Cutscene6 = 11,

        End = 12,
        DemoTutorial = 13,
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
        cutScene.loopPointReached += VideoPlayer_LoopPointReached;
    }

    private void VideoPlayer_LoopPointReached(VideoPlayer source)
    {
        ChangeSceneManager();
    }
}
